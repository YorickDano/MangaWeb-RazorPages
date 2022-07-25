using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;
using static MangaWeb.Areas.Identity.Pages.Account.LoginModel;

namespace MangaWeb.Areas.Identity.Data
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<MangaWebUser> userManager;

        public TokenController(UserManager<MangaWebUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] InputModel inputModel)
        {
            var user = await userManager.FindByNameAsync(inputModel.UserName);
            if (user == null || await userManager.CheckPasswordAsync(user, inputModel.Password))
            {
                return Unauthorized();
            }

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Solodki lox"));
            var tokenDescroptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHendler = new JwtSecurityTokenHandler();
            var token = tokenHendler.CreateToken(tokenDescroptor);

            return Ok(new
            {
                token = tokenHendler.WriteToken(token),
                expires = token.ValidTo
            });
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("allManga")]
        public async Task<IActionResult> GetManga()
        {
            var client = new HttpClient();

            var respounce = await client.GetAsync("https://localhost:7083/allManga");
            var data = await respounce.Content.ReadAsStringAsync();

            return Ok(data);
        }
    }
}
