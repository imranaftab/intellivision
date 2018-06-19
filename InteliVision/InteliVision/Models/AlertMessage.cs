using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteliVision.Models
{
    public class AlertMessage
    {
        public string Token { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string DialogStyle { get; set; }
    }
}
