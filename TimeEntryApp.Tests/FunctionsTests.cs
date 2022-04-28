using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using TimeEntryApp.Tests.Infrastructure;
using Xunit;

namespace TimeEntryApp.Tests
{
    [Collection(TestsCollection.Name)]
    public class FunctionsTests
    {
        ILogger logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");

        TimeEntryFunction function;  

        public FunctionsTests(TestHost testHost)
        {
            function = new TimeEntryFunction(testHost.ServiceProvider.GetRequiredService<DataverseServiceClient>());
        }

        [Fact]
        public async void Http_trigger_should_return_ok()
        {
            // arrange
            var req = TestFactory.CreateHttpRequest(new TimeEntryDto
            {
                StartOn = new DateTime(2020, 01, 01),
                EndOn = new DateTime(2020, 01, 03)
            });

            // act
            var result = await function.Run(req, logger);

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Http_trigger_should_return_bad_request1()
        {
            // arrange
            var req = TestFactory.CreateHttpRequest(new TimeEntryDto
            {
                StartOn = new DateTime(2020, 01, 03),
                EndOn = new DateTime(2020, 01, 02)
            });

            // act
            var result = await function.Run(req, logger);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Http_trigger_should_return_bad_request2()
        {
            // arrange
            var req = TestFactory.CreateHttpRequest(null);

            // act
            var result = await function.Run(req, logger);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}