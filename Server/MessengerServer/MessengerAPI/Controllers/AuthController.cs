using System.Threading.Tasks;
using Application.Models;
using Application.Models.AuthModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IProvidersAuthService _providerAuth;
        private readonly IHostingEnvironment _env;

        public AuthController(IAuthService auth, IProvidersAuthService providerAuth, IHostingEnvironment env)
        {
            _auth = auth;

            _providerAuth = providerAuth;

            _env = env;
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(LoginModel model)
        {
            var signInResponce = await _auth.AuthenticateAsync(model);

            return Ok(signInResponce);
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            var result = await _auth.RegisterAsync(model);

            if (result.Succeeded)
                return Ok("Success!!");
            else
                return BadRequest("Register denied!");
        }

        [HttpPost]
        public async Task<bool> EmailExist([FromBody]CheckRegisterModel model)
        {
            return await this._auth.EmailExistAsync(model);
        }

        [HttpPost]
        public async Task<SignInResponce> ExchangeTokens([FromBody]ExchangeTokenRequest request)
        {
            return await _auth.ExchangeTokensAsync(request);
        }

        [HttpPost]
        public async Task<IActionResult> FacebookAuthenticate([FromBody]FacebookAuthRequest model)
        {
            var responce = await this._providerAuth.FacebookAuthenticateAsync(model);

            return Ok(responce);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userName, string code)
        {
            var result = await _auth.ConfirmEmailAsync(userName, code);

            if(result.Succeeded)
                return Redirect("https://localhost:44334/htmlresponces/emailConfirmed.html");

            return Redirect("https://localhost:44334/htmlresponces/emailNotConfirmed.html");
        }
    }
}