using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;

namespace Zaczytani.Infrastructure.Persistance;

internal class BookDbContext(DbContextOptions options) : IdentityDbContext<User, UserRole, Guid>(options)
{
    internal DbSet<Book> Books { get; set; } = null!;
    internal DbSet<BookRequest> BookRequests { get; set; } = null!;
    internal DbSet<Author> Authors { get; set; } = null!;
    internal DbSet<PublishingHouse> PublishingHouses { get; set; } = null!;
    internal DbSet<UserDrawnBook> UserDrawnBook { get; set; } = null!;
    internal DbSet<BookShelf> BookShelves { get; set; } = null!;
    internal DbSet<Challenge> Challenges { get; set; } = null!;
    internal DbSet<ChallengeProgress> ChallengeProgresses { get; set; } = null!;
    internal DbSet<Comment> Comments { get; set; } = null!;
    internal DbSet<Review> Reviews { get; set; } = null!;
    internal DbSet<Report> Reports { get; set; } = null!;
    internal DbSet<Follower> Followers { get; set; } = null!;
    internal DbSet<EmailInfo> EmailInfo { get; set; } = null!;
    internal DbSet<Badge> Badges { get; set; } = null!;
    internal DbSet<UserBadge> UserBadges { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookRequest>()
            .Property(b => b.Status)
            .HasConversion<int>();

        modelBuilder.Entity<UserDrawnBook>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.UserDrawnBooks)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserDrawnBook>()
            .HasOne(ub => ub.Book)
            .WithMany()
            .HasForeignKey(ub => ub.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookShelf>()
            .HasOne(bs => bs.User)
            .WithMany(u => u.BookShelves)
            .HasForeignKey(bs => bs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookShelf>()
            .HasMany(bs => bs.Books)
            .WithMany();

        modelBuilder.Entity<Challenge>()
            .HasMany(c => c.UserProgress)
            .WithOne(up => up.Challenge)
            .HasForeignKey(u => u.ChallengeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Review)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Review)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(f => new { f.FollowerId, f.FollowedId });

            entity.HasOne(f => f.FollowerUser)
                  .WithMany(u => u.Following)
                  .HasForeignKey(f => f.FollowerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.FollowedUser)
                  .WithMany(u => u.Followers)
                  .HasForeignKey(f => f.FollowedId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}   
