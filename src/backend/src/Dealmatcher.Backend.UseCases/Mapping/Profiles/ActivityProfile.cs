namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class ActivityProfile : Profile
{
    public ActivityProfile()
    {
        CreateMap<Activity, ActivityDto>()
            .ConstructUsing(src => new ActivityDto(
                src.Id,
                src.User.Id,
                src.Offer != null ? src.Offer.Id : null,
                src.Action.Name,
                src.Details.ToDictionary(),
                src.IPAddress.ToString(),
                src.CreatedAt));
    }
}
