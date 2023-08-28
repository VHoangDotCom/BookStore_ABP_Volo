using CloudinaryTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudinaryTest.DAL
{
    public class CloudDBContext : DbContext
    {
        public CloudDBContext(DbContextOptions<CloudDBContext> options) : base(options)
        {

        }

        public DbSet<User> Students { get; set; }
        public DbSet<CloudFolder> CloudFolders { get; set; }
        public DbSet<CloudFile> CloudFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CloudFile>()
                 .HasKey(e => e.FolderId);
            modelBuilder.Entity<CloudFile>()
                .HasOne(p => p.CloudFolder)
                .WithMany(pc => pc.CloudFiles)
                .HasForeignKey(p => p.FolderId);
        }
    }
}
