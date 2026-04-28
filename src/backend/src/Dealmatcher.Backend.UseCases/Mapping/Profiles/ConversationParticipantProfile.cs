namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class ConversationParticipantProfile : Profile
{
    public ConversationParticipantProfile()
    {
        CreateMap<User, ConversationParticipantDto>();
    }
}
