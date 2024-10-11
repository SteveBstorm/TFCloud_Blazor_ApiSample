using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TFCloud_Blazor_ApiSample.Models.DTOs;
using TFCloud_Blazor_ApiSample.Repos;
using TFCloud_Blazor_ApiSample.Tools;

namespace TFCloud_Blazor_ApiSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepo userRepo;
        private readonly JwtGenerator jwt;

        public UserController(UserRepo userRepo, JwtGenerator _jwt)
        {
            this.userRepo = userRepo;
            this.jwt = _jwt;
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody]RegisterForm form)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            string hashpwd = BCrypt.Net.BCrypt.HashPassword(form.Password);

            if(userRepo.Register(form.Email, hashpwd, form.Nickname))
            {
                return Ok("Inscription réussie");
            }
            return BadRequest("t'as du merder");
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody]LoginForm loginForm)
        {
            if (!ModelState.IsValid) return BadRequest();

            string dbpwd = userRepo.GetPassword(loginForm.Email);

            if(BCrypt.Net.BCrypt.Verify(loginForm.Password, dbpwd)) 
            {
                string token = jwt.GenerateToken(userRepo.Login(loginForm.Email, dbpwd));
                return Ok(token);
            }
            return BadRequest("Mot de passe invalide");
        }
        [Authorize("adminRequired")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(userRepo.GetAll());
        }

        [Authorize("userRequired")]
        [HttpGet("profile")]
        public IActionResult GetUserInfo()
        {
            string tokenFromRequest = HttpContext.Request.Headers["Authorization"];
            string token = tokenFromRequest.Substring(7, tokenFromRequest.Length - 7);
            JwtSecurityToken jwt = new JwtSecurityToken(token);
            string email = jwt.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            return Ok(userRepo.GetProfileByMail(email));
        }
    }
}
