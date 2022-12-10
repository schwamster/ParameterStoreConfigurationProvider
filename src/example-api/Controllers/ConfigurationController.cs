using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace example_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET Configuration/key
        [HttpGet("{key}")]
        public string Get(string key)
        {
            var value = _configuration[key];
            Console.WriteLine($"configuration: {value}");
            return value ?? "***ValueWasNull***";
        }
    }
}
