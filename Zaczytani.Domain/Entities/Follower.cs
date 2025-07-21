using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zaczytani.Domain.Entities;

public class Follower
{
    [Required] // Klucz główny (część składowa klucza złożonego)
    [ForeignKey(nameof(User))]
    public Guid FollowerId { get; set; }

    [Required] // Klucz główny (część składowa klucza złożonego)
    [ForeignKey(nameof(User))]
    public Guid FollowedId { get; set; }

    public virtual User FollowerUser { get; set; } = null!; // Użytkownik, który obserwuje

    public virtual User FollowedUser { get; set; } = null!; // Użytkownik, który jest obserwowany
}
