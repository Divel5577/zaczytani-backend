using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Entities;

public class BookShelf
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;


    public bool IsDefault { get; set; } = false;

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BookShelfType Type { get; set; } = BookShelfType.Other;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = [];

    public static BookShelf CreateCurrentlyReading(Guid userId)
    {
        return new BookShelf()
        {
            Name = "Aktualnie czytane",
            IsDefault = true,
            Type = BookShelfType.Reading,
            UserId = userId,
        };
    }

    public static BookShelf CreateRead(Guid userId)
    {
        return new BookShelf()
        {
            Name = "Przeczytane",
            IsDefault = true,
            Type = BookShelfType.Read,
            UserId = userId,
        };
    }

    public static BookShelf CreateToRead(Guid userId)
    {
        return new BookShelf()
        {
            Name = "Chce przeczytać",
            IsDefault = true,
            Type = BookShelfType.ToRead,
            UserId = userId,
        };
    }
}
