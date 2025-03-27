namespace BillController.Services.Abstraction
{
    public interface ITokenService
    {
        public string GenerateToken(string username);
    }
}
