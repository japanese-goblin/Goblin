using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations;

public class RemindConfiguration : IEntityTypeConfiguration<Remind>
{
    public void Configure(EntityTypeBuilder<Remind> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
               .HasMaxLength(100)
               .IsRequired();
        builder.Property(x => x.Date)
               .IsRequired();
        builder.Property(x => x.ConsumerType)
               .HasDefaultValue(ConsumerType.Vkontakte);
    }
}