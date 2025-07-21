using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Queries;

public class GetAuthorsQuery : IRequest<IEnumerable<AuthorDto>>
{
    private class GetAuthorsQueryHandler(IAuthorRepository authorRepository, IMapper mapper) : IRequestHandler<GetAuthorsQuery, IEnumerable<AuthorDto>>
    {
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<IEnumerable<AuthorDto>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
        {
            var authors = await _authorRepository.GetAll().ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }
    }
}
