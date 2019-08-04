using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations
{
    public class SubscribeConfiguration : IEntityTypeConfiguration<Subscribe>
    {
        public void Configure(EntityTypeBuilder<Subscribe> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.BotUser)
                   .WithOne(x => x.SubscribeInfo);

            builder.Property(x => x.IsSchedule).HasDefaultValue(false);
            builder.Property(x => x.IsWeather).HasDefaultValue(false);
        }
    }
}