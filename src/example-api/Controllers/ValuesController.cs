using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace example_api.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        private readonly IConfiguration configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // GET api/values/5
        [HttpGet("{key}")]
        public string Get(string key)
        {
            var value = configuration[key];
            Console.WriteLine($"configuration: {value}");
            return value;
        }
    }
}
