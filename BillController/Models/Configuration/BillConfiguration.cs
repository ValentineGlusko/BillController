using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillController.Models.Configuration
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(e => e.BillId);
            builder.HasOne(e => e.Category)
                .WithMany(e => e.Bills)
                .HasForeignKey(f => f.CategoryId);
            builder.Property(e => e.Amount).HasPrecision(10, 2);
            builder.Property(e => e.Version).IsRowVersion();
            
        }
    }
}
