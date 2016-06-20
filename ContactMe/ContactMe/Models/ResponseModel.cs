using System.Collections.Generic;
using System.Linq;

namespace ContactMe.Models
{
    /// <summary>
    /// A helper view model for returning JSON for AJAX postbacks.
    /// </summary>
    public class ResponseModel
    {
        public ResponseModel(List<string> errors = null)
        {
            Errors = errors ?? new List<string>();
        }

        public List<string> Errors { get; set; }

        public bool Success
        {
            get
            {
                return !Errors.Any();
            }
        }
    }
}