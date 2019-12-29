using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAA.DataAccess.Model
{
    public class ApprovedMenu
    {
        public Int64? LiveOrderId { get; set; }
        public Int64 MenuId { get; set; }
        public string MenuCode { get; set; }
        public string Route { get; set; }        
        public string ClassName { get; set; }
        public string MenuType { get; set; }
        public string FlightNo { get; set; }
        public string Quantity { get; set; }
        public string LanguageName { get; set; }
    }
}
