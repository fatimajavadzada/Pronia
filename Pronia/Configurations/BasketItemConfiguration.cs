using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations
{
    public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.Property(b => b.Count).IsRequired();

            builder.HasOne(b => b.Product)
                   .WithMany(u => u.BasketItems)
                   .HasForeignKey(b => b.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.AppUser)
                     .WithMany(u => u.BasketItems)
                     .HasForeignKey(b => b.AppUserId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(opt =>
            {
                opt.HasCheckConstraint("CK_BasketItems_Count", "[Count] >= 0");
            });
        }
    }
}
