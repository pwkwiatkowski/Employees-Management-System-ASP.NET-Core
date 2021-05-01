using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektZaliczeniowy.Controllers
{
    public class ErrorController : Controller
    {
        //wielojezykowosc
        private readonly IStringLocalizer<ErrorController> _localizer;
        
        public ErrorController(IStringLocalizer<ErrorController> localizer)
        {
            _localizer = localizer;
        }

        //[Route("Error/404")]
        [Route("Error/{statusCode}")] // {} - placeholder
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = _localizer["Error 404: Page not found"];
                    break;
                case 403:
                    ViewBag.ErrorMessage = _localizer["Error 403: Access denied"];
                    break;
                case 500:
                    ViewBag.ErrorMessage = _localizer["Error 500: Internal server error"];
                    break;

            }

            return View("NotFound");
        }
    }
}
