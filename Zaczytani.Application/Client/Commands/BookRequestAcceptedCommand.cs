using MediatR;
using Microsoft.Extensions.Configuration;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record BookRequestAcceptedCommand(Guid Id, Guid BookId) :IRequest
{
    private class BookRequestAcceptedCommandHandler(IEmailInfoRepository emailInfoRepository,IBookRequestRepository bookRequestRepository,IBookRepository bookRepository,IConfiguration configuration):IRequestHandler<BookRequestAcceptedCommand>
    {
        private readonly IBookRequestRepository _bookRequestRepository=bookRequestRepository;
        private readonly IBookRepository _bookRepository=bookRepository;
        private readonly IEmailInfoRepository _emailInfoRepository = emailInfoRepository;
        private readonly string _frontendUrl = configuration.GetSection("FrontendUrl").Value
           ?? throw new InvalidOperationException("Frontend URL is not configured. Please set 'FrontendUrl' in appsettings.json.");
        public async Task Handle(BookRequestAcceptedCommand request,CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken)
              ?? throw new NotFoundException($"Book with ID {request.BookId} was not found.");
            var bookRequest = await _bookRequestRepository.GetByIdAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException($"Book Request with ID {request.Id} was not found.");
            if(bookRequest.User.Email is null)
            {
                return;
            }

            var emailInfo = new EmailInfo()
            {
                EmailTo = bookRequest.User.Email,
                EmailContent = [book.Title, string.Format("{0}/books/{1}", _frontendUrl, book.Id)],
                EmailTemplate = EmailTemplate.BookRequestConfirmed,
            };
            await _emailInfoRepository.AddAsync(emailInfo, cancellationToken);
            await _emailInfoRepository.SaveChangesAsync(cancellationToken);

        }
    }
}
