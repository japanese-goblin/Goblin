using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<BotUser>
    {
        public void Configure(EntityTypeBuilder<BotUser> builder)
        {
            builder.Property(b => b.Weather)
                   .HasDefaultValue(false)
                   .ValueGeneratedNever();

            builder.Property(b => b.Schedule)
                   .HasDefaultValue(false)
                   .ValueGeneratedNever();

            builder.Property(b => b.City)
                   .IsRequired(false);

            builder.Property(b => b.Group)
                   .HasDefaultValue(0);

            builder.Property(b => b.Schedule)
                   .HasDefaultValue(false);

            builder.Property(b => b.Weather)
                   .HasDefaultValue(false);
        }
    }
}