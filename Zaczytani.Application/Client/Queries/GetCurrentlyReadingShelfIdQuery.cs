using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;
public class GetCurrentlyReadingShelfIdQuery : IRequest<ReadingBookShelfIdDto>, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId) => UserId = userId;

    public class Handler(IBookShelfRepository bookShelfRepository) : IRequestHandler<GetCurrentlyReadingShelfIdQuery, ReadingBookShelfIdDto>
    {
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;

        public async Task<ReadingBookShelfIdDto> Handle(GetCurrentlyReadingShelfIdQuery request, CancellationToken cancellationToken)
        {
            var readingBookShelf = await _bookShelfRepository.GetBookShelfByTypeAsync(BookShelfType.Reading, request.UserId, cancellationToken)
                ?? throw new NotFoundException("Reading BookShelf not found");

            return new ReadingBookShelfIdDto
            {
                Id = readingBookShelf.Id
            };
        }
    }
}