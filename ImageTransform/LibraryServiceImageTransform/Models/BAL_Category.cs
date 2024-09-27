using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServiceImageTransform.Models
{
	public class BAL_Category
	{
		[JsonProperty("_id")]
		public string Id { get; set; }

		public string Name { get; set; }

        public string Description { get; set; }
    }
}
