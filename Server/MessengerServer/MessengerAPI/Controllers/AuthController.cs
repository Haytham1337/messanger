using System.Threading.Tasks;
using Application.Models;
using Infrastructure.AppSecurity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        private readonly UserManager<SecurityUser> _userManager;
        public AuthController(IAuthService auth,UserManager<SecurityUser> userManager)
        {
            _auth = auth;

            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _auth.AuthenticateAsync(model));
            }

            return BadRequest("Model is not valid!!");
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _auth.RegisterAsync(model);

                if (result.Succeeded)
                    return Ok("Success!!");
                else
                    return BadRequest("Register denied!");
            }

            return BadRequest("Model is not valid!");
        }

        [HttpPost]
        public async  Task<bool> EmailExist([FromBody]CheckRegisterModel model)
        {
            return await this._auth.EmailExistAsync(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<SignInResponce> ExchangeTokens([FromBody]ExchangeTokenRequest request)
        {
            return await _auth.ExchangeTokensAsync(request);
        }
    }
}