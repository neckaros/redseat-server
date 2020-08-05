using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.IdentityModel.Tokens;

namespace RedSeatServer.Services
{
    public interface IFirebaseService
    {
        JwtSecurityToken VerifyToken(string idToken);
    }

    public class FirebaseService : IFirebaseService
    {
        public FirebaseService()
        {



        }

        public JwtSecurityToken VerifyToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();


            var token = new JwtSecurityToken(jwtEncodedString: idToken);

            
            var tokenString = handler.WriteToken(token);

            SecurityToken validatedToken;
            var param = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(1),
                ValidIssuer = "issuer",
                ValidAudience = "audience",
                // IssuerSigningKey = new SecurityKey() ,
            };
            var claims = handler.ValidateToken(tokenString, param, out validatedToken);
            


            Console.WriteLine("email => " + token.Claims.First(c => c.Type == "user_id").Value);
            return token;
        }

    }
}