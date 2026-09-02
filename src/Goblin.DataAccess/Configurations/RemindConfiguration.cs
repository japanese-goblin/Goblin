using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations;

internal class RemindConfiguration : IEntityTypeConfiguration<Remind>
{
    public void Configure(EntityTypeBuilder<Remind> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasValueGenerator<IdValueGenerator>();

        builder.Property(p => p.Text)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(p => p.Date)
            .IsRequired();

        builder.HasOne(p => p.BotUser)
            .WithMany(p => p.Reminds)
            .HasForeignKey(p => p.BotUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
