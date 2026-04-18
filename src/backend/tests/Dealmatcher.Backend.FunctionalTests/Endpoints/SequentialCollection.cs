using Dealmatcher.Backend.FunctionalTests;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints;

[CollectionDefinition("Sequential", DisableParallelization = true)]
public class SequentialCollection : ICollectionFixture<CustomWebApplicationFactory> { }
