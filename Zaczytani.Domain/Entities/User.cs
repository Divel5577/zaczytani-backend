
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zaczytani.Domain.Entities;
public class User : IdentityUser<Guid>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override Guid Id
    {
        get { return base.Id; }
        set { base.Id = value; }
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Image { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public ICollection<UserDrawnBook> UserDrawnBooks { get; set; } = [];
    public ICollection<BookShelf> BookShelves { get; set; } = [];
    public ICollection<Challenge> Challenges { get; set; } = [];
    public ICollection<ChallengeProgress> ChallengeProgresses { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Follower> Followers { get; set; } = [];
    public ICollection<Follower> Following { get; set; } = [];
    public ICollection<UserBadge> UserBadges { get; set; } = [];

}

public class UserRole : IdentityRole<Guid>
{
    public UserRole() : base() { }

    public UserRole(string roleName) : base(roleName)
    {
        Name = roleName;
    }
}