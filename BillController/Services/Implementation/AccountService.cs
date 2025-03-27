using BillController.AuthenticationConfigs;
using BillController.Services.Abstraction;
using Microsoft.Extensions.Options;

namespace BillController.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly ITokenService _tokenService;

        public AccountService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        public string GenerateToken(string login)
        {
            return _tokenService.GenerateToken(login);
        }
    }
}
