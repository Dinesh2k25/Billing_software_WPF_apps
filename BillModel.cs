using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS
{
    
        public class BillModel
        {
            public string BillID { get; set; }
            public string Date { get; set; }
            public string CName { get; set; }
            public string CCompanyName { get; set; }
            public string CPhone { get; set; }
            public string CAddress { get; set; }
            public string CEmail { get; set; }
            public decimal Total { get; set; }
            public List<BillItem> Items { get; set; } = new List<BillItem>();
        }
    

}
