namespace Dealmatcher.Backend.UseCases.Features.Activities.Interfaces;

public interface ILoggableActivity<TResult> where TResult : Ardalis.Result.IResult
{
    ActivityAction Action { get; }
    int? GetUserId(TResult result);
    int? GetOfferId(TResult result);
    Dictionary<string, string> GetDetails(TResult result);
}
