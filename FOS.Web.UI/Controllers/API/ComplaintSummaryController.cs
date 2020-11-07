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
    public class ComplaintSummaryController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get()
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
            


                MyComplaintSummary summary = new MyComplaintSummary();
                summary.TotalComplaints = Dashboard.SOPresenttoday().Count();
                summary.Resolved = Dashboard.FSPlannedtoday().Count();
                summary.InProgress = Dashboard.FSVisitedtoday().Count();
                summary.PassOn= Dashboard.RSPlannedToday().Count();
                summary.NewComplaints= Dashboard.NewComplaints().Count();
                summary.OpenComplaints = Dashboard.OpenComplaints().Count();
                summary.OpenComplaintsToday = summary.InProgress + summary.NewComplaints;


                if (summary != null )
                    {
                        return Ok(new
                        {
                            MyComplaintSummary = summary

                        });
                    }
                 
                
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "MyComplaintSummary GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                MyComplaintSummary = paramm
            });

        }


    }


    public class MyComplaintSummary
    {
        public int TotalComplaints { get; set; }


        public int NewComplaints { get; set; }

        public int Resolved { get; set; }

        public int InProgress { get; set; }

        public int PassOn { get; set; }
        public int OpenComplaintsToday { get; set; }
        public int OpenComplaints { get; set; }

    }
}