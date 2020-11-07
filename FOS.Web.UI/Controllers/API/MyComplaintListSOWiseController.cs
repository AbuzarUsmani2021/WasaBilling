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
    public class MyComplaintListSOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,int SaleOfficerID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                var RoleID = dbContext.SaleOfficers.Where(x => x.ID == SaleOfficerID).Select(x => x.RoleID).FirstOrDefault();

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                if (SOID > 0)
                {
                    object[] param = { SOID };
                    List<MyComplaintList> list = new List<MyComplaintList>();
                    MyComplaintList comlist;

                    var result = dbContext.Sp_MyComplaintList1_3(SOID,dtFromToday,dtToToday).ToList();


                    var result1= dbContext.Sp_MyComplaintListRemarks1_2(SOID, dtFromToday, dtToToday).ToList();


                    if (RoleID != 3)
                    {

                        foreach (var item in result)
                        {
                            foreach (var items in result1)
                            {
                                if (item.ComplaintID == items.ComplaintID)
                                {
                                    comlist = new MyComplaintList();


                                    comlist.ComplaintID = item.ComplaintID;
                                    comlist.SiteCode = item.SiteCode;
                                    comlist.LaunchDate = item.LaunchDate;
                                    comlist.SiteID = item.SiteID;
                                    comlist.SiteName = item.SiteName;
                                    comlist.TicketNo = item.TicketNo;
                                    comlist.LaunchedByName = item.LaunchedByName;
                                    comlist.SaleOfficerName = item.LaunchedByName;
                                    comlist.ProgressRemarks = items.ProgressStatusName;
                                    comlist.InitialRemarks = item.InitialRemarks;
                                    comlist.ComplaintStatus = item.StatusName;
                                    comlist.FaultType = item.FaulttypeName;
                                   
                                    comlist.FaultTypeDetail = item.FaulttypedetailName;

                                    if (item.FaulttypedetailName == "Others")
                                    {
                                        var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ActivityType).FirstOrDefault();

                                        comlist.FaultTypeDetail = item.FaulttypedetailName + "/" + otherremarks;
                                    }
                                    if (items.ProgressStatusName == "Others")
                                    {
                                       // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                                        comlist.ProgressRemarks = items.ProgressStatusName + "/" + items.ProgressStatusRemarks;
                                    }



                                    list.Add(comlist);

                                }
                            }

                        }
                    }
                    else
                    {
                        foreach (var item in result)
                        {
                            foreach (var items in result1)
                            {
                                if (item.ComplaintID == items.ComplaintID)
                                {
                                    if (SaleOfficerID == items.SaleOfficerID)
                                    {

                                        comlist = new MyComplaintList();
                                        comlist.ComplaintID = item.ComplaintID;
                                        comlist.SiteCode = item.SiteCode;
                                        comlist.LaunchDate = item.LaunchDate;
                                        comlist.SiteID = item.SiteID;
                                        comlist.SiteName = item.SiteName;
                                        comlist.TicketNo = item.TicketNo;
                                        comlist.LaunchedByName = item.LaunchedByName;
                                        comlist.SaleOfficerName = item.LaunchedByName;
                                        comlist.ProgressRemarks = items.ProgressStatusName;
                                        comlist.InitialRemarks = item.InitialRemarks;
                                        comlist.ComplaintStatus = item.StatusName;
                                        comlist.FaultType = item.FaulttypeName;
                                        comlist.FaultTypeDetail = item.FaulttypedetailName;
                                        if (item.FaulttypedetailName == "Other")
                                        {
                                            var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ActivityType).FirstOrDefault();

                                            comlist.FaultTypeDetail = comlist.FaultTypeDetail + "/" + otherremarks;
                                        }

                                        if (items.ProgressStatusName == "Others")
                                        {
                                           // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                                            comlist.ProgressRemarks = items.ProgressStatusName + "/" + items.ProgressStatusRemarks;
                                        }
                                        list.Add(comlist);
                                    }
                                }
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
            object[] paramm = {};
            return Ok(new
            {
                MyComplaintList = paramm
            });

        }


    }


    public class MyComplaintList
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
        public string FaultTypeDetailOther { get; set; }
        public string SaleOfficerName { get; set; }

        public string InitialRemarks { get; set; }

        public string ProgressRemarks { get; set; }
    }
}