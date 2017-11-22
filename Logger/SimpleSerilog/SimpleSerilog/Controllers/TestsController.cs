using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleSerilog.Extensions;

namespace SimpleSerilog.Controllers
{
    [Route("api/[controller]")]
    public class TestsController : Controller
    {
        private readonly ILogger<TestsController> _logger;

        public TestsController(ILogger<TestsController> logger)
        {
            _logger = logger;
        }

        // GET api/tests
        [HttpGet]
        public string Get()
        {
            var obj = new { foo = "Foo", bar = "Bar" };
            _logger.LogInformation("API LoggerTests 1 - {0}", obj.ToJson());

            string val = "foo bar";
            _logger.LogInformation($"API LoggerTests 2 - {val}");

            int id = 999;
            _logger.LogInformation("API LoggerTests 3 - #{ID}", id);

            return "Test log added, check it out in the `Logs` folder.";
        }
    }
}
