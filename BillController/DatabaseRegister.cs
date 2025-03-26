using BillController.Models;
using Microsoft.EntityFrameworkCore;

namespace BillController
{
    public static class DatabaseRegister
    {
        public static IServiceProvider UseSomething(this IServiceProvider serv)
        {
            var o = serv.CreateScope();

            var service = o.ServiceProvider.GetRequiredService<DataContext>();
            using (o)
            {
                
               service.Database.Migrate();
            }

            return serv;
        }
    }
}
