using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations;

internal class BotUserSessionConfiguration : IEntityTypeConfiguration<BotUserSession>
{
    public void Configure(EntityTypeBuilder<BotUserSession> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.HasOne(p => p.BotUser)
            .WithOne(p => p.Session)
            .HasForeignKey<BotUser>(p => p.Id);

        builder.Property(p => p.FlowStepType)
            .HasMaxLength(100);
    }
}