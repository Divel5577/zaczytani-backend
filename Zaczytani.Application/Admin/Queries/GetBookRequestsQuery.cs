using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Queries;

public class GetBookRequestsQuery : IRequest<IEnumerable<BookRequestDto>>
{
    private class GetBookRequestQueryHandler(IBookRequestRepository bookRequestRepository, IFileStorageRepository fileStorageRepository, IMapper mapper) : IRequestHandler<GetBookRequestsQuery, IEnumerable<BookRequestDto>>
    {
        private readonly IBookRequestRepository _bookRequestRepository = bookRequestRepository;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BookRequestDto>> Handle(GetBookRequestsQuery request, CancellationToken cancellationToken)
        {
            var bookRequests = await _bookRequestRepository.GetAllPending()
                .Include(b => b.User)
                .ToListAsync(cancellationToken);

            var bookRequestDtos = _mapper.Map<IEnumerable<BookRequestDto>>(bookRequests);
            bookRequestDtos.ToList().ForEach(b => b.ImageUrl = _fileStorageRepository.GetFileUrl(b.FileName));

            return bookRequestDtos;
        }
    }
}
