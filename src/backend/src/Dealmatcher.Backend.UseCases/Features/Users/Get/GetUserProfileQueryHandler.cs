namespace Dealmatcher.Backend.UseCases.Features.Users.Get;

public class GetUserProfileQueryHandler(
    IReadRepository<User> userRepository,
    IMapper mapper)
    : IQueryHandler<GetUserProfileQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserByIdSpec(request.UserId);
        var user = await userRepository.SingleOrDefaultAsync(spec, cancellationToken);

        if (user is null)
        {
            return Result.NotFound();
        }

        var userDto = mapper.Map<UserDto>(user);

        return Result.Success(userDto);
    }
}
