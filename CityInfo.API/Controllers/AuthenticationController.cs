using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public IConfiguration _configuration { get; }


        // we wont use this classes outside this class.
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }
        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string userName, string firstname, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstname;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            //step 1 :  validate the username/password

            var user = ValidateUserCredentials(authenticationRequestBody.Username, authenticationRequestBody.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            // step 2 :  create a token. Nb install Microsoft.IdentityModel.Tokens
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //the claims that we need
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            // generate a jwt token

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
              );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(tokenToReturn);  //return token
        }

        private CityInfoUser ValidateUserCredentials(string? username, string? password)
        {
            // we dont have the user db or table. If you have, just check the passed-through username/password against what's stored in the db
            //for demo purposes, we assume the credentials are valid
            //return a new CityInfoUser(values would normally come from user Db/Table)
            return new CityInfoUser(1, username ?? "", "Kevin", "comba", "Nyeri");
        }
    }
}
