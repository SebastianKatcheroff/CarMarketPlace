using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace.DAO
{
    public class CarModel
    {
        [Key]
        public int CarModelId { get; set; }
        public int CarMakeId { get; set; }
        public string Name { get; set; }
    }
}
