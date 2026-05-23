namespace Dealmatcher.Backend.UseCases.Features.Admin.ListOffers;

public sealed class AdminListOffersQueryHandler(
    IReadRepository<User> usersRepository,
    IReadRepository<Offer> offersRepository,
    IMapper mapper) : IQueryHandler<AdminListOffersQuery, Result<AdminOffersPageDto>>
{
    public async Task<Result<AdminOffersPageDto>> Handle(AdminListOffersQuery request, CancellationToken cancellationToken)
    {
        var admin = await usersRepository.GetByIdAsync(request.AdminId, cancellationToken);

        if (admin is null)
        {
            return Result.Unauthorized($"Admin with id: {request.AdminId} doesn't exist");
        }

        if (!admin.IsPrivileged)
        {
            return Result.Forbidden($"User id: {request.AdminId} isn't privileged");
        }

        if (request.Limit <= 0)
        {
            return Result.Invalid(new ValidationError($"Invalid Limit value: {request.Limit}"));
        }

        if (request.Page <= 0)
        {
            return Result.Invalid(new ValidationError($"Invalid Page value: {request.Page}"));
        }

        OfferStatus status;
        try
        {
            status = OfferStatus.FromName(request.Status, true);
        }
        catch
        {
            return Result.Invalid(new ValidationError($"Offer status: {request.Status} is invalid"));
        }

        var pagedOffersByStatusSpec = new PagedOffersByStatusSpec(request.Page, request.Limit, status);
        var offers = await offersRepository.ListAsync(pagedOffersByStatusSpec, cancellationToken);

        var offersByStatusSpec = new OffersByStatusSpec(status);
        var total = await offersRepository.CountAsync(offersByStatusSpec, cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)request.Limit);
        var dtos = offers.Select(o => mapper.Map<OfferDto>(o)).ToList();

        return Result.Success(new AdminOffersPageDto(dtos, total, Math.Min(request.Page, totalPages), totalPages));
    }
}
