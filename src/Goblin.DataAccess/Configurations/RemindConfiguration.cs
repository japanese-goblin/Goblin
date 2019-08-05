using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Goblin.DataAccess.Configurations
{
    public class RemindConfiguration : IEntityTypeConfiguration<Remind>
    {
        public void Configure(EntityTypeBuilder<Remind> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.BotUser)
                   .WithMany(x => x.Reminds)
                   .HasForeignKey(x => x.BotUserId);
            
            builder.Property(x => x.Text).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Date).IsRequired();
        }
    }
}