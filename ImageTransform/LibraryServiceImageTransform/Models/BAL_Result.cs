using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServiceImageTransform.Models
{
    public class BAL_Result
    {
        public string base64Data { get;set; }
        public string image { get; set; }
        public string error { get; set; }
        public string format { get; set; }
    }
}
