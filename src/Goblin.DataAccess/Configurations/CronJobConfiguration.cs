using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations;

internal class CronJobConfiguration : IEntityTypeConfiguration<CronJob>
{
    public void Configure(EntityTypeBuilder<CronJob> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasValueGenerator<IdValueGenerator>();

        builder.Property(p => p.ConsumerType)
            .HasConversion(new EnumMemberConverter<ConsumerType>());
        builder.Property(p => p.CronType)
            .HasConversion(new EnumMemberConverter<CronType>());

        builder.Property(p => p.Name)
               .IsRequired();
        builder.Property(p => p.ChatId)
               .IsRequired();
        builder.OwnsOne(p => p.Time);

        builder.Property(p => p.Text)
               .HasMaxLength(500);
    }
}