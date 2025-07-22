using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebWacker.Models
{
    public class WebExecutionResult
    {
        public string Url { get; set; } = string.Empty;

        public int StatusCode { get; set; } = 0;

        public long ResponseTime { get; set; } = 0;
    }
}
