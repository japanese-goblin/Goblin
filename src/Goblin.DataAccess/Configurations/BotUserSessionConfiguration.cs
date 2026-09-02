using Goblin.Domain;
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
            .HasValueGenerator<IdValueGenerator>();

        builder.Property(p => p.FlowStepType)
            .HasMaxLength(100);
        builder.Property(p => p.FlowType)
            .HasConversion(new EnumMemberConverter<FlowType>());

        builder.HasOne(p => p.BotUser)
            .WithOne(p => p.Session)
            .HasForeignKey<BotUserSession>(p => p.BotUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
