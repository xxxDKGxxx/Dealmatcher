using Dealmatcher.Backend.UseCases.Features.Activities;
using Dealmatcher.Backend.UseCases.Features.Activities.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Activities;

public class ActivityLoggingBehaviorTests
{
    private readonly IRepository<Activity> _activitiesRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IReadRepository<User> _usersRepository;
    private readonly IReadRepository<Offer> _offerRepository;
    private readonly ILogger<ActivityLoggingBehavior<TestLoggableCommand, Result<string>>> _loggerLoggable;
    private readonly ILogger<ActivityLoggingBehavior<TestNonLoggableCommand, Result<string>>> _loggerNonLoggable;

    public ActivityLoggingBehaviorTests()
    {
        _activitiesRepository = Substitute.For<IRepository<Activity>>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _offerRepository = Substitute.For<IReadRepository<Offer>>();
        _loggerLoggable = Substitute.For<ILogger<ActivityLoggingBehavior<TestLoggableCommand, Result<string>>>>();
        _loggerNonLoggable = Substitute.For<ILogger<ActivityLoggingBehavior<TestNonLoggableCommand, Result<string>>>>();

        SetupHttpContext("127.0.0.1");
    }

    private void SetupHttpContext(string ipAddress)
    {
        var httpContext = Substitute.For<HttpContext>();
        var connection = Substitute.For<ConnectionInfo>();
        connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse(ipAddress));
        httpContext.Connection.Returns(connection);
        _httpContextAccessor.HttpContext.Returns(httpContext);
    }

    private static User CreateUser(int id = 1)
    {
        var user = new User("user@example.com", "hash", "Test", "User") { Id = id };
        return user;
    }

    private static Offer CreateOffer(int id = 10)
    {
        var seller = new User("seller@example.com", "hash", "Seller", "User") { Id = 2 };
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []) { Id = id };
        return offer;
    }

    private ActivityLoggingBehavior<TestLoggableCommand, Result<string>> CreateLoggableBehavior()
    {
        return new ActivityLoggingBehavior<TestLoggableCommand, Result<string>>(
            _activitiesRepository,
            _httpContextAccessor,
            _usersRepository,
            _offerRepository,
            _loggerLoggable);
    }

    private ActivityLoggingBehavior<TestNonLoggableCommand, Result<string>> CreateNonLoggableBehavior()
    {
        return new ActivityLoggingBehavior<TestNonLoggableCommand, Result<string>>(
            _activitiesRepository,
            _httpContextAccessor,
            _usersRepository,
            _offerRepository,
            _loggerNonLoggable);
    }

    [Fact]
    public async Task Handle_SuccessfulLoggableCommand_CreatesActivity()
    {
        var behavior = CreateLoggableBehavior();
        var user = CreateUser(1);
        var offer = CreateOffer(10);

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        var command = new TestLoggableCommand(1, 10);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.Received(1).AddAsync(
            Arg.Is<Activity>(a =>
                a.User == user &&
                a.Offer == offer &&
                a.Action == ActivityAction.Create),
            Arg.Any<CancellationToken>());
        await _activitiesRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SuccessfulLoggableCommand_ReturnsOriginalResult()
    {
        var behavior = CreateLoggableBehavior();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateUser(1));
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(CreateOffer(10));

        var command = new TestLoggableCommand(1, 10);
        var expectedResult = Result.Success("test value");
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(expectedResult);

        var result = await behavior.Handle(command, next, CancellationToken.None);

        result.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task Handle_FailedResult_DoesNotCreateActivity()
    {
        var behavior = CreateLoggableBehavior();
        var command = new TestLoggableCommand(1, 10);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result<string>.Error("error"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonLoggableCommand_DoesNotCreateActivity()
    {
        var behavior = CreateNonLoggableBehavior();
        var command = new TestNonLoggableCommand();
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullUserId_DoesNotCreateActivity()
    {
        var behavior = CreateLoggableBehavior();
        var command = new TestLoggableCommand(null, 10);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_DoesNotCreateActivity()
    {
        var behavior = CreateLoggableBehavior();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((User?)null);

        var command = new TestLoggableCommand(1, 10);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullOfferId_CreatesActivityWithoutOffer()
    {
        var behavior = CreateLoggableBehavior();
        var user = CreateUser(1);
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var command = new TestLoggableCommand(1, null);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.Received(1).AddAsync(
            Arg.Is<Activity>(a => a.User == user && a.Offer == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoHttpContext_DoesNotCreateActivity()
    {
        var behavior = CreateLoggableBehavior();
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        var command = new TestLoggableCommand(1, 10);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullIpAddress_DoesNotCreateActivity()
    {
        var behavior = CreateLoggableBehavior();
        var httpContext = Substitute.For<HttpContext>();
        var connection = Substitute.For<ConnectionInfo>();
        connection.RemoteIpAddress.Returns((System.Net.IPAddress?)null);
        httpContext.Connection.Returns(connection);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        var command = new TestLoggableCommand(1, 10);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ReturnsOriginalResult()
    {
        var behavior = CreateLoggableBehavior();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateUser(1));
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(CreateOffer(10));
        _activitiesRepository.AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>())
            .Returns<Activity>(x => throw new Exception("DB error"));

        var command = new TestLoggableCommand(1, 10);
        var expectedResult = Result.Success("ok");
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(expectedResult);

        var result = await behavior.Handle(command, next, CancellationToken.None);

        result.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_DoesNotThrow()
    {
        var behavior = CreateLoggableBehavior();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateUser(1));
        _activitiesRepository.AddAsync(Arg.Any<Activity>(), Arg.Any<CancellationToken>())
            .Returns<Activity>(x => throw new Exception("DB error"));

        var command = new TestLoggableCommand(1, 10);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await Should.NotThrowAsync(() => behavior.Handle(command, next, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_HandlerExecutesBeforeLogging()
    {
        var behavior = CreateLoggableBehavior();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateUser(1));

        var handlerExecuted = false;
        var command = new TestLoggableCommand(1, null);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(callInfo =>
        {
            handlerExecuted = true;
            return Result.Success("ok");
        });

        await behavior.Handle(command, next, CancellationToken.None);

        handlerExecuted.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_CorrectDetailsPassedToActivity()
    {
        var behavior = CreateLoggableBehavior();
        var user = CreateUser(1);
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var command = new TestLoggableCommand(1, null);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.Received(1).AddAsync(
            Arg.Is<Activity>(a =>
                a.Details.ContainsKey("test") &&
                a.Details["test"] == "value"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CorrectIpAddressPassedToActivity()
    {
        var behavior = CreateLoggableBehavior();
        var user = CreateUser(1);
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        SetupHttpContext("192.168.1.100");

        var command = new TestLoggableCommand(1, null);
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next().Returns(Result.Success("ok"));

        await behavior.Handle(command, next, CancellationToken.None);

        await _activitiesRepository.Received(1).AddAsync(
            Arg.Is<Activity>(a =>
                a.IPAddress.ToString() == "192.168.1.100"),
            Arg.Any<CancellationToken>());
    }
}

// Test helpers

public sealed record TestLoggableCommand(int? UserId, int? OfferId)
    : IRequest<Result<string>>, ILoggableActivity<Result<string>>
{
    public ActivityAction Action => ActivityAction.Create;
    public int? GetUserId(Result<string> result) => UserId;
    public int? GetOfferId(Result<string> result) => OfferId;
    public Dictionary<string, string> GetDetails(Result<string> result) =>
        new() { ["test"] = "value" };
}

public sealed record TestNonLoggableCommand()
    : IRequest<Result<string>>;
