using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Repos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ComplexCRUDApplication.Services
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly ILogger<RefreshHandler> _logger;
        private readonly DataContext _dataContext;
        public RefreshHandler(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomNumber = new Byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create()) 
            {
                randomNumberGenerator.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);
                var existToken = await _dataContext.TblRefreshtokens.FirstOrDefaultAsync(r => r.Userid == username);
                if (existToken != null) 
                {
                    existToken.Refreshtoken = refreshToken;
                }
                else
                {
                    await _dataContext.TblRefreshtokens.AddAsync(new TblRefreshtoken()
                    {
                        Userid = username,
                        Tokenid = new Random().Next().ToString(),
                        Refreshtoken = refreshToken
                    });
                }

                _dataContext.SaveChanges();
                return refreshToken;
            }
        }
    }
}
