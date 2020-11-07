using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class OrderSummaryReportData
    {

        public int RetailerID { get; set; }
        public string ShopName { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int DistributorID { get; set; }
        public int RangeID { get; set; }
        public int RegionID { get; set; }
        public int CityID { get; set; }
        public int SaleOfficerID { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<CategoryData> range { get; set; }
        public List<SaleOfficerData> saleofficerdata { get; set; }
        public List<RetailerData> dealerdata { get; set; }
        public List<CityData> CityData { get; set; }
        public List<RegionData> regionData { get; set; }
    }
}
