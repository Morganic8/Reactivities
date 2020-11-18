using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security {
    public class JwtGenerator : IJwtGenerator {
        private readonly SymmetricSecurityKey _key;
        public JwtGenerator(IConfiguration config) {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user) {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            //generate signing credentials
            //server validates token without query DB

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //Generate Token object
            var tokenDescriptor = new SecurityTokenDescriptor {

                //adds all claims
                Subject = new ClaimsIdentity(claims),
                //When token expires - 7 days
                Expires = DateTime.Now.AddDays(7),

                //add credentials
                SigningCredentials = creds
            };

            //Create handler to generate the usuable token
            var tokenHandler = new JwtSecurityTokenHandler();

            //Create usuable token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //return usuable token
            return tokenHandler.WriteToken(token);
        }
    }
}