using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Constants;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Seeders;

internal class Seeder(BookDbContext dbContext, IPasswordHasher<User> passwordHasher) : ISeeder
{
    private readonly BookDbContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task Seed()
    {
        if (_dbContext.Database.CanConnect())
        {
            if (!_dbContext.Roles.Any())
            {
                var roles = GetRoles();
                _dbContext.Roles.AddRange(roles);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.Users.Any())
            {
                var users = GetUsers();
                await _dbContext.Users.AddRangeAsync(users);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.UserRoles.Any())
            {
                var userRoles = await GetUserRoles();
                await _dbContext.UserRoles.AddRangeAsync(userRoles);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.Authors.Any())
            {
                var authors = GetAuthors();
                await _dbContext.Authors.AddRangeAsync(authors);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.Books.Any())
            {
                var books = GetBooks();
                await _dbContext.Books.AddRangeAsync(books);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.BookRequests.Any())
            {
                var bookRequests = GetBookRequests();
                await _dbContext.BookRequests.AddRangeAsync(bookRequests);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.BookShelves.Any())
            {
                var bookShelves = GetDefaultBookShelves();
                await _dbContext.BookShelves.AddRangeAsync(bookShelves);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.Badges.Any())
            {
                var badges = GetDefaultBadges();
                _dbContext.Badges.AddRange(badges);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task<IEnumerable<IdentityUserRole<Guid>>> GetUserRoles()
    {
        var roles = await _dbContext.Roles.ToListAsync();
        var users = await _dbContext.Users.ToListAsync();

        List<IdentityUserRole<Guid>> userRoles = [
            new()
            {
                RoleId = roles.First(r => r.Name == UserRoles.Admin).Id,
                UserId = users.First(u => u.Email == "admin@email.com").Id
            },
            new()
            {
                RoleId = roles.First(r => r.Name == UserRoles.User).Id,
                UserId = users.First(u => u.Email == "user@email.com").Id
            }
            ];

        return userRoles;
    }

    private IEnumerable<User> GetUsers()
    {
        var admin = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = "Admin",
            Email = "admin@email.com",
            NormalizedEmail = "admin@email.com".ToUpper(),
            UserName = "admin",
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            NormalizedUserName = "admin".ToUpper(),
            EmailConfirmed = true,
        };

        admin.PasswordHash = _passwordHasher.HashPassword(admin, "password");

        var user = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = "User",
            Email = "user@email.com",
            NormalizedEmail = "user@email.com".ToUpper(),
            UserName = "user",
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            NormalizedUserName = "user".ToUpper(),
            EmailConfirmed = true,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, "password");

        return [admin, user];
    }

    private static List<UserRole> GetRoles()
    {
        List<UserRole> roles = [
            new(UserRoles.User)
            {
                Id = Guid.NewGuid(),
                NormalizedName = UserRoles.User,
            },
            new(UserRoles.Admin)
            {
                Id = Guid.NewGuid(),
                NormalizedName = UserRoles.Admin,
            }
            ];

        return roles;
    }


    private static List<Author> GetAuthors()
    {
        List<Author> authors = [
            new()
            {
                Id = Guid.NewGuid(),
                Name = "J.K. Rowling"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Władysław Reymont"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "J.R.R. Tolkien"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Michaił Bułhakow"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Gabriel García Márquez"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "George Orwell"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Stephen King"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Herman Melville"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "F. Scott Fitzgerald"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Fiodor Dostojewski"
            }
            ];

        return authors;
    }

    private List<Book> GetBooks()
    {
        List<Book> books = [
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter i Kamień Filozoficzny",
                Description = "Pierwszy tom cyklu \"Harry Potter\" w poważnej, \"dorosłej\" okładce. Harry Potter, sierota i podrzutek, od niemowlęcia wychowywany był przez ciotkę i wuja, którzy traktowali go jak piąte koło u wozu. Pochodzenie chłopca owiane jest tajemnicą; jedyną pamiątką Harry`ego z przeszłości jest zagadkowa blizna na czole. Skąd jednak biorą się niesamowite zjawiska, które towarzyszą nieświadomemu niczego Potterowi? Harry nigdy by się nie spodziewał, że można latać na miotle, znać bardzo pożyteczne zaklęcia i nosić pelerynę niewidkę. Nigdy też nie przyszłoby mu do głowy, że to właśnie on stoczy walkę z potężnym i złym Lordem Voldermortem.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("J.K"))],
                Genre = [BookGenre.Romance],
                Isbn = "1231231231",
                PageNumber = 10,
                ReleaseDate = new DateOnly(2010, 1, 1),
                Series = "Harry Potter",
                PublishingHouse = new PublishingHouse()
                {
                    Id = Guid.NewGuid(),
                    Name = "Media Rodzina"
                },
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter i Komnata Tajemnic",
                Description = "Drugi tom bestsellerowego cyklu w poważnej, \"dorosłej\" okładce.\r\nTym razem Harry będzie musiał się zmierzyć z tajemniczym potworem z komnaty tajemnic na zamku Hogwart. Otworzyć tę komnatę mógł jedynie prawowity dziedzic Slytherina, a wskutek nieszczęśliwego zbiegu okoliczności podejrzenie pada na Harryego. W dodatku jeden z najbliższych przyjaciół bohatera znajduje się w śmiertelnym niebezpieczeństwie. Z tego tomu dowiecie się, jaka była przeszłość Hagrida, jakie sekrety skrywa rodzina Malfoya i kto naprawdę wypuścił potwora.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("J.K"))],
                Genre= [BookGenre.Romance, BookGenre.Memoir],
                Isbn = "9871231231",
                PageNumber = 10,
                ReleaseDate = new DateOnly(2010, 1, 1),
                Series = "Harry Potter",
                PublishingHouse = new PublishingHouse()
                {
                    Id = Guid.NewGuid(),
                    Name = "Bloomsbury"
                },
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Władca Pierścieni: Drużyna Pierścienia",
                Description = "Pierwszy tom trylogii 'Władca Pierścieni', w którym młody hobbit Frodo Baggins staje przed zadaniem zniszczenia Pierścienia Władzy. Razem ze swoją drużyną wyrusza na niebezpieczną wyprawę, której celem jest zniszczenie pierścienia w ogniu Góry Przeznaczenia.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("J.R.R. Tolkien"))],
                Genre = [BookGenre.Fantasy, BookGenre.Adventure],
                Isbn = "2345678901",
                PageNumber = 423,
                ReleaseDate = new DateOnly(2001, 7, 1),
                Series = "Władca Pierścieni",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Allen & Unwin"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Ziemia Obiecana",
                Description = "Powieść Władysława Reymonta, która opowiada o ciężkiej pracy robotników w przemysłowym Łodzi na przełomie XIX i XX wieku. W książce przedstawiono brutalne realia życia społecznego i ekonomicznego, a także ambicje i dążenia ludzi do sukcesu.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("Władysław Reymont"))],
                Genre = [BookGenre.Drama],
                Isbn = "3456789012",
                PageNumber = 510,
                ReleaseDate = new DateOnly(1905, 1, 1),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Zysk i S-ka"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "1984",
                Description = "Klasyczna powieść George'a Orwella, która przedstawia totalitarne społeczeństwo, w którym wszyscy są pod stałą obserwacją. Powieść bada tematykę władzy, kontroli i wolności jednostki.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("George Orwell"))],
                Genre = [BookGenre.Biography, BookGenre.History],
                Isbn = "4567890123",
                PageNumber = 328,
                ReleaseDate = new DateOnly(1949, 6, 8),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Secker & Warburg"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Sto lat samotności",
                Description = "Magiczna powieść Gabriela Garcíi Márqueza, która opowiada historię rodziny Buendía w fikcyjnym miasteczku Macondo. Łączy realizm magiczny z historią i kulturą Kolumbii.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("Gabriel García Márquez"))],
                Genre = [BookGenre.History],
                Isbn = "5678901234",
                PageNumber = 417,
                ReleaseDate = new DateOnly(1967, 5, 30),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Harper & Row"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Mistrz i Małgorzata",
                Description = "Powieść Michaiła Bułhakowa, która łączy elementy realizmu i fantastyki, opowiadając historię szatana, który przybywa do ZSRR, by ukarać władze, a także historię miłości Mistrza i Małgorzaty.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("Michaił Bułhakow"))],
                Genre = [BookGenre.Fantasy, BookGenre.HistoricalFiction],
                Isbn = "6789012345",
                PageNumber = 384,
                ReleaseDate = new DateOnly(1966, 1, 1),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Izd-vo Khudozhestvennaya Literatura"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Zbrodnia i kara",
                Description = "Powieść Fiodora Dostojewskiego opowiada o losach Rodiona Raskolnikowa, młodego studenta, który popełnia morderstwo, przekonany, że jego czyn jest moralnie usprawiedliwiony. Powieść bada tematykę winy, kary i odkupienia.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("Fiodor Dostojewski"))],
                Genre = [BookGenre.Psychology, BookGenre.History],
                Isbn = "7890123456",
                PageNumber = 430,
                ReleaseDate = new DateOnly(1866, 1, 1),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Izd-vo Dostojewskiego"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Wielki Gatsby",
                Description = "Powieść F. Scotta Fitzgeralda, która ukazuje obraz amerykańskiego społeczeństwa lat 20-tych XX wieku, skoncentrowany na tytułowym Gatsby'm, bogatym, ale samotnym mężczyźnie dążącym do odzyskania miłości z przeszłości.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("F. Scott Fitzgerald"))],
                Genre = [BookGenre.Business, BookGenre.Romance],
                Isbn = "8901234567",
                PageNumber = 180,
                ReleaseDate = new DateOnly(1925, 4, 10),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Scribner"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Moby Dick",
                Description = "Powieść Hermana Melville'a, która opowiada historię Kapitana Ahaba i jego obsesji na punkcie złapania legendarnego białego wieloryba, Moby Dicka. Jest to historia pełna symboliki i refleksji na temat ludzkiej natury.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("Herman Melville"))],
                Genre = [BookGenre.Psychology, BookGenre.History],
                Isbn = "9012345678",
                PageNumber = 635,
                ReleaseDate = new DateOnly(1851, 10, 18),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Harper & Brothers"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Zielona Mila",
                Description = "Powieść Stephena Kinga, która opowiada historię strażników więziennych oraz skazanych, którzy zostali umieszczeni na Oddziale ówcześnie tzw. 'zielonej mili'. Historia pełna emocji, tajemnic i niewyjaśnionych zdarzeń.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("Stephen King"))],
                Genre = [BookGenre.Horror, BookGenre.Drama],
                Isbn = "1234567890",
                PageNumber = 432,
                ReleaseDate = new DateOnly(1996, 12, 1),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Viking Penguin"
                }
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Rok 1984",
                Description = "Kolejna dystopia George'a Orwella, przedstawiająca totalitarne państwo, które kontroluje każdy aspekt życia swoich obywateli. Powieść dotyka tematów takich jak inwigilacja, cenzura i manipulacja prawdą.",
                Authors = [_dbContext.Authors.FirstOrDefault(a => a.Name.Contains("George Orwell"))],
                Genre = [BookGenre.History, BookGenre.Politics],
                Isbn = "2345678901",
                PageNumber = 328,
                ReleaseDate = new DateOnly(1949, 6, 8),
                Series = "Brak",
                PublishingHouse = new PublishingHouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Secker & Warburg"
                }
            }
            ];

        return books;

    }
    private List<BookRequest> GetBookRequests()
    {
        List<BookRequest> bookRequests = [
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter i Kamień Filozoficzny",
                Description = "Pierwszy tom cyklu \"Harry Potter\" w poważnej, \"dorosłej\" okładce. Harry Potter, sierota i podrzutek, od niemowlęcia wychowywany był przez ciotkę i wuja, którzy traktowali go jak piąte koło u wozu. Pochodzenie chłopca owiane jest tajemnicą; jedyną pamiątką Harry`ego z przeszłości jest zagadkowa blizna na czole. Skąd jednak biorą się niesamowite zjawiska, które towarzyszą nieświadomemu niczego Potterowi? Harry nigdy by się nie spodziewał, że można latać na miotle, znać bardzo pożyteczne zaklęcia i nosić pelerynę niewidkę. Nigdy też nie przyszłoby mu do głowy, że to właśnie on stoczy walkę z potężnym i złym Lordem Voldermortem.",
                Authors = "J. K. Rowling",
                Genre = [BookGenre.Romance],
                Isbn = "1231231231",
                PageNumber = 10,
                ReleaseDate = new DateOnly(2010, 1, 1),
                Series = "Harry Potter",
                PublishingHouse = "Media Rodzina",
                User = _dbContext.Users.FirstOrDefault(u => u.Email == "user@email.com")!,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter i Komnata Tajemnic",
                Description = "Drugi tom bestsellerowego cyklu w poważnej, \"dorosłej\" okładce.\r\nTym razem Harry będzie musiał się zmierzyć z tajemniczym potworem z komnaty tajemnic na zamku Hogwart. Otworzyć tę komnatę mógł jedynie prawowity dziedzic Slytherina, a wskutek nieszczęśliwego zbiegu okoliczności podejrzenie pada na Harryego. W dodatku jeden z najbliższych przyjaciół bohatera znajduje się w śmiertelnym niebezpieczeństwie. Z tego tomu dowiecie się, jaka była przeszłość Hagrida, jakie sekrety skrywa rodzina Malfoya i kto naprawdę wypuścił potwora.",
                Authors = "Nie wiem",
                Genre= [BookGenre.Romance, BookGenre.Memoir],
                ReleaseDate = new DateOnly(2010, 1, 1),
                Series = "Harry Potter",
                User = _dbContext.Users.FirstOrDefault(u => u.Email == "user@email.com")!,
            }
            ];

        return bookRequests;
    }

    private IEnumerable<BookShelf> GetDefaultBookShelves()
    {
        return
        [
            new BookShelf
            {
                Id = Guid.NewGuid(),
                Name = "Przeczytane",
                UserId = _dbContext.Users.FirstOrDefault(u => u.Email == "user@email.com")!.Id,
                Type = BookShelfType.Read,
                IsDefault = true,

            },
            new BookShelf
            {
                Id = Guid.NewGuid(),
                Name = "Chce przeczytać",
                UserId = _dbContext.Users.FirstOrDefault(u => u.Email == "user@email.com")!.Id,
                Type = BookShelfType.ToRead,
                IsDefault = true,

            },
            new BookShelf
            {
                Id = Guid.NewGuid(),
                Name = "Aktualnie czytane",
                UserId = _dbContext.Users.FirstOrDefault(u => u.Email == "user@email.com")!.Id,
                Type = BookShelfType.Reading,
                IsDefault = true
            }
        ];
    }
    private static IEnumerable<Badge> GetDefaultBadges()
    {
        return
        [
            new Badge { Id = Guid.NewGuid(), Type = BadgeType.FirstBook, Name = "Pierwsza Lektura", IconPath = "first_book.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.TenBooks, Name = "Lektor", IconPath = "ten_books.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.FiftyBooks, Name = "Bibliomaniak", IconPath = "fifty_books.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.HundredBooks, Name = "Książkowy Maratończyk", IconPath = "hundred_books.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.TwoHundredBooks, Name = "Mistrz Biblioteki", IconPath = "two_hundred_books.png" },

        new Badge { Id = Guid.NewGuid(), Type = BadgeType.Newbie, Name = "Nowicjusz", IconPath = "newbie.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.RegularReader, Name = "Stały Czytelnik", IconPath = "regular_reader.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.Veteran, Name = "Weteran", IconPath = "veteran.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.LegendaryReader, Name = "Kultowy Czytelnik", IconPath = "legendary_reader.png" },

        new Badge { Id = Guid.NewGuid(), Type = BadgeType.FirstReview, Name = "Pierwsza Recenzja", IconPath = "first_review.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.Reviewer, Name = "Recenzent", IconPath = "reviewer.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.LiteraryCritic, Name = "Krytyk Literacki", IconPath = "literary_critic.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.ReviewExpert, Name = "Ekspert Recenzji", IconPath = "review_expert.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.ReviewMaster, Name = "Mistrz Recenzji", IconPath = "review_master.png" },

        new Badge { Id = Guid.NewGuid(), Type = BadgeType.FirstComment, Name = "Pierwszy Komentarz", IconPath = "first_comment.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.Commentator, Name = "Komentator", IconPath = "commentator.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.ActiveCommentator, Name = "Aktywny Komentator", IconPath = "active_commentator.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.Debater, Name = "Dyskutant", IconPath = "debater.png" },
        new Badge { Id = Guid.NewGuid(), Type = BadgeType.DiscussionMaster, Name = "Mistrz Dyskusji", IconPath = "discussion_master.png" }
        ];
    }


}
