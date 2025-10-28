using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS
{
    public class BillItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public decimal CQFT { get; set; }
        public decimal GST { get; set; }
        public decimal Amount { get; set; }
    }
}
