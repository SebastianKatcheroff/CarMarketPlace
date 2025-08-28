using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace.DAO
{
    public class Lookup
    {
        [Key]
        public int LookupId { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
    }
}
