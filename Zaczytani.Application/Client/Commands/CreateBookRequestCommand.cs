using AutoMapper;
using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public class CreateBookRequestCommand : IRequest<Guid>, IUserIdAssignable
{
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int? PageNumber { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? FileName { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string? PublishingHouse { get; set; } = string.Empty;
    public List<BookGenre> Genre { get; set; } = [];
    public string? Series { get; set; } = string.Empty;
    private Guid UserId { get; set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    private class CreateBookRequestCommandHandler(IBookRequestRepository bookRequestRepository, IMapper mapper) : IRequestHandler<CreateBookRequestCommand, Guid>
    {
        private readonly IBookRequestRepository _bookRequestRepository = bookRequestRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<Guid> Handle(CreateBookRequestCommand request, CancellationToken cancellationToken)
        {
            var bookRequest = _mapper.Map<BookRequest>(request);
            bookRequest.UserId = request.UserId;

            await _bookRequestRepository.AddAsync(bookRequest);
            await _bookRequestRepository.SaveChangesAsync(cancellationToken);

            return bookRequest.Id;
        }
    }
}
