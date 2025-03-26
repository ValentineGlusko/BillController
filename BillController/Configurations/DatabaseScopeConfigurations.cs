namespace BillController.Configurations
{
    public class DatabaseScopeConfigurations
    {
        public const string SectionName = "DatabaseScopeConfiguration";
        public string CommonOperations { get; set; }
        public string NotSuccessfulOperation { get; set; } 
    }
}
