using System.Web.Mvc;

namespace ContactMe.Controllers
{
    /// <summary>
    /// Controller for the contact me page.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Displays a generic error page.
        /// </summary>
        /// <returns>An error page.</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
