using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace.DAO
{
    public class Quotes
    {
        [Key]
        public int QuoteId { get; set; }
        public int BuyerId { get; set; }
        public int ZipCodeId { get; set; }
        public decimal Amount { get; set; }
        public bool IsCurrent { get; set; }
        public int CarId { get; set; }
    }
}
