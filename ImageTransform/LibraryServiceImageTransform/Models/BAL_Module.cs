using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServiceImageTransform.Models
{
    public class BAL_Module
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = default!;

        public string GuId { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;
        
        public DateTime DateCreated { get; set; }

        public string Path { get; set; } = default!;

        public string Icon { get; set; } = default!;

        public List<string> Category { get; set; } = new List<string>();

        public List<string> Type { get; set; } = new List<string>();

        public int Visit { get; set; } = default!;
        public bool Enabled { get; set; } = default!;

    }
}
