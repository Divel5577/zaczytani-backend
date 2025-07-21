using AutoMapper;
using MediatR;
using Zaczytani.Application.Client.Commands;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public class AcceptBookRequestCommand : IRequest
{
    private Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string? FileName { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = [];
    public string PublishingHouse { get; set; } = string.Empty;
    public List<BookGenre> Genre { get; set; } = [];
    public string? Series { get; set; } = string.Empty;

    public void SetId(Guid id)
    {
        Id = id;
    }

    private class AcceptBookRequestCommandHandler(IMediator mediator, IBookRequestRepository bookRequestRepository, IMapper mapper) : IRequestHandler<AcceptBookRequestCommand>
    {
        private readonly IMediator _mediator = mediator;
        private readonly IBookRequestRepository _bookRequestRepository = bookRequestRepository;
        private readonly IMapper _mapper = mapper;

        public async Task Handle(AcceptBookRequestCommand request, CancellationToken cancellationToken)
        {
            var bookRequest = await _bookRequestRepository.GetByIdAsync(request.Id, cancellationToken)
                 ?? throw new NotFoundException($"Book Request with ID {request.Id} was not found.");

            bookRequest.Status = BookRequestStatus.Accepted;
            await _bookRequestRepository.SaveChangesAsync(cancellationToken);

            var command = _mapper.Map<CreateBookCommand>(request);
            var bookId = await _mediator.Send(command, cancellationToken);
            
            var bookRequestAcceptedCommand = new BookRequestAcceptedCommand(request.Id, bookId);
            await _mediator.Send(bookRequestAcceptedCommand, cancellationToken);
        }
    }
}
