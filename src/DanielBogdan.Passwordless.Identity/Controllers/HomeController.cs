using System.Threading.Tasks;
using DanielBogdan.Passwordless.Identity.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DanielBogdan.Passwordless.Identity.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;

        public HomeController(IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment,
            ILogger<HomeController> logger)
        {
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (_environment.IsDevelopment())
            {
                // only show in development
                return View();
            }

            return NotFound();
        }

        [Route("error/{code:int?}")]
        public async Task<IActionResult> Error(int? code = null, string errorId = null)
        {
            var model = new ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code
            };

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                model.IdentityError = message;
            }

            if (code.HasValue)
            {
                switch (code.Value)
                {
                    //Add other custom views
                    case 401:
                        {
                            return View($"{code}", model);
                        }
                    case 404:
                        {
                            return View($"{code}", model);
                        }
                }
            }

            model.Title = $"Internal Server Error";
            model.Description = $"The server encountered something unexpected that didn't allow it to complete the request.";

            return View("Error", model);
        }
    }
}
