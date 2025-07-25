﻿

using System.Net;
using Newtonsoft.Json;

namespace SchoolPilot.Business.BasicResults
{
    public interface IActionResult
    {
        [JsonIgnore]
        HttpStatusCode Status { get; set; }

        [JsonIgnore]
        string ErrorMessage { get; set; }
    }
}
