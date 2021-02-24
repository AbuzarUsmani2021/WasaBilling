using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class NotificationsForWasaController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ProjectID,int RoleID, int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            try
            {
               
                if (RoleID > 0)
                {
                    object[] param = { RoleID };
                    List<NotificationsForWasa> list = new List<NotificationsForWasa>();
                    NotificationsForWasa comlist;

                    var result = db.Sp_KSBNotificationsforCC(ProjectID, dtFromToday, dtToToday).ToList();

                    var IDS = dbContext.SOZoneAndTowns.Where(x => x.SOID == SOID).Select(x => x.AreaID).ToList();


                    foreach (var items in IDS)
                    {
                        foreach (var item in result)
                        {
                            if (items == Convert.ToInt32(item.areas))
                            {

                                comlist = new NotificationsForWasa();
                                comlist.ComplaintID = item.ComplaintID;
                                comlist.SiteCode = item.SiteCode;
                                comlist.LaunchDate = item.LaunchDate;
                                comlist.SiteID = item.SiteID;
                                comlist.SiteName = item.SiteName;
                                comlist.ComplaintStatus = item.StatusName;
                                comlist.Childs = db.Sp_WasaChildNotifications(item.ComplaintID, SOID).ToList();
                                list.Add(comlist);

                            }
                        }
                        }
                    if (list != null && list.Count > 0)
                    {
                        return Ok(new
                        {
                            MyComplaintList = list

                        });
                    }

                }
                   
                   
                    


                  
                 
                }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "MyComplaintList GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                MyComplaintList = paramm
            });
        }
            

        }

    public class NotificationsForWasa
    {
        public int ComplaintID { get; set; }

        public DateTime? LaunchDate { get; set; }

        public string SiteName { get; set; }
        public int? SiteID { get; set; }
        public string SiteCode { get; set; }
        public string ComplaintStatus { get; set; }


        public List<Sp_WasaChildNotifications_Result> Childs { get; set; }


    }


    public class ChildNotificationsForWasa
    {
        public int ComplaintID { get; set; }
        public string TicketNo { get; set; }
        public DateTime? LaunchDate { get; set; }
        public DateTime? LastDate { get; set; }
        public string SiteName { get; set; }

        public string LaunchedByName { get; set; }
        public int? SiteID { get; set; }

        public bool IsSiteIDChanged { get; set; }
        public int? PriorityID { get; set; }
        public bool IsPriorityIDChanged { get; set; }
        public string SiteCode { get; set; }
        public bool IsSiteCodeChanged { get; set; }
        public string ComplaintStatusID { get; set; }
        public bool IsComplaintStatusIDChanged { get; set; }
        public string FaultType { get; set; }
        public bool IsFaultTypeIDChanged { get; set; }
        public string FaultTypeDetail { get; set; }
        public bool IsFaultTypeDetailIDChanged { get; set; }
        public string FaultTypeDetailOtherRemarks { get; set; }
        public bool IsFaultTypeDetailOtherRemarksChanged { get; set; }
        public string AssignedSaleOfficerName { get; set; }
        public bool IsAssignedSaleOfficerNameChanged { get; set; }
        public string PersonName { get; set; }
        public bool IsPersonNameChanged { get; set; }

        public string Picture1 { get; set; }
        public bool IsPicture1Changed { get; set; }
        public string Picture2 { get; set; }
        public bool IsPicture2Changed { get; set; }
        public string Picture3 { get; set; }
        public bool IsPicture3Changed { get; set; }
        public int ProgressStatusID { get; set; }
        public bool IsProgressStatusIDChanged { get; set; }
        public string updateRemarks { get; set; }
        public bool IsupdateRemarksChanged { get; set; }
        public string ProgressStatusOtherRemarks { get; set; }
        public bool IsProgressStatusOtherRemarksChanged { get; set; }
    }


}







