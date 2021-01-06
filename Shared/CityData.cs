using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class CityData
    {
        public int ID { get; set; }
        public int? ComplaintID { get; set; }

        public int? SOID { get; set; }

        [DisplayName("City Name *")]
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }
        public string SaleOfficerName { get; set; }
        public string ShopName { get; set; }
        public string Remarks { get; set; }

        [DisplayName("Region *")]
        [Required(ErrorMessage = "* Required")]
        public int RegionID { get; set; }

        public int ClientId { get; set; }

        [DisplayName("Region Name *")]
        public string RegionName { get; set; }

        [DisplayName("City Code *")]
        public string CityCode { get; set; }

        [DisplayName("Short Code *")]
        public string ShortCode { get; set; }
        public DateTime? LaunchAt { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdate { get; set; }

        public string SiteName { get; set; }
        public List<Region> Regions { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }

    }

    public class CityGraphData
    {
        public string RegionName { get; set; }
        public int CityCount { get; set; }
    }

    public class MainCategories
    {
        public int ID { get; set; }
        public string MainCategoryName { get; set; }
        public List<MainCategory> Regions { get; set; }


    }


    public class SubCategories
    {
        public int MainCategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public int ID { get; set; }
        public string SubName { get; set; }
        public List<MainCategory> Regions { get; set; }
        public int CityID { get; set; }
        public bool IsActive { get; set; }
        public List<CityData> Cities { get; set; }
    
    public List<MainCategories> mainCat { get; set; }

    }

    public class Items
    {
        public int ItemId { get; set; }
        public int MainCategoryID { get; set; }
        public int SubCategoryAID { get; set; }
        public string SubCategoryAName { get; set; }
        public string ItemName { get; set; }

        public string Scheme { get; set; }
        public string ItemDesc { get; set; }
        public string ItemCode { get; set; }
        public int ItemPacking { get; set; }
        public int? SortOrder { get; set; }
        public decimal ItemPrice { get; set; }
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public List<SubCategories> SubCategory { get; set; }

        public List<SubCategoryA> SubCategoryAList { get; set; }
        public List<MainCategories> Regions { get; set; }
        public List<MainCategories> mainCat { get; set; }

    }



    public class SubCategoryA
    {
        public int SubCategoryAID{ get; set; }
        public int MainCategoryID { get; set; }
        public string SubCategoryAName { get; set; }
        public string MainCatName { get; set; }
        public string SubCatName { get; set; }
        public string ItemDesc { get; set; }
        public string ItemCode { get; set; }

        public decimal ItemPrice { get; set; }
        public int ID { get; set; }
        public bool IsActive { get; set; }
      
        public List<SubCategories> SubCategory { get; set; }
        public List<MainCategories> Regions { get; set; }
        public List<MainCategories> mainCat { get; set; }

    }


    public class ZonesData
    {
        public int ID { get; set; }

   
        public string Name { get; set; }

       
        public int RegionID { get; set; }

  
        public string RegionName { get; set; }

        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdate { get; set; }

        public List<Region> Regions { get; set; }
        public List<Zone> Zones { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }

    }



}