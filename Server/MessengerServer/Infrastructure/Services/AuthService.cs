using Application.Models;
using Domain;
using Domain.Entities;
using Infrastructure.AppSecurity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Domain.Exceptions.UserExceptions;
using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Application.IServices.IHelpers;

namespace Infrastructure.Services
{
    public interface IAuthService
    {
        Task<SignInResponce> AuthenticateAsync(LoginModel model);

        Task<IdentityResult> RegisterAsync(RegisterModel model);

        Task<bool> EmailExistAsync(CheckRegisterModel model);

        Task<User> FindByIdUserAsync(int id);

        Task<SignInResponce> ExchangeTokensAsync(ExchangeTokenRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<SecurityUser> _userManager;

        private readonly IUnitOfWork _unit;

        private readonly IConfiguration _config;

        private readonly TokenOption options;

        private readonly IJwtHelper _jwtHelper;

        public AuthService(UserManager<SecurityUser> userManager, IOptions<TokenOption> options,
         IUnitOfWork unit, IConfiguration config,IJwtHelper jwtHelper)
        {
            _userManager = userManager;

            _unit = unit;

            _config = config;

            this.options = options.Value;

            _jwtHelper = jwtHelper;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            SecurityUser user = new SecurityUser();
            user.Email = model.Email;
            user.UserName = model.Email;

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
           
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
                    Photo = model.Sex == Sex.Male ? _config.GetValue<string>("defaultmale") : _config.GetValue<string>("defaultfemale"),
                    Id = user.Id
                };

                await _unit.UserRepository.CreateAsync(appUser);

                await _unit.Commit();
            }

            return result;
        }

        public async Task<User> FindByIdUserAsync(int id)
        {
            return await _unit.UserRepository.GetAsync(id);
        }

        public async Task<bool> EmailExistAsync(CheckRegisterModel model)
        {
            return await _userManager.FindByEmailAsync(model.Email) == null;
        }

        public async Task<SignInResponce> AuthenticateAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (user == null||!isPasswordValid)
                throw new UserNotExistException("Given credentials not valid!!", 400);

            var identity = await _jwtHelper.GetIdentityAsync(model.Email);

            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            var now = DateTime.Now;

            var jwt = new JwtSecurityToken(
                    issuer: options.Issuer,
                    audience: options.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromSeconds(options.LifeTime)),
                    signingCredentials: new SigningCredentials(options.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new SignInResponce
            {
                Access_Token = encodedJwt,
                ExpiresIn = now.Add(TimeSpan.FromSeconds(options.LifeTime)),
                Refresh_Token = refreshToken
            };
        }

        public async Task<SignInResponce> ExchangeTokensAsync(ExchangeTokenRequest request)
        {
            var principal =_jwtHelper.GetPrincipalFromExpiredToken(request.AccessToken);

            var userName = principal.Identity.Name;

            var user = await this._userManager.FindByNameAsync(userName);

            if (user == null)
                throw new UserNotExistException("User not exist!!", 400);

            if (user.RefreshToken != request.RefreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userManager.UpdateAsync(user);

            var now = DateTime.Now;

            var jwt = new JwtSecurityToken(
                    issuer: options.Issuer,
                    audience: options.Audience,
                    notBefore: now,
                    claims: principal.Claims,
                    expires: now.Add(TimeSpan.FromSeconds(options.LifeTime)),
                    signingCredentials: new SigningCredentials(options.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new SignInResponce
            {
                Access_Token = encodedJwt,
                ExpiresIn = now.Add(TimeSpan.FromSeconds(options.LifeTime)),
                Refresh_Token = newRefreshToken
            };
        }
    }
}
