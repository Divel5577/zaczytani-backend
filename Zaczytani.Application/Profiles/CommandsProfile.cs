using AutoMapper;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Application.Client.Commands;
using Zaczytani.Domain.Entities;

namespace Zaczytani.Application.Profiles;

internal class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<CreateBookRequestCommand, BookRequest>()
            .ForMember(x => x.Image, opt => opt.MapFrom(src => src.FileName));

        CreateMap<AcceptBookRequestCommand, CreateBookCommand>();

        CreateMap<CreateReviewCommand, Review>();

        CreateMap<ReportUserCommand, Report>();

        CreateMap<CreateCommentCommand, Comment>();

        CreateMap<CreateChallengeCommand, Challenge>();
    }
}
