using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactMe.Models
{
    /// <summary>
    /// The view model used by the Contact page.
    /// </summary>
    public class ContactModel : IValidatableObject
    {
        public const int MinuteIncrement = 15;
        public const string MinTime = "9:00 AM";
        public const string MaxTime = "6:00 PM";

        public DateTime MinimumTime
        {
            get
            {
                return DateTime.Parse(MinTime);
            }
        }

        public DateTime MaximumTime
        {
            get
            {
                return DateTime.Parse(MaxTime);
            }
        }

        [Required(ErrorMessage = "Enter your first name.")]
        [StringLength(40, ErrorMessage = "Enter a first name that does not exceed 40 characters.")]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name.")]
        [StringLength(40, ErrorMessage = "Enter a last name that does not exceed 40 characters.")]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter your email address.")]
        [StringLength(80, ErrorMessage = "Enter an email address that does not exceed 80 characters.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Enter a valid email address such as \"name@domain.com.\"")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [StringLength(16, ErrorMessage = "Enter a phone number that does not exceed 16 characters.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9 \\-+()x.]*$", ErrorMessage = "Enter a valid phone number such as \"555-555-5555\".")]
        [Display(Name = "Phone Number")]
        public string Telephone { get; set; }

        [DataType(DataType.Time, ErrorMessage = "Enter a 12 hour time to call such as \"12:30\".")]
        [Display(Name = "Best Time to Call")]
        public DateTime? BestTimeToCall { get; set; }

        [Required(ErrorMessage = "Enter the Captcha code.")]
        [Display(Name = "Prove You Aren't a Robot")]
        public string CaptchaCode { get; set; }

        public string CaptchaId { get; set; }

        public string CaptchaInstanceId { get; set; }

        /// <summary>
        /// Custom server side validation for the BestTimeToCall
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BestTimeToCall.HasValue)
            {
                var time = BestTimeToCall.Value.TimeOfDay;

                // Validate that the time is within the range.
                if (time < MinimumTime.TimeOfDay || time > MaximumTime.TimeOfDay)
                {
                    yield return new ValidationResult(string.Format("Enter a time between {0} and {1}.", MinTime, MaxTime), new[] { "BestTimeToCall" });
                }

                // Validate that the minutes are a valid increment.
                if (time.Minutes % MinuteIncrement != 0)
                {
                    yield return new ValidationResult(string.Format("Enter a time with a {0} minute increment.", MinuteIncrement), new[] { "BestTimeToCall" });
                }
            }
        }
    }
}
