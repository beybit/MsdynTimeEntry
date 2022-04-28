using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TimeEntryApp;

namespace TimeEntryApp.Tests
{
    public class TestFactory
    {
        public static TimeEntryDto Data()
        {
            return new TimeEntryDto
            {
                StartOn = new DateTime(2020, 01, 01),
                EndOn = new DateTime(2020, 01, 03)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(TimeEntryDto timeEntry)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(JsonConvert.SerializeObject(timeEntry));
            writer.Flush();
            stream.Position = 0;
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = stream
            };
            return request;
        }
    }
}
