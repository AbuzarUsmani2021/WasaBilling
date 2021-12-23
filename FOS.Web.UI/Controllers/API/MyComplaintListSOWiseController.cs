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

                    var result = dbContext.Sp_MyComplaintList1_4(SOID,dtFromToday,dtToToday).ToList();


                    //var result1= dbContext.Sp_MyComplaintListRemarksFinal(SOID, dtFromToday, dtToToday).ToList();


                    if (RoleID != 3)
                    {
                        var query = result.GroupBy(d => d.ComplaintID)
                                       .SelectMany(g => g.OrderByDescending(d => d.historydate)
                                     .Take(1));

                        foreach (var item in query)
                        {
                            comlist = new MyComplaintList();
                            comlist.ComplaintID = item.ComplaintID;
                            comlist.SiteCode = item.SiteCode;
                            comlist.LaunchDate = (DateTime)item.LaunchDate;
                            comlist.SiteID = item.SiteID;
                            comlist.SiteName = item.SiteName;
                            comlist.TicketNo = item.TicketNo;
                            comlist.LaunchedByName = db.SaleOfficers.Where(x => x.ID == item.LaunchedById).Select(x => x.Name).FirstOrDefault();
                            comlist.SaleOfficerName = db.SaleOfficers.Where(x => x.ID == item.LaunchedById).Select(x => x.Name).FirstOrDefault();
                            comlist.ProgressRemarks = item.ProgressStatusName + " " + "(" + item.historydate + ")";
                            comlist.InitialRemarks = item.InitialRemarks;
                            comlist.ComplaintStatus = item.StatusName;
                            comlist.FaultType = item.FaulttypeName;
                            comlist.FaultTypeDetail = item.FaulttypedetailName;
                            if (item.FaulttypedetailName == "Other")
                            {


                                comlist.FaultTypeDetail = comlist.FaultTypeDetail + "/" + item.faulttypedetailremarks;
                            }

                            if (item.ProgressStatusName == "Others")
                            {
                                // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                                comlist.ProgressRemarks = item.ProgressStatusName + "/" + item.ProgressStatusRemarks + "(" + item.historydate + ")";
                            }
                            if (item.ComplaintStatusId == 3)
                            {
                                comlist.ProgressRemarks = db.WorkDones.Where(x => x.ID == item.progressstatusid).Select(x => x.Name).FirstOrDefault() + "(" + item.historydate + ")";
                            }
                            comlist.ClientRemarks = new CommonController().GetClientRemarks((int)item.ComplaintID);
                            list.Add(comlist);
                        }


                        //foreach (var item in result)
                        //{
                        //    foreach (var items in result1)
                        //    {
                        //        if (item.ComplaintID == items.ComplaintID)
                        //        {
                        //            comlist = new MyComplaintList();


                        //            comlist.ComplaintID = item.ComplaintID;
                        //            comlist.SiteCode = item.SiteCode;
                        //            comlist.LaunchDate = (DateTime)item.LaunchDate;
                        //            comlist.SiteID = item.SiteID;
                        //            comlist.SiteName = item.SiteName;
                        //            comlist.TicketNo = item.TicketNo;
                        //            comlist.LaunchedByName = item.LaunchedByName;
                        //            comlist.SaleOfficerName = item.LaunchedByName;
                        //            if (items.ProgressStatusName == null && items.ProgressstatusId==null)
                        //            {
                        //                comlist.ProgressRemarks = " ";
                        //            }
                        //            else
                        //            {

                        //                comlist.ProgressRemarks = items.ProgressStatusName + "(" + items.datecomplete + ")";
                        //            }
                        //            comlist.InitialRemarks = item.InitialRemarks;
                        //            comlist.ComplaintStatus = item.StatusName;
                        //            comlist.FaultType = item.FaulttypeName;


                        //            if (item.ComplaintStatusId == 3)
                        //            {
                        //                comlist.ProgressRemarks = db.WorkDones.Where(x => x.ID == items.ProgressstatusId).Select(x => x.Name).FirstOrDefault() + "(" + items.datecomplete + ")"; 
                        //            }

                        //            comlist.FaultTypeDetail = item.FaulttypedetailName;

                        //            if (item.FaulttypedetailName == "Others")
                        //            {
                        //                var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).OrderByDescending(x=>x.ID).Select(x => x.ActivityType).FirstOrDefault();

                        //                comlist.FaultTypeDetail = item.FaulttypedetailName + "/" + otherremarks;

                        //            }
                        //            if (items.ProgressStatusName == "Others")
                        //            {
                        //               // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                        //                comlist.ProgressRemarks = items.ProgressStatusName + "/" + items.ProgressStatusRemarks + "(" + items.datecomplete + ")"; 
                        //            }

                        //            comlist.ClientRemarks = new CommonController().GetClientRemarks(item.ComplaintID);


                        //            list.Add(comlist);

                        //        }
                        //    }

                        //}
                    }
                    else
                    {
                        var results = db.Sp_MyComplaintListForFS1_1(SOID, dtFromToday, dtToToday, SaleOfficerID).ToList();

                        var query = results.GroupBy(d => d.ComplaintID)
                                        .SelectMany(g => g.OrderByDescending(d => d.LaunchDate)
                                      .Take(1));

                        foreach (var item in query)
                        {
                            comlist = new MyComplaintList();
                            comlist.ComplaintID = item.ComplaintID;
                            comlist.SiteCode = item.SiteCode;
                            comlist.LaunchDate = (DateTime)item.LaunchDate;
                            comlist.SiteID = item.SiteID;
                            comlist.SiteName = item.SiteName;
                            comlist.TicketNo = item.TicketNo;
                           comlist.LaunchedByName = db.SaleOfficers.Where(x=>x.ID==item.LaunchedById).Select(x=>x.Name).FirstOrDefault();
                            comlist.SaleOfficerName = db.SaleOfficers.Where(x => x.ID == item.LaunchedById).Select(x => x.Name).FirstOrDefault();
                            comlist.ProgressRemarks = item.ProgressStatusName + " " + "(" + item.LaunchDate + ")";
                            comlist.InitialRemarks = item.InitialRemarks;
                            comlist.ComplaintStatus = item.StatusName;
                            comlist.FaultType = item.FaulttypeName;
                            comlist.FaultTypeDetail = item.FaulttypedetailName;
                            if (item.FaulttypedetailName == "Other")
                            {
                               

                                comlist.FaultTypeDetail = comlist.FaultTypeDetail + "/" + item.faulttypedetailremarks;
                            }

                            if (item.ProgressStatusName == "Others")
                            {
                                // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                                comlist.ProgressRemarks = item.ProgressStatusName + "/" + item.ProgressStatusRemarks + "(" + item.LaunchDate + ")";
                            }
                            if (item.ComplaintStatusId == 3)
                            {
                                comlist.ProgressRemarks = db.WorkDones.Where(x => x.ID == item.progressstatusid).Select(x => x.Name).FirstOrDefault() + "(" + item.LaunchDate + ")";
                            }
                            comlist.ClientRemarks = new CommonController().GetClientRemarks((int)item.ComplaintID);
                            list.Add(comlist);
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

                if (SOID == 0)
                {
                    object[] param = { SOID };
                    List<MyComplaintList> list = new List<MyComplaintList>();
                    MyComplaintList comlist;

                    var result = dbContext.Sp_MyComplaintList1_5(SOID, dtFromToday, dtToToday).ToList();

                   var data= result.Where(note => note.Projectid != 9).ToList();
                    //var result1= dbContext.Sp_MyComplaintListRemarksFinal(SOID, dtFromToday, dtToToday).ToList();


                    if (RoleID != 3)
                    {
                        var query = data.GroupBy(d => d.ComplaintID)
                                       .SelectMany(g => g.OrderByDescending(d => d.historydate)
                                     .Take(1));

                        foreach (var item in query)
                        {
                            comlist = new MyComplaintList();
                            comlist.ComplaintID = item.ComplaintID;
                            comlist.SiteCode = item.SiteCode;
                            comlist.LaunchDate = (DateTime)item.LaunchDate;
                            comlist.SiteID = item.SiteID;
                            comlist.SiteName = item.SiteName;
                            comlist.TicketNo = item.TicketNo;
                            comlist.LaunchedByName = db.SaleOfficers.Where(x => x.ID == item.LaunchedById).Select(x => x.Name).FirstOrDefault();
                            comlist.SaleOfficerName = db.SaleOfficers.Where(x => x.ID == item.LaunchedById).Select(x => x.Name).FirstOrDefault();
                            comlist.ProgressRemarks = item.ProgressStatusName + " " + "(" + item.historydate + ")";
                            comlist.InitialRemarks = item.InitialRemarks;
                            comlist.ComplaintStatus = item.StatusName;
                            comlist.FaultType = item.FaulttypeName;
                            comlist.FaultTypeDetail = item.FaulttypedetailName;
                            if (item.FaulttypedetailName == "Other")
                            {


                                comlist.FaultTypeDetail = comlist.FaultTypeDetail + "/" + item.faulttypedetailremarks;
                            }

                            if (item.ProgressStatusName == "Others")
                            {
                                // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                                comlist.ProgressRemarks = item.ProgressStatusName + "/" + item.ProgressStatusRemarks + "(" + item.historydate + ")";
                            }
                            if (item.ComplaintStatusId == 3)
                            {
                                comlist.ProgressRemarks = db.WorkDones.Where(x => x.ID == item.progressstatusid).Select(x => x.Name).FirstOrDefault() + "(" + item.historydate + ")";
                            }
                            comlist.ClientRemarks = new CommonController().GetClientRemarks((int)item.ComplaintID);
                            list.Add(comlist);
                        }


                        //foreach (var item in result)
                        //{
                        //    foreach (var items in result1)
                        //    {
                        //        if (item.ComplaintID == items.ComplaintID)
                        //        {
                        //            comlist = new MyComplaintList();


                        //            comlist.ComplaintID = item.ComplaintID;
                        //            comlist.SiteCode = item.SiteCode;
                        //            comlist.LaunchDate = (DateTime)item.LaunchDate;
                        //            comlist.SiteID = item.SiteID;
                        //            comlist.SiteName = item.SiteName;
                        //            comlist.TicketNo = item.TicketNo;
                        //            comlist.LaunchedByName = item.LaunchedByName;
                        //            comlist.SaleOfficerName = item.LaunchedByName;
                        //            if (items.ProgressStatusName == null && items.ProgressstatusId==null)
                        //            {
                        //                comlist.ProgressRemarks = " ";
                        //            }
                        //            else
                        //            {

                        //                comlist.ProgressRemarks = items.ProgressStatusName + "(" + items.datecomplete + ")";
                        //            }
                        //            comlist.InitialRemarks = item.InitialRemarks;
                        //            comlist.ComplaintStatus = item.StatusName;
                        //            comlist.FaultType = item.FaulttypeName;


                        //            if (item.ComplaintStatusId == 3)
                        //            {
                        //                comlist.ProgressRemarks = db.WorkDones.Where(x => x.ID == items.ProgressstatusId).Select(x => x.Name).FirstOrDefault() + "(" + items.datecomplete + ")"; 
                        //            }

                        //            comlist.FaultTypeDetail = item.FaulttypedetailName;

                        //            if (item.FaulttypedetailName == "Others")
                        //            {
                        //                var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).OrderByDescending(x=>x.ID).Select(x => x.ActivityType).FirstOrDefault();

                        //                comlist.FaultTypeDetail = item.FaulttypedetailName + "/" + otherremarks;

                        //            }
                        //            if (items.ProgressStatusName == "Others")
                        //            {
                        //               // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                        //                comlist.ProgressRemarks = items.ProgressStatusName + "/" + items.ProgressStatusRemarks + "(" + items.datecomplete + ")"; 
                        //            }

                        //            comlist.ClientRemarks = new CommonController().GetClientRemarks(item.ComplaintID);


                        //            list.Add(comlist);

                        //        }
                        //    }

                        //}
                    }
                    else
                    {





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
        public int? ComplaintID { get; set; }
        public string TicketNo { get; set; }
        public DateTime LaunchDate { get; set; }
        public DateTime LastDate { get; set; }
        public string SiteName { get; set; }

        public string LaunchedByName { get; set; }
        public int? SiteID { get; set; }

        public string SiteCode { get; set; }

        public string ComplaintStatus { get; set; }
        public string FaultType { get; set; }
        public string FaultTypeDetail { get; set; }
        public string FaultTypeDetailOther { get; set; }
        public string Project { get; set; }
        public string Zone { get; set; }
        public string ComplaintType { get; set; }
        public string WorkDoneStatus { get; set; }
        public string ProgressStatusName { get; set; }
        public string ProgressStatusOtherName { get; set; }
        public string SaleOfficerName { get; set; }
        public string TimeElapse { get; set; }
        public TimeSpan ElapseTime
        {
            get
            {

                return LastDate.Subtract(LaunchDate);
            }
        }
        public string InitialRemarks { get; set; }
        public List<ClientRemarks> ClientRemarks { get; set; }
        public string ProgressRemarks { get; set; }
    }

    public class BillSummaryList
    {
        public int? TotalBills { get; set; }
        public string SOName { get; set; }
        public int? NormalBills { get; set; }
        public int? NewConsumers { get; set; }
        public int? Voilations { get; set; }
    }
}