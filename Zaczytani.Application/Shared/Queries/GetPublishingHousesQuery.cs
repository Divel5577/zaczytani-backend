using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Queries;

public class GetPublishingHousesQuery : IRequest<IEnumerable<PublishingHouseDto>>
{
    private class GetAuthorsQueryHandler(IPublishingHouseRepository publishingHouseRepository, IMapper mapper) : IRequestHandler<GetPublishingHousesQuery, IEnumerable<PublishingHouseDto>>
    {
        private readonly IPublishingHouseRepository _publishingHouseRepository = publishingHouseRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<IEnumerable<PublishingHouseDto>> Handle(GetPublishingHousesQuery request, CancellationToken cancellationToken)
        {
            var houses = await _publishingHouseRepository.GetAll().ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<PublishingHouseDto>>(houses);
        }
    }
}
