using AutoMapper;
using Moq;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Shared.Queries;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.UnitTests.Handlers;

public class GetBookDetailsQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock = new();
    private readonly Mock<IBookShelfRepository> _bookShelfRepositoryMock = new();
    private readonly Mock<IFileStorageRepository> _fileStorageRepositoryMock = new();
    private readonly IMapper _mapper;

    public GetBookDetailsQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, BookDto>()
           .ForMember(x => x.PublishingHouse, opt => opt.MapFrom(src => src.PublishingHouse.Name))
           .ForMember(x => x.Rating, opt => opt.MapFrom(src => src.Reviews.Where(r => r.Rating != null).Average(r => r.Rating)))
           .ForMember(x => x.RatingCount, opt => opt.MapFrom(src => src.Reviews.Where(r => r.Rating != null).Count()))
           .ForMember(x => x.Reviews, opt => opt.MapFrom(src => src.Reviews.Where(r => r.Content != null).Count()));

            cfg.CreateMap<Author, AuthorDto>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedBookDto()
    {
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Testowa Książka",
            Isbn = "1234567890123",
            Description = "Przykładowy opis",
            PageNumber = 123,
            ReleaseDate = new DateOnly(2023, 1, 1),
            Image = "cover.jpg",
            Genre = [BookGenre.Fantasy],
            Series = "Testowa seria",
            PublishingHouse = new PublishingHouse { Name = "Test Wydawnictwo" },
            Authors = [new Author { Name = "Author A" }],
            Reviews = [
                new Review { Rating = 4, Content = "Good book" },
                new Review { Rating = 5, Content = "Great book" }
            ]
        };

        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        _fileStorageRepositoryMock.Setup(r => r.GetFileUrl("cover.jpg"))
            .Returns("http://filestorage.com/cover.jpg");
        _bookShelfRepositoryMock.Setup(r => r.GetBookCountOnReadShelfAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        var handler = new GetBookDetailsQuery.GetBookDetailsQueryHandler(
            _bookRepositoryMock.Object,
            _bookShelfRepositoryMock.Object,
            _fileStorageRepositoryMock.Object,
            _mapper);

        var query = new GetBookDetailsQuery(bookId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(book.Title, result.Title);
        Assert.Equal(book.Isbn, result.Isbn);
        Assert.Equal("http://filestorage.com/cover.jpg", result.ImageUrl);
        Assert.Equal(book.Authors[0].Name, result.Authors.First().Name);
        Assert.Equal(book.PublishingHouse.Name, result.PublishingHouse);
        Assert.Equal(4.5, result.Rating);
        Assert.Equal(2, result.RatingCount);
        Assert.Equal(2, result.Reviews);
        Assert.Equal(10, result.Readers);
    }
}
