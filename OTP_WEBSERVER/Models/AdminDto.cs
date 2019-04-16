using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTP_WEBSERVER.Models
{
    public class AdminDto
    {
        public IEnumerable<Application> Applications { get; set; }
    }
}