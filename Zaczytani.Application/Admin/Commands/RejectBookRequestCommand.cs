using AutoMapper;
using MediatR;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public class RejectBookRequestCommand : IRequest
{
    private Guid Id { get; set; }

    public void SetId(Guid id) => Id = id;

    private class RejectBookRequestCommandHandler(IBookRequestRepository bookRequestRepository) : IRequestHandler<RejectBookRequestCommand>
    {
        private readonly IBookRequestRepository _bookRequestRepository = bookRequestRepository;

        public async Task Handle(RejectBookRequestCommand request, CancellationToken cancellationToken)
        {
            var bookRequest = await _bookRequestRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Book Request with ID {request.Id} was not found.");

            bookRequest.Status = BookRequestStatus.Rejected;
            await _bookRequestRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
