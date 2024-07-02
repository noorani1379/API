using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using API.Models.Services;
using API.Models.Entities;
using API.Models.Dto;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        private readonly IConfiguration configuration;
        private readonly UserRepository userRepository;
        private readonly UserTokenRepository userTokenRepository;


        public AccountsController(IConfiguration configuration
            , UserRepository userRepository
            , UserTokenRepository userTokenRepository
            )
        {
            this.configuration = configuration;
            this.userRepository = userRepository;
            this.userTokenRepository = userTokenRepository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] RequestLoginDto request)
        {
            var loginResult = userRepository.Login(request.PhoneNumber, request.SmsCode);
            if (loginResult.IsSuccess == false)
            {
                return Ok(new LoginResultDto()
                {
                    IsSuccess = false,
                    Message = loginResult.Message
                });
            }
            var token = CreateToken(loginResult.User);

            return Ok(new LoginResultDto()
            {
                IsSuccess = true,
                Data = token,
            });
        }


       

        [Route("GetSmsCode")]
        [HttpGet]
        public IActionResult GetSmsCode(string PhoneNumber)
        {
            var smsCode = userRepository.GetCode(PhoneNumber);
            //smsCode پیامک کنید به همین شماره
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            var user = User.Claims.First(p => p.Type == "UserId").Value;
            userRepository.Logout(Guid.Parse(user));
            return Ok();
        }


        private LoginDataDto CreateToken(User user)
        {
            SecurityHelper securityHelper = new SecurityHelper();


            var claims = new List<Claim>
                {
                    new Claim ("UserId", user.Id.ToString()),
                    new Claim ("Name",  user?.Name??""),
                };
            string key = configuration["JWtConfig:Key"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenexp = DateTime.Now.AddMinutes(int.Parse(configuration["JWtConfig:expires"]));
            var token = new JwtSecurityToken(
                issuer: configuration["JWtConfig:issuer"],
                audience: configuration["JWtConfig:audience"],
                expires: tokenexp,
                notBefore: DateTime.Now,
                claims: claims,
                signingCredentials: credentials
                );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            

            return new LoginDataDto()
            {
                Token = jwtToken,
               
            };


        }
    }
}
