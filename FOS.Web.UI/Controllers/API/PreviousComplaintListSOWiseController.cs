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
    public class PreviousComplaintListSOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,string DateFrom,string DateTo)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime Todate = DateTime.Parse(DateTo);
                DateTime newDate = Todate.AddDays(1);
                DateTime FromDate = DateTime.Parse(DateFrom);


                if (SOID > 0)
                {
                    List<MyComplaintList> list = new List<MyComplaintList>();
                    MyComplaintList comlist;
                    object[] param = { SOID };


                    var result = dbContext.Sp_MyPreviousComplaintList1_1(SOID, FromDate, newDate).ToList();


                    var result1 = dbContext.Sp_MyComplaintListRemarks1_2(SOID, FromDate, newDate).ToList();


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
                                comlist.FaultType = item.FaultType;

                                comlist.FaultTypeDetail = item.FaultTypeDetail;

                                if (item.FaultTypeDetail == "Others")
                                {
                                    var otherremarks = db.JobsDetails.Where(x => x.JobID == item.ComplaintID).Select(x => x.ActivityType).FirstOrDefault();

                                    comlist.FaultTypeDetail = item.FaultTypeDetail + "/" + otherremarks;
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
}