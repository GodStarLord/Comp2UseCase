using Microsoft.EntityFrameworkCore;

namespace TweetAPP.Models
{
    public class TweetDBContext : DbContext
    {

        public TweetDBContext()
        {

        }

        public TweetDBContext(DbContextOptions<TweetDBContext> options)
            : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Tweet> Tweets { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        // Specify DbSet properties etc
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Specify DbSet properties etc

            // User Table
            modelBuilder.Entity<User>(entity => {
                entity.ToTable("tbl_tweetapp_users");
                entity.HasKey(x => x.UserId);
                entity.Property(x => x.FirstName).IsRequired();
                entity.Property(x => x.ContactNumber).IsRequired();
                entity.Property(x => x.Username).IsRequired();
                entity.Property(x => x.EmailId).IsRequired();
                entity.Property(x => x.Password).IsRequired();
            });

            // Comment Table
            modelBuilder.Entity<Comment>(entity => {
                entity.ToTable("tbl_tweetapp_comments");
                entity.HasKey(x => x.Id);
            });

            // Tweet Table
            modelBuilder.Entity<Tweet>(entity => {
                entity.ToTable("tbl_tweetapp_tweets");
                entity.HasKey(x => x.Id);
            });
        }
    }
}
