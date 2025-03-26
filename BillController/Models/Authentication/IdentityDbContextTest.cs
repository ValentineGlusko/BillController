using Microsoft.EntityFrameworkCore;

namespace BillController.Models.Authentication
{
    public class IdentityDbContextTest : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<AppUser>
    {
        public IdentityDbContextTest(DbContextOptions<IdentityDbContextTest> options) : base(options) { }
    }
}
