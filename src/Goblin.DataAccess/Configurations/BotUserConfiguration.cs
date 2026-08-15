using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations;

internal class BotUserConfiguration : IEntityTypeConfiguration<BotUser>
{
    public void Configure(EntityTypeBuilder<BotUser> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasValueGenerator<IdValueGenerator>();

        builder.Property(p => p.WeatherCity)
            .HasMaxLength(100);

        builder.Property(p => p.IsAdmin)
            .HasDefaultValue(false);
        builder.Property(p => p.IsErrorsEnabled)
            .HasDefaultValue(true);
        builder.Property(p => p.HasScheduleSubscription)
            .HasDefaultValue(false);
        builder.Property(p => p.HasWeatherSubscription)
            .HasDefaultValue(false);

        builder.HasIndex(p => new { p.ConsumerType, p.ConsumerId });
    }
}