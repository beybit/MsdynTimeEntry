using Xunit;

namespace TimeEntryApp.Tests.Infrastructure
{
    [CollectionDefinition(Name)]
    public class TestsCollection : ICollectionFixture<TestHost>
    {
        public const string Name = nameof(TestsCollection);
    }
}
