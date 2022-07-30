using Microsoft.AspNetCore.Mvc;

namespace SF.LvmBlog.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.StatusCode = 404;
                    ViewBag.ErrorMessage = "Ресурс не найден";
                    break;
                case 403:
                    ViewBag.StatusCode = 403;
                    ViewBag.ErrorMessage = "Доступ запрещен";
                    break;
                default:
                    ViewBag.StatusCode = statusCode;
                    ViewBag.ErrorMessage = "Неизвестная ошибка";
                    break ;
            }
            return View("~/Views/Shared/HttpError.cshtml");
        }
    }
}
