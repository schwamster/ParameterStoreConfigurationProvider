using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace example_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // GET Configuration/key
        [HttpGet("{key}")]
        public string Get(string key)
        {
            var value = configuration[key];
            Console.WriteLine($"configuration: {value}");
            return value ?? "***ValueWasNull***";
        }
    }
}
