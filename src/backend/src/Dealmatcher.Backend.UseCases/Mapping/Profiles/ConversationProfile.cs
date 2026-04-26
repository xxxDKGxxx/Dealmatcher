namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>()
          .ForCtorParam(
            nameof(ConversationDto.LastMessage),
            opt => opt.MapFrom(c => c.LastMessage.Content)
          )
          .ForCtorParam(
            nameof(ConversationDto.LastMessageAt),
            opt => opt.MapFrom(c => c.LastMessage.CreatedAt)
          )
          .ForCtorParam(nameof(ConversationDto.Status), opt => opt.MapFrom(c => c.Status.Name));
        CreateMap<Conversation, ConversationDetailDto>()
          .ForCtorParam(
            nameof(ConversationDetailDto.LastMessage),
            opt => opt.MapFrom(c => c.LastMessage.Content)
          )
          .ForCtorParam(
            nameof(ConversationDetailDto.LastMessageAt),
            opt => opt.MapFrom(c => c.LastMessage.CreatedAt)
          )
          .ForCtorParam(nameof(ConversationDetailDto.Status), opt => opt.MapFrom(c => c.Status.Name));
    }
}
