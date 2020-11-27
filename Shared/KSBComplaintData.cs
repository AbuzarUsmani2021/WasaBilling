using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
   public class KSBComplaintData
    {
        public int ID { get; set; }
        public int? ClientId { get; set; }
        public int? SaleOfficerID { get; set; }
        public string SaleOfficerName { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string ComplaintType { get; set; }



        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? ReslovedHours { get; set; }
        public string TicketNo { get; set; }
        public string FaultTypesDetailName{ get; set; }





        public int? RegionID { get; set; }
        public string RegionName { get; set; }
        public int ProjectId { get; set; }
        public int? CityID { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string CItyName { get; set; }
        [DisplayName("Retailer Name *")]
        public string RetailerName { get; set; }
   
        public string FaultTypeName { get; set; }
        public int? ComplaintTypeID { get; set; }
        public string PriorityName { get; set; }

        public string FaultTypeDetailOtherRemarks { get; set; }
        public string  ProgressStatusOtherRemarks { get; set; }
        public Nullable<int> LaunchedByID { get; set; }
        public string LaunchedByName { get; set; }

        public string DisableSiteID { get; set; }

        public Nullable<int> StatusID { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> RetailerID { get; set; }
        public int? SubDivisionID { get; set; }

        public string SubDivisionName { get; set; }
        public int AreaID { get; set; }
        public int SiteId { get; set; }
        public int? FaulttypeId { get; set; }
       
        public int? PriorityId { get; set; }
       
        public int? ProgressStatusId { get; set; }
        public string ProgressStatusName { get; set; }
        public int? FaulttypeDetailId { get; set; }
       
        public string Picture1 { get; set; }
        public string Picture2 { get; set; }
        public string Picture3 { get; set; }
        public string Picture4 { get; set; }

        public string Picture5 { get; set; }
        public string Name { get; set; }

        public string Remarks { get; set; }

        public List<CityData> Cities { get; set; }
        public List<RegionData> Regions { get; set; }
        public List<AreaData> Areas { get; set; }
        public List<RegionData> Client { get; set; }
        public List<FaultTypeData> faultTypes { get; set; }
        public List<PriorityData> priorityDatas { get; set; }
        public List<ComplaintStatus> complaintStatuses { get; set; }
        public List<ComplaintStatus> ProgressStatus { get; set; }
        public List<ComplaintStatus> ComplaintTypes { get; set; }
        public List<FaultTypeData> faultTypesDetail { get; set; }
        public List<ComplaintLaunchedBy> LaunchedBy { get; set; }
        public List<SubDivisionData> SubDivisions { get; set; }
        public List<RetailerData> Sites { get; set; }
        public List<SaleOfficerData> SaleOfficers { get; set; }
    }
}
