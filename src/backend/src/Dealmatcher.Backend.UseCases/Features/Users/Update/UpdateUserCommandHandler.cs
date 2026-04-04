namespace Dealmatcher.Backend.UseCases.Features.Users.Update;

public class UpdateUserCommandHandler(
    IRepository<User> userRepository,
    IMapper mapper)
    : ICommandHandler<UpdateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound();
        }

        user.UpdateName(request.Name);
        user.UpdateSurname(request.Surname);

        await userRepository.UpdateAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var userDto = mapper.Map<UserDto>(user);

        return Result.Success(userDto);
    }
}
