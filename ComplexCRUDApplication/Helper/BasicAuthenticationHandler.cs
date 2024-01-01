using ComplexCRUDApplication.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace ComplexCRUDApplication.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DataContext _dbContext;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, DataContext dataContext) : base(options, logger, encoder, clock)
        {
            _dbContext = dataContext;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check the authorization header exists or not
            if (!Request.Headers.ContainsKey("Authorization")) 
            {
                return AuthenticateResult.Fail("No header found.");
            }
            var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (headerValue != null)
            {
                var bytes = Convert.FromBase64String(headerValue.Parameter);
                string credentials = Encoding.UTF8.GetString(bytes);
                string[] credentialArray = credentials.Split(':');
                string usernname = credentialArray[0];
                string password = credentialArray[1];
                var user = await _dbContext.TblUsers.FirstOrDefaultAsync(c => c.Code == usernname && c.Password == password);

                if (user != null)
                {
                    var claim = new[] { new Claim(ClaimTypes.Name, user.Code) };
                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("No user found.");
                }
            }
            else 
            {
                return AuthenticateResult.Fail("Empty header.");
            }
        }
    }
}
