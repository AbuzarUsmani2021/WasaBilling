using FOS.DataLayer;
using FOS.Shared.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{

    public class JobsData
    {

        public JobsData()
        {
            this.VisitPlanEach = new SelectedWeekday("0000000");
        }

        public int ID { get; set; }
        public string ItemName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        [DisplayName("Job Title *")]
        [Required(ErrorMessage = "* Required")]
        public string JobTitle { get; set; }
        public string Picture1 { get; set; }
        public string Picture2 { get; set; }
        public string Picture3 { get; set; }
        public string Picture4 { get; set; }

        public string Picture5 { get; set; }

        public int Type { get; set; }
        [DisplayName("Regional Head *")]
        [Required(ErrorMessage = "* Required")]
        public Nullable<int> RegionalHeadID { get; set; }
        public int HiddenRegionalHeadID { get; set; }
        [DisplayName("Sales Officer Name *")]
        public string RegionalHeadName { get; set; }

        //Sales Officer
        [DisplayName("Sale Officer ID *")]
        [Required(ErrorMessage = "* Required")]
        public Nullable<int> SaleOfficerID { get; set; }
        [DisplayName("Sales Officer Name *")]
        public string SaleOfficerName { get; set; }

        public string RetailerType { get; set; }
        public string RT { get; set; }

        public string VisitType { get; set; }

        /// <Retailer>
        [DisplayName("RetailerID *")]
        [Required(ErrorMessage = "* Required")]
        public int? RetailerID { get; set; }

        public string SelectedRetailers { get; set; }
        public string SelectedDealers { get; set; }
        public string SelectedPainters { get; set; }
        public string SelectedAreas { get; set; }
        
        [DisplayName("Retailer Name *")]
        public string RetailerName { get; set; }
        public Nullable<int> FaultTypeID { get; set; }
        public string FaultTypeName { get; set; }
        public Nullable<int> PriorityID { get; set; }
        public string PriorityName { get; set; }
        public Nullable<int> LaunchedByID { get; set; }
        public string LaunchedByName { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int DealerID { get; set; }
        public string DealerName { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string LocationMargin { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public int? CityID { get; set; }
        public string CItyName { get; set; }
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        /// </Retailer>

        [DisplayName("Schedule No")]
        [Required(ErrorMessage = "* Required")]
        public string JobNo { get; set; }

        /// <Visit Plan>
        [DisplayName("Visit Plan")]
        [Required(ErrorMessage = "* Required")]
        public int VisitPlanID { get; set; }
        public int? RegionID { get; set; }
        public string RegionName { get; set; }
        public int VisitPlanHiddenID { get; set; }
        public string VisitPlanName { get; set; }
        public string VisitPlanDays { get; set; }
        public string VisitPlanWeeklyDays { get; set; }
        public string VisitPlanEachDays { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
    
        public int SiteId { get; set; }
        public int FaulttypeId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }

        public int LaunchedById { get; set; }

        public string Name { get; set; }

        public string Remarks { get; set; }

        public List<CityData> Cities { get; set; }
        public List<RegionData> Regions { get; set; }
        public List<AreaData> Areas { get; set; }
        public List<RegionData> Client { get; set; }
        public List<FaultTypeData> faultTypes { get; set; }
        public List<PriorityData> priorityDatas { get; set; }
        public List<ComplaintStatus> complaintStatuses { get; set; }
        public List<ComplaintLaunchedBy> LaunchedBy { get; set; }
        public List<SaleOfficerData> SaleOfficers { get; set; }

        public SelectedWeekday VisitPlanEach
        {
            get;
            set;
        }
         
        public bool Status { get; set; }

        [DisplayName("Job Assign Date")]
        public string DateOfAssign { get; set; }
        public string VisitedDate { get; set; }

        [DisplayName("Job Compeletion Date")]
        public string DateOfExecution { get; set; }

        public Nullable<System.DateTime> StartingDate { get; set; }
        
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public IEnumerable<RegionalHeadTypeData> RegionalHeadTypeData { get; set; }
        public virtual List<RegionalHeadData> RegionalHead { get; set; }
        public virtual List<SaleOfficer> SaleOfficer { get; set; }
        public virtual List<DealerData> Dealer { get; set; }
        public virtual List<RetailerData> Retailers { get; set; }
      
        public virtual List<VisitPlanData> VisitPlan { get; set; }

        // Painter 
        public string City { get; set; }
        public virtual List<PainterCityNamesData> PainterCityNames { get; set; }
        public int PainterCityID { get; set; }

        
    }

    public class JobsDetailData
    {
        public int ID { get; set; }
        public int? RegionID { get; set; }
        public string RegionName { get; set; }
        public string JobTitle { get; set; }
        public int JobID { get; set; }
        public Nullable<int> RetailerID { get; set; }
        public string RetailerName { get; set; }
        public Nullable<int> FaultTypeID { get; set; }
        public string FaultTypeName { get; set; }
        public string ComplaintTypeName { get; set; }
        public Nullable<int> FaultTypeDetailID { get; set; }
        public string FaultTypeDetailName { get; set; }
        public string FaultTypeFinalName { get {

                return FaultTypeName + "/" + FaultTypeDetailName;
            } }
        public Nullable<int> PriorityID { get; set; }
        public Nullable<int> ResolvedHours { get; set; }
        public Nullable<int> Exceed { get; set; }


        public string PriorityName { get; set; }
        public Nullable<int> LaunchedByID { get; set; }
        public string LaunchedByName { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string FaultTypeDetailOtherRemarks { get; set; }
        public string ProgressStatusOtherRemarks { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> ProgressStatusID { get; set; }
        public string ProgressStatus { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string ShopName { get; set; }
        public string PainterName { get; set; }
        public string TicketNo { get; set; }

        public string SiteCode { get; set; }
        public int RegionalHeadID { get; set; }
        public string RegionalHeadName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string ShopAddress { get; set; }
        public string LatLong { get; set; }
       
        public Nullable<int> SaleOfficerID { get; set; }
        public string SaleOfficerName { get; set; }
        public Nullable<int> DealerID { get; set; }
        public string DealerName { get; set; }
        public string DealerPhone { get; set; }
        public string VisitPlanName { get; set; }
        public Nullable<bool> Mapple { get; set; }
        public Nullable<bool> DG { get; set; }
        public Nullable<bool> BestWay { get; set; }
        public Nullable<bool> Lucky { get; set; }
        public Nullable<bool> Other { get; set; }
        public string Major { get; set; }
        public Nullable<double> MapplePrice { get; set; }
        public string Display { get; set; }
        public string VisitType { get; set; }
        public string dateformat { get; set; }
        public string UpdatedAt { get; set; }
        public TimeSpan ElapseTime { get
            {
                
                return d1.Subtract(d2);
            }
             }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
        public DateTime ResolvedAt { get; set; }

        public TimeSpan ResolveTime
        {
            get
            {

                return ResolvedAt.Subtract(d2);
            }
        }
        // Retailer
        public string RetailerType { get; set; }
        public Nullable<bool> SAvailable { get; set; }
        public string SAvailableFormatted { get; set; }
        public int? SQuantity1KG { get; set; }
        public int? SQuantity5KG { get; set; }
        public bool? SNewOrder { get; set; }
        public string SNewOrderFormatted { get; set; }
        public int? SNewQuantity1KG { get; set; }
        public int? SNewQuantity5KG { get; set; }
        public int? SPreviousOrder1KG { get; set; }
        public int? SPreviousOrder5KG { get; set; }
        public bool? SPOSMaterialAvailable { get; set; }
        public string SPOSMaterialAvailableFormatted { get; set; }
        public string SImage { get; set; }
        public string SImageFormatted { get; set; }
        public string SNote { get; set; }

        // Painter
        public Nullable<bool> PUseWC { get; set; }
        public Nullable<int> PUseWC1KG { get; set; }
        public Nullable<int> PUseWC5KG { get; set; }
        public Nullable<bool> PNewOrder { get; set; }
        public Nullable<int> PNewOrder1KG { get; set; }
        public Nullable<int> PNewOrder5KG { get; set; }
        public Nullable<bool> PNewLead { get; set; }
        public string PNewLeadMobNo { get; set; }
        public string PRemarks { get; set; }
        
        // B2B
        public Nullable<int> AreaID { get; set; }
        public string BAreaName { get; set; }
        public string BShop { get; set; }
        public string BOldHouse { get; set; }
        public string BNewHouse { get; set; }
        public string BParking { get; set; }
        public string BPlazaBasement { get; set; }
        public string BFactoryArea { get; set; }
        public string BMosque { get; set; }
        public string BOthers { get; set; }
        public Nullable<bool> BLead { get; set; }
        public Nullable<bool> BSampleApplied { get; set; }
        public string BRemarks { get; set; }


        public Nullable<bool> Status { get; set; }
        public string StatusChecker { get; set; }
        public DateTime? AssignDate { get; set; }
        public DateTime? VisitedDate { get; set; }
        public DateTime? VisitDate { get; set; }
        public string VisitDateFormatted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedDateFormatted { get; set; }
        public string RetailerAddress { get; set; }
        public string PainterAddress { get; set; }

        public List<ProgressStatusData> ProgressStatuses { get; set; }
        public List<DealerData> Dealers { get; set; }
        public List<RetailerData> Retailers { get; set; }
    }

    public class VisitPlanData
    {
        public int ID { get; set; }
        public string Type { get; set; }
    }

    public class JobGraphData
    {
        public int Donejobs;
        public int Pendingjobs;
    }

    public class ProgressStatusData
    {
        public int ID;
        public int Name;
    }

    public class SojobGraphData
    {
        public string RegionName { get; set; }
        public int JobsCount { get; set; }
    }

    public class CompletePlanData
    {
        public int MonthlyVisit { get; set; }
        public int WeeklyVisit { get; set; }
        public int DailyVisit { get; set; }
    }



}


