namespace BillController.Services.Abstraction
{
    public interface IAccountService
    {
        public string GenerateToken(string username);
    }
}
