using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class BaseResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IList<string> Errors { get; set; }

        public static BaseResult<T> Succeed()
        {
            return new BaseResult<T>()
            {
                Success = true,
                Message = "Success"
            };
        }

        public static BaseResult<T> Fail(string message = "Failure", IEnumerable<string> errors = null)
        {
            return new BaseResult<T>()
            {
                Success = false,
                Message = message,
                Errors = errors?.ToList()
            };
        }
    }
}
