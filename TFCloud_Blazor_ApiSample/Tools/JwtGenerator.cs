using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TFCloud_Blazor_ApiSample.Models;

namespace TFCloud_Blazor_ApiSample.Tools
{
    public class JwtGenerator
    {
        public static readonly string secretKey = 
            "lEs fR@mB0isI3rs S0nT Perchés SuR le T@b0ur3t D3 mOn Grand-Père";

        public string GenerateToken(User u)
        {
            //Génération de la signin key
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            //Génération du payload (Body)
            Claim[] myclaims = new Claim[]
            {
                new Claim(ClaimTypes.Email, u.Email),
                new Claim(ClaimTypes.Role, u.IsAdmin ? "Admin" : "User"),
                new Claim("Nickname", u.Nickname)
            };

            //Génération du token

            JwtSecurityToken token = new JwtSecurityToken(
                claims: myclaims,
                signingCredentials: credentials,
                expires: DateTime.Now.AddHours(1),
                issuer : "monapi.com"
                );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }
    }
}
