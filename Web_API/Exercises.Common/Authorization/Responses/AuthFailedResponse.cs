using System.Collections.Generic;

namespace Exercises.Common.Authorization.Responses
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}