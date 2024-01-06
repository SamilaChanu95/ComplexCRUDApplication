using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Repos;
using ComplexCRUDApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ComplexCRUDApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        private readonly IRefreshHandler _refresh;

        public AuthorizationController(DataContext  dataContext, ILogger<AuthorizationController> logger, IOptions<JwtSettings> _jwtOptions, IConfiguration configuration, IRefreshHandler refreshHandler)
        {
            _dataContext = dataContext;
            _logger = logger;
            _jwtSettings = _jwtOptions.Value;
            _configuration = configuration;
            _refresh = refreshHandler;
        }

        [HttpPost("generate-refresh-token")]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] TokenResponse tokenResponse) 
        {
            var refreshToken = await _dataContext.TblRefreshtokens.FirstOrDefaultAsync(r => r.Refreshtoken == tokenResponse.RefreshToken);
            if (refreshToken != null)
            {
                // Generate the token by JwtSecurity key
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey);
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(tokenResponse.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateAudience = false
                }, out securityToken);

                var token = securityToken as JwtSecurityToken;

                if (token != null && token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string username = principal.Identity?.Name;
                    var exists = await _dataContext.TblRefreshtokens.FirstOrDefaultAsync(r => r.Userid == username && r.Refreshtoken == tokenResponse.RefreshToken);
                    if (exists != null)
                    {
                        var jwtTokenNew = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey)), SecurityAlgorithms.HmacSha256)
                        );

                        var finalToken = tokenHandler.WriteToken(jwtTokenNew);
                        return Ok(new TokenResponse() { Token = finalToken, RefreshToken = await _refresh.GenerateToken(username) });
                    }
                    else 
                    {
                        return Unauthorized();
                    }

                }
                else 
                {
                    return Unauthorized();
                }                
            }
            else 
            {
                return Unauthorized();
            }
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCredential userCredential)
        {
            var user = await _dataContext.TblUsers.FirstOrDefaultAsync(r => r.Code == userCredential.Username && r.Password == userCredential.Password);
            if (user != null)
            {
                // Generate the token by JwtSecurity key
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = _jwtSettings.SecurityKey;
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey);
                // var securityKeyClone = _configuration["JwtSettings:SecurityKey"];
                // var securityKeyClone1 = _configuration.GetSection("JwtSettings:SecurityKey").Value;
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, user.Code),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var finalToken = tokenHandler.WriteToken(token);
                return Ok(new TokenResponse() { Token = finalToken, RefreshToken = await _refresh.GenerateToken(userCredential.Username) });
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
