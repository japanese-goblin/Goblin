using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations
{
    public class CronJobConfiguration : IEntityTypeConfiguration<CronJob>
    {
        public void Configure(EntityTypeBuilder<CronJob> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.VkId).IsRequired();
            builder.Property(x => x.NarfuGroup).HasDefaultValue(0);
            builder.Property(x => x.WeatherCity).HasDefaultValue(string.Empty);
            builder.Property(x => x.Hours).IsRequired();
            builder.Property(x => x.Minutes).IsRequired();
        }
    }
}