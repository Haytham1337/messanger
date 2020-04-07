using Application.IServices;
using Application.IServices.IHelpers;
using Application.Models;
using Application.Models.AuthModels;
using Domain;
using Domain.Entities;
using Domain.Exceptions.UserExceptions;
using Infrastructure.AppSecurity;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IProvidersAuthService
    {
        Task<SignInResponce> FacebookAuthenticateAsync(FacebookAuthRequest model);
    }
    public class ProvidersAuthService : IProvidersAuthService
    {
        private readonly HttpClient _client;
        private readonly FbOptions _fbOptions;
        private readonly UserManager<SecurityUser> _userManager;
        private readonly IUnitOfWork _unit;
        private readonly IPhotoHelper _photoHelper;
        private readonly IJwtHelper _jwtHelper;
        private readonly TokenOption _options;

        public ProvidersAuthService(IOptions<FbOptions> fbOptions,
            UserManager<SecurityUser> userManager, IUnitOfWork unit,
            IPhotoHelper photoHelper, IJwtHelper jwtHelper, IOptions<TokenOption> options)
        {
            _client = new HttpClient();

            _fbOptions = fbOptions.Value;

            _userManager = userManager;

            _unit = unit;

            _photoHelper = photoHelper;

            _jwtHelper = jwtHelper;

            _options = options.Value;
        }

        public async Task<SignInResponce> FacebookAuthenticateAsync(FacebookAuthRequest model)
        {
            var requestUrl = $"{_fbOptions.validUri}input_token=" +
                $"{model.accessToken}&access_token={_fbOptions.appId}|{_fbOptions.appSecret}";

            var httpResponce =
                await _client.GetAsync(requestUrl);

            if (httpResponce.IsSuccessStatusCode)
            {
                using var responseStream = await httpResponce.Content.ReadAsStreamAsync();

                var validationResponce = await JsonSerializer.DeserializeAsync<ValidationResponce>(responseStream);

                if (validationResponce.data.is_valid)
                {
                    var user = await _userManager.FindByEmailAsync(model.email);

                    if (user == null)
                    {
                        var registerModel = new RegisterModel
                        {
                            Email = model.email,
                            NickName = model.lastName,
                            Sex = (Sex)Enum.Parse(typeof(Sex), model.gender)
                        };

                        await RegisterAsync(registerModel, model.photoUrl);
                    }

                    return await AuthenticateAsync(model.email);
                }

                throw new AccessTokenIsNotValidException(ExceptionMessages.InvalidToken, 400);
            }

            return await Task.FromResult(default(SignInResponce));
        }

        public async Task<IdentityResult> RegisterAsync(RegisterModel model, string photoUri)
        {
            SecurityUser user = new SecurityUser();
            user.Email = model.Email;
            user.UserName = model.Email;

            IdentityResult result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Chatter");

                var appUser = new User()
                {
                    NickName = model.NickName,
                    Age = model.Age,
                    PhoneNumber = model.PhoneNumber,
                    Sex = model.Sex,
                    Email = model.Email,
                    Photo = await _photoHelper.SavePhotoFromUriAsync(photoUri),
                    Id = user.Id
                };

                await _unit.UserRepository.CreateAsync(appUser);

                await _unit.Commit();
            }

            return result;
        }

        public async Task<SignInResponce> AuthenticateAsync(string email)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
                throw new UserNotExistException(ExceptionMessages.CredentialsNotValid, 400);

            var identity = await _jwtHelper.GetIdentityAsync(email);

            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            var now = DateTime.Now;

            var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromSeconds(_options.LifeTime)),
                    signingCredentials: new SigningCredentials(_options.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new SignInResponce
            {
                Access_Token = encodedJwt,
                ExpiresIn = now.Add(TimeSpan.FromSeconds(_options.LifeTime)),
                Refresh_Token = refreshToken
            };
        }
    }
}
