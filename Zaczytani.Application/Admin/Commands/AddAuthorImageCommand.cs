using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public class AddAuthorImageCommand : IRequest<Guid>, IUserIdAssignable
{
    public Guid AuthorId { get; set; }
    public string FileName { get; set; } = string.Empty;

    private Guid UserId { get; set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    private class AddAuthorPhotoCommandHandler(IAuthorRepository authorRepository) : IRequestHandler<AddAuthorImageCommand, Guid>
    {
        private readonly IAuthorRepository _authorRepository = authorRepository;

        public async Task<Guid> Handle(AddAuthorImageCommand request, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetByIdAsync(request.AuthorId);

            if (author == null)
            {
                throw new NotFoundException($"Author with ID {request.AuthorId} not found.");
            }

            author.Image = request.FileName;

            await _authorRepository.SaveChangesAsync(cancellationToken);

            return author.Id;
        }
    }
}




