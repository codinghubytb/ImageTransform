using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LibraryServiceImageTransform.Models
{
    public class BAL_Type
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}
