using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace.DAO
{
    public class CarSubModel
    {
        [Key]
        public int CarSubModelId { get; set; }
        public int CarModelId { get; set; }
        public string Name { get; set; }
    }
}
