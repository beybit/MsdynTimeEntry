using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TimeEntryApp
{
    public class TimeEntryDto
    {
        [JsonProperty(Required = Newtonsoft.Json.Required.Always)]
        public DateTime StartOn { get; set; }

        [JsonProperty(Required = Newtonsoft.Json.Required.Always)]
        public DateTime EndOn { get; set; }
    }
}
