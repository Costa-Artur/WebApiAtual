using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Univali.Api.Models;

namespace Univali.Api.Controllers;
[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AuthenticationController (IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [HttpPost("authenticate")]
    public ActionResult<string> Authenticate (AuthenticationRequestDto authenticationRequestDto)
    {
        var user = ValidateUserCredentials(
            authenticationRequestDto.Username!,
            authenticationRequestDto.Password!
        );

        if(user == null)
        {
            return Unauthorized();
        }

        var securityKey = new SymmetricSecurityKey
        (
            Encoding.UTF8.GetBytes(
                _configuration["Authentication:SecretKey"]
                    ?? throw new ArgumentNullException(nameof(_configuration))
            )
        );

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>();
        claims.Add(new Claim("sub", user.UserId.ToString()));
        claims.Add(new Claim("given_name", user.Name));

        var jwt = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],

            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials
        );

        var jwtToReturn = new JwtSecurityTokenHandler().WriteToken(jwt);
        return Ok(jwtToReturn);
    }

    private InfoUser? ValidateUserCredentials (string userName, string password)
    {
        var userFromDatabase = new Entities.User
        {
            Id = 1,
            Name = "Ada Lovelace",
            UserName = "love",
            Password = "MinhaSenha"
        };

        if(userFromDatabase.UserName == userName && userFromDatabase.Password == password)
        {
            return new InfoUser(userFromDatabase.Id, userName, userFromDatabase.Name);
        }
        return null;
    }

    private class InfoUser 
    {
        public int UserId {get;set;}
        public string Name {get;set;} = string.Empty;
        public string UserName {get;set;} = string.Empty;

        public InfoUser(int userId, string userName, string name)
        {
            UserId = userId;
            UserName = userName;
            Name = name;
        }
    }

}
