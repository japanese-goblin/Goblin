using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations;

public class TgBotUserConfiguration : IEntityTypeConfiguration<TgBotUser>
{
       public void Configure(EntityTypeBuilder<TgBotUser> builder)
       {
              builder.HasKey(x => x.Id);
              builder.Property(x => x.Id)
                     .ValueGeneratedNever();

              builder.Property(x => x.WeatherCity)
                     .HasMaxLength(100)
                     .HasDefaultValue(string.Empty);
              builder.Property(x => x.NarfuGroup)
                     .HasDefaultValue(0);

              builder.Property(x => x.IsAdmin)
                     .HasDefaultValue(false);
              builder.Property(x => x.IsErrorsEnabled)
                     .HasDefaultValue(true);
              builder.Property(x => x.HasScheduleSubscription)
                     .HasDefaultValue(false);
              builder.Property(x => x.HasWeatherSubscription)
                     .HasDefaultValue(false);
       }
}