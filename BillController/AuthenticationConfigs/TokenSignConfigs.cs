namespace BillController.AuthenticationConfigs
{
    public class TokenSignConfigs
    {
        public const string SectionName = "JwtConfig";
        public   string ValidAudiences { get; set; }
        public   string ValidIssuer { get; set; }
        public   string Secret { get; set; }
    }
}
