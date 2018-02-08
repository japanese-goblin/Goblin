using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Goblin
{
    public partial class d9o30apvvh50ejContext : DbContext
    {
        public virtual DbSet<Persons> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"***REMOVED***");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persons>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.VkId)
                    .HasColumnName("VkID")
                    .HasDefaultValueSql("0");
            });
        }
    }
}
