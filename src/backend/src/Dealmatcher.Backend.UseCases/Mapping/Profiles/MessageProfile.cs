namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageDto>()
          .ForCtorParam(nameof(MessageDto.SenderId), opt => opt.MapFrom(m => m.Sender.Id));
    }
}
