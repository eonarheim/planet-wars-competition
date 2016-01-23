using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class BaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static BaseResult Succeed()
        {
            return new BaseResult()
            {
                Success = true,
                Message = "Success"
            };
        }

        public static BaseResult Fail(string message)
        {
            return new BaseResult()
            {
                Success = false,
                Message = message
            };
        }
    }
}
