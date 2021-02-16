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
    public class PreviousOpenComplaintController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID, int SaleOfficerID)
        {

            
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);
                DateTime previous = dtFromTodayUtc.AddMonths(-2);

                var RoleID = dbContext.SaleOfficers.Where(x => x.ID == SaleOfficerID).Select(x => x.RoleID).FirstOrDefault();


                
                    List<MyComplaintList> list = new List<MyComplaintList>();
                    MyComplaintList comlist;

                    var result = dbContext.Sp_MyOpenComplaintList(SOID, previous, dtFromTodayUtc).ToList();


                    var result1 = dbContext.Sp_MyComplaintListRemarksFinal(SOID, previous, dtFromTodayUtc).ToList();


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
                                    comlist.LaunchDate = (DateTime)item.LaunchDate;
                                    comlist.SiteID = item.SiteID;
                                    comlist.SiteName = item.SiteName;
                                    comlist.TicketNo = item.TicketNo;
                                    comlist.LaunchedByName = item.LaunchedByName;
                                    comlist.SaleOfficerName = item.LaunchedByName;
                                    comlist.ProgressRemarks = items.ProgressStatusName + "(" + items.datecomplete + ")";
                                    comlist.InitialRemarks = item.InitialRemarks;
                                    comlist.ComplaintStatus = item.StatusName;
                                    comlist.FaultType = item.FaulttypeName;


                                    if (item.ComplaintStatusId == 3)
                                    {
                                        comlist.ProgressRemarks = db.WorkDones.Where(x => x.ID == items.ProgressstatusId).Select(x => x.Name).FirstOrDefault() + "(" + items.datecomplete + ")";
                                    }

                                    comlist.FaultTypeDetail = item.FaulttypedetailName;

                                    if (item.FaulttypedetailName == "Others")
                                    {
                                        var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ActivityType).FirstOrDefault();

                                        comlist.FaultTypeDetail = item.FaulttypedetailName + "/" + otherremarks;

                                    }
                                    if (items.ProgressStatusName == "Others")
                                    {
                                        // var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();

                                        comlist.ProgressRemarks = items.ProgressStatusName + "/" + items.ProgressStatusRemarks + "(" + items.datecomplete + ")";
                                    }

                                    comlist.ClientRemarks = new CommonController().GetClientRemarks(item.ComplaintID);


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
                                        comlist.LaunchDate = (DateTime)item.LaunchDate;
                                        comlist.SiteID = item.SiteID;
                                        comlist.SiteName = item.SiteName;
                                        comlist.TicketNo = item.TicketNo;
                                        comlist.LaunchedByName = item.LaunchedByName;
                                        comlist.SaleOfficerName = item.LaunchedByName;
                                        comlist.ProgressRemarks = items.ProgressStatusName + " " + "(" + items.datecomplete + ")";
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

                                            comlist.ProgressRemarks = items.ProgressStatusName + "/" + items.ProgressStatusRemarks + "(" + items.datecomplete + ")";
                                        }
                                        comlist.ClientRemarks = new CommonController().GetClientRemarks(item.ComplaintID);
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
}