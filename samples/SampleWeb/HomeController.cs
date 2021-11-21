using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleWeb
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly ILogger _logger2;
        public HomeController(ILogger<HomeController> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _logger2 = loggerFactory.CreateLogger("Microsoft.XXX");
        }

        [Route("/")]
        public IActionResult Index()
        {
            _logger.LogDebug("LogDebug");
            _logger.LogInformation("LogInformation");
            _logger.LogWarning("LogWarning");
            _logger.LogError("LogError");
            _logger.LogCritical("LogCritical");

            _logger2.LogDebug("LogDebug");
            _logger2.LogInformation("LogInformation");
            _logger2.LogWarning("LogWarning");
            _logger2.LogError("LogError");
            _logger2.LogCritical("LogCritical");
            return Content("OK");
        }

        [Route("/error")]
        public IActionResult Error()
        {
            _logger.LogDebug("LogDebug");
            _logger.LogInformation("LogInformation");
            _logger.LogWarning("LogWarning");
            _logger.LogError("LogError");
            _logger.LogCritical("LogCritical");


            _logger2.LogDebug("LogDebug");
            _logger2.LogInformation("LogInformation");
            _logger2.LogWarning("LogWarning");
            _logger2.LogError("LogError");
            _logger2.LogCritical("LogCritical");

            int a = 10, b = 0;
            try
            {
                _logger.LogDebug("Dividing {A} by {B}", a, b);
                Console.WriteLine(a / b);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong");
                _logger2.LogError(ex, "Something went wrong");
                throw;
            }

            return Content("OK");
        }
    }
}
