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
    public class MyComplaintListForWasaController : ApiController
    {
       

        public IHttpActionResult Get(int SOID, int ProjectID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                if (SOID > 0)
                {
                    object[] param = { SOID };
                    List<MyComplaintList> list = new List<MyComplaintList>();
                    MyComplaintList comlist;



                    var IDS = dbContext.SOZoneAndTowns.Where(x => x.SOID == SOID).Select(x => x.AreaID).ToList();

                   // var data = dbContext.Jobs.Where(x => x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday);

                    var result = dbContext.Sp_MyComplaintListForWasa1_2(ProjectID,dtFromToday,dtToToday).ToList();


                    

                    //var result1= dbContext.Sp_MyComplaintListRemarks(SOID, dtFromToday, dtToToday).ToList();




                    foreach (var item in IDS)
                    {
                        foreach (var items in result)
                        {
                            if (item ==  Convert.ToInt32(items.AreaID))
                            {
                                comlist = new MyComplaintList();


                                comlist.ComplaintID = items.ComplaintID;
                                comlist.SiteCode = items.SiteCode;
                                comlist.LaunchDate = (DateTime)items.LaunchDate;
                                comlist.SaleOfficerName = items.SaleOfficerName;
                                comlist.SiteID = items.SiteID;
                                comlist.SiteName = items.SiteName;
                                comlist.TicketNo = items.TicketNo;
                                comlist.InitialRemarks = items.InitialRemarks;
                                comlist.LaunchedByName = items.LaunchedByName;
                                comlist.ComplaintStatus = items.StatusName;
                                comlist.FaultType = items.FaulttypeName;
                                comlist.FaultTypeDetail = items.FaulttypedetailName;
                              
                                if (items.FaulttypedetailName == "Others")
                                {
                                    var otherremarks = dbContext.JobsDetails.Where(x => x.JobID == items.ComplaintID).OrderByDescending(x => x.ID).Select(x => x.ActivityType).FirstOrDefault();

                                    comlist.FaultTypeDetail = items.FaulttypedetailName + "/" + otherremarks;
                                }

                                if (items.StatusName == "Resolved")
                                {
                                    comlist.ProgressRemarks = new CommonController().GetProgressStatusForWasaResolved(items.ComplaintID);
                                }
                                else
                                {
                                    comlist.ProgressRemarks = new CommonController().GetProgressStatusForWasa(items.ComplaintID);
                                }
                                comlist.ClientRemarks = new CommonController().GetClientRemarks(items.ComplaintID);

                                list.Add(comlist);

                            }
                        }
                       
                    }
                    
                    if (list != null && list.Count > 0)
                    {
                        var finallist= list.OrderByDescending(o => o.ComplaintID).ToList();

                        return Ok(new
                        {
                            MyComplaintListForWasa = finallist

                        });
                    }
                 
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "MyComplaintListForWasa GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                MyComplaintListForWasa = paramm
            });

        }


    }


    public class MyComplaintListForWasa
    {
        public int ComplaintID { get; set; }
        public string TicketNo { get; set; }
        public DateTime? LaunchDate { get; set; }

        public string SiteName { get; set; }

        public string LaunchedByName { get; set; }
        public int? SiteID { get; set; }

        public string SiteCode { get; set; }

        public string ComplaintStatus { get; set; }
        public string FaultType { get; set; }
        public string FaultTypeDetail { get; set; }

        public string SaleOfficerName { get; set; }

        public string InitialRemarks { get; set; }

        public string ProgressRemarks { get; set; }
    }
}