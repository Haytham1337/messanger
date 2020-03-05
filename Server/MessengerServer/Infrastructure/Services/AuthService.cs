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
using Microsoft.AspNetCore.Mvc;

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

    public class AuthService:IAuthService
    {
        private readonly UserManager<SecurityUser> _userManager;
                       
        private readonly IUnitOfWork _unit;

        private readonly IConfiguration _config;

        private readonly JWToptions options;

        public AuthService(UserManager<SecurityUser> userManager, IOptions<JWToptions> options,
            IUnitOfWork unit,IConfiguration config)
        {
            _userManager = userManager;

            _unit = unit;

            _config = config;

            this.options = options.Value;       
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
                    Photo= model.Sex == Sex.Male ? _config.GetValue<string>("defaultmale") : _config.GetValue<string>("defaultfemale"),
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
            var user = await this._userManager.FindByNameAsync(model.Email);

            if (user == null)
                throw new UserNotExistException("User with the given email not exist!!", 400);

            var identity =await this.GetIdentityAsync(model);

            var refreshToken = this.GenerateRefreshToken();

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

            return new SignInResponce { Access_Token=encodedJwt,ExpiresIn= now.Add(TimeSpan.FromSeconds(options.LifeTime)),
                Refresh_Token=refreshToken};
        }

        public async Task<SignInResponce> ExchangeTokensAsync(ExchangeTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);

            var userName = principal.Identity.Name;

            var user = await this._userManager.FindByNameAsync(userName);

            if (user == null)
                throw new UserNotExistException("User not exist!!", 400);

            if (user.RefreshToken != request.RefreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newRefreshToken = this.GenerateRefreshToken();

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

        private async Task<ClaimsIdentity> GetIdentityAsync(LoginModel model)
        {
            var user = await this._userManager.FindByNameAsync(model.Email);

            var isSasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (isSasswordValid)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType,user.Email),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
                };

                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            throw new UserAlreadyExistException("Password is invalid", 400);
        }

        private string GenerateRefreshToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer= options.Issuer,
                ValidAudience= options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = options.GetSymmetricSecurityKey(),
                ValidateLifetime = false 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
           
            SecurityToken securityToken;
            
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
