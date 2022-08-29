using Microsoft.AspNetCore.Mvc;

namespace SF.LvmBlog.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ErrorController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            ViewBag.StatusCode = statusCode;
            switch (statusCode)
            {
                case 400:
                    ViewBag.ErrorMessage = "Плохой запрос.";
                    break;
                case 401:
                    ViewBag.ErrorMessage = "Неавторизованно.";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Доступ запрещен.";
                    break;
                case 404:
                    ViewBag.ErrorMessage = "Ресурс не найден.";
                    break;
                case 405:
                    ViewBag.ErrorMessage = "Метод не разрешён";
                    break;
                default:
                    ViewBag.ErrorMessage = "Неизвестная ошибка HTTP";
                    break ;
            }
            _logger.LogError($"[{HttpContext.User.Identity.Name}] {ViewBag.StatusCode} - {ViewBag.ErrorMessage}");
            return View("~/Views/Shared/HttpError.cshtml");
        }
    }
}
