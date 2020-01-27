﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SignIn(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _auth.SignIn(model);

                if (result.Succeeded)
                    return Ok(Response.Headers["set-cookie"]);
                else
                    return BadRequest("Sign in denied!!");
            }

            return BadRequest("Model is not valid!!");
        }

        [HttpGet("[action]")]
        public async Task SignOut()
        {
             await _auth.SignOut();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if(await _auth.FindByNameAsync(model.Email) == null)
                {
                    var result = await _auth.Register(model);

                    if (result.Succeeded)
                        return Ok(Response.Headers["set-cookie"]);
                    else
                        return BadRequest("Register denied!");
                }
            }

            return BadRequest("Model is not valid!");
        }

    }
}