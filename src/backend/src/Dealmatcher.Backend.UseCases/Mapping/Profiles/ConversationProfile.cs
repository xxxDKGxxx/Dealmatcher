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
          .ForCtorParam(nameof(ConversationDto.Status), opt => opt.MapFrom(c => c.Status.Name))
          .ForCtorParam(
            nameof(ConversationDto.UnreadCount),
            opt =>
              opt.MapFrom(
                (src, context) =>
                {
                    if (context.Items.TryGetValue("readerId", out var readerId))
                    {
                        return src.UnreadCount((int)readerId);
                    }
                    return -1;
                }
              )
          );
        CreateMap<Conversation, ConversationDetailDto>()
          .ForCtorParam(
            nameof(ConversationDetailDto.LastMessage),
            opt => opt.MapFrom(c => c.LastMessage.Content)
          )
          .ForCtorParam(
            nameof(ConversationDetailDto.LastMessageAt),
            opt => opt.MapFrom(c => c.LastMessage.CreatedAt)
          )
          .ForCtorParam(nameof(ConversationDetailDto.Status), opt => opt.MapFrom(c => c.Status.Name))
          .ForCtorParam(
            nameof(ConversationDetailDto.UnreadCount),
            opt =>
              opt.MapFrom(
                (src, context) =>
                {
                    if (context.Items.TryGetValue("readerId", out var readerId))
                    {
                        return src.UnreadCount((int)readerId);
                    }
                    return -1;
                }
              )
          );
    }
}
