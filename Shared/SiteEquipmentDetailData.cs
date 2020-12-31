using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FOS.Shared
{

    public class SiteEquipmentDetailData
    {
     
        public int ID { get; set; }
        public int ClientID { get; set; }
        public int? RegionID { get; set; }
        public int? ZoneID { get; set; }
        [DisplayName("Region Name *")]
        public string RegionName { get; set; }

        [DisplayName("City *")]
        public int? CityID { get; set; }
        [DisplayName("City *")]
        public string CItyName { get; set; }

        [DisplayName("Area *")]
        public int AreaID { get; set; }

        [DisplayName("Area *")]
        public string AreaName { get; set; }
        public int AreaNameID { get; set; }
        public int SaleOfficerID { get; set; }
        public int SubDivisionID { get; set; }

        public string SubDivisionName { get; set; }
        public int EquipmentCatID { get; set; }
        public int EquipmentTypeID { get; set; }
        public int? SiteID { get; set; }
        public string SiteName { get; set; }
        public int? BrandID { get; set; }
        public string BrandName { get; set; }
        public string MaterialNo { get; set; }
        public string EquipmentCatName { get; set; }
        public string EquipmentTypeName { get; set; }
        public string Condition { get; set; }
        public string Capacity { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string YearOfManufacture { get; set; }
        public string YearOfInstall { get; set; }

        public bool? Guarantee { get; set; }
        public string GuaranteeDetail { get; set; }

        public DateTime ExpiryDate { get; set; }
        public bool? MaintainedByKSB { get; set; }
        public string MaintaineByWhome { get; set; }
        public string Weight { get; set; }
        public string MediumInUse { get; set; }
        public string OperatingTemperature { get; set; }
        public string OperatingPressure { get; set; }
        public string Remarks { get; set; }
    

        public List<RegionData> Client { get; set; }
        public List<RegionalHeadData> RegionalHead { get; set; }
        public List<SaleOfficerData> SaleOfficers { get; set; }
        public List<DealerData> Dealers { get; set; }
        public List<CityData> Cities { get; set; }
        public List<RegionData> Regions { get; set; }
        public List<AreaData> Areas { get; set; }
        public List<SubDivisionData> SubDivisions { get; set; }
        public List<Equipment> EquipmentBrand { get; set; }
        public List<Equipment> EquipmentCategory { get; set; }
        public List<Equipment> EquipmentType { get; set; }
        public List<Fees> FeeStructure { get; set; }

    }




 



  

  


}