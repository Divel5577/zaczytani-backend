using MediatR;
using Microsoft.Extensions.Configuration;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record CommentCreatedCommand(Guid ReviewId) : IRequest
{
    private class CommentCreatedCommandHandler(IEmailInfoRepository emailInfoRepository, IReviewRepository reviewRepository, IConfiguration configuration) : IRequestHandler<CommentCreatedCommand>
    {
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IEmailInfoRepository _emailInfoRepository = emailInfoRepository;
        private readonly string _frontendUrl = configuration.GetSection("FrontendUrl").Value
            ?? throw new InvalidOperationException("Frontend URL is not configured. Please set 'FrontendUrl' in appsettings.json.");

        public async Task Handle(CommentCreatedCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken)
                ?? throw new NotFoundException("Review with given ID not found");

            if (review.User.Email is null)
                return;

            var emailInfo = new EmailInfo()
            {
                EmailTo = review.User.Email,
                EmailContent = [review.Book.Title, string.Format("{0}/review/{1}", _frontendUrl, review.Id)],
                EmailTemplate = EmailTemplate.CommentAdded,
            };

            await _emailInfoRepository.AddAsync(emailInfo, cancellationToken);
            await _emailInfoRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
