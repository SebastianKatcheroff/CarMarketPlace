using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace.DAO
{
    public class Invoice
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Decimal Total { get; set; }
    }
}
