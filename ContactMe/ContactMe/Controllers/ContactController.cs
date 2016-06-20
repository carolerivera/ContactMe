using BotDetect.Web;
using ContactMe.DataAccess.Models;
using ContactMe.DataAccess.Repositories;
using ContactMe.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace ContactMe.Controllers
{
    /// <summary>
    /// Controller for the contact me page.
    /// </summary>
    public class ContactController : Controller
    {
        public const string DefaultErrorMessage = "An error occurred saving your information, please try again.";
        public const string CaptchaErrorMessage = "Invalid Captcha code.";

        public IContactRepository ContactRepository;

        public ContactController()
        {
            ContactRepository = new ContactRepository();
        }

        /// <summary>
        /// Displays the contact me page.
        /// </summary>
        /// <returns>An index page.</returns>
        public ActionResult Index()
        {
            return View(new ContactModel());
        }

        /// <summary>
        /// Validates and saves a user's contact information sent via AJAX.
        /// </summary>
        /// <returns>A JsonResult indicating success/failure and containing appropriate messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(ContactModel model)
        {
            // The empty result. Indicates success without errors messages.
            var result = new ResponseModel();

            // If the ModelState is invalid, return the error messages.
            if (!ModelState.IsValid)
            {
                result.Errors.AddRange(ModelState.Values.SelectMany(v => v.Errors.Select(x => x.ErrorMessage)).ToList());
            }

            // Validate the Captcha Code if it was entered
            if (!string.IsNullOrEmpty(model.CaptchaCode) && 
                !Captcha.Validate(model.CaptchaId, model.CaptchaCode, model.CaptchaInstanceId))
            {
                result.Errors.Add(CaptchaErrorMessage);
            }
            
            // If there are no errors so far, map the view model to the data model and try to insert it into the database.
            // If the insertion fails, add a user friendly error message.
            if (result.Success && !ContactRepository.Insert(AutoMapper.Mapper.Map<Contact>(model)))
            {
                result.Errors.Add(DefaultErrorMessage);
            }

            return Json(result);
        }

        /// <summary>
        /// Validates the CAPTCHA code
        /// </summary>
        /// <returns>A JsonResult indicating success/failure and containing appropriate messages.</returns>
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult ValidateCaptcha(string captchaId, string instanceId, string userInput)
        {
            return Json(Captcha.AjaxValidate(captchaId, userInput, instanceId));
        }
    }
}
