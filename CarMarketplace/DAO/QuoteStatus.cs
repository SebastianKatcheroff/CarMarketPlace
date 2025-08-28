using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace.DAO
{
    public class QuoteStatus
    {
        [Key]
        public int QuoteStatusId { get; set; }
        public int QuoteId { get; set; }
        public int StatusId { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
