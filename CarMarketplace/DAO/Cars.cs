using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace.DAO
{
    public class Cars
    {
        [Key]
        public int CarId { get; set; }
        public int Year { get; set; }
        public int CarSubModelId { get; set; }
        public int ZipCodeId { get; set; }
        public int CustomerId { get; set; }
    }
}
