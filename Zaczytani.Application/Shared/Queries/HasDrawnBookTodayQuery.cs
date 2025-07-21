using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Queries;

public record HasDrawnBookTodayQuery : IRequest<BookDto>, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IUserDrawnBookRepository userDrawnBookRepository, IFileStorageRepository fileStorageRepository, IMapper mapper) : IRequestHandler<HasDrawnBookTodayQuery, BookDto>
    {
        private readonly IUserDrawnBookRepository _userDrawnBookRepository = userDrawnBookRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;

        public async Task<BookDto> Handle(HasDrawnBookTodayQuery request, CancellationToken cancellationToken)
        {
            var drawnBook = await _userDrawnBookRepository.GetDrawnBookByUserIdAndDateAsync(request.UserId, DateTime.UtcNow.Date, cancellationToken)
                ?? throw new NotFoundException("User did not draw a book today");

            var bookDto = _mapper.Map<BookDto>(drawnBook.Book);
            bookDto.ImageUrl = _fileStorageRepository.GetFileUrl(drawnBook.Book.Image);

            return bookDto;
        }
    }
}
