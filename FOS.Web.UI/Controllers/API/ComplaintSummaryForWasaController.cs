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
    public class ComplaintSummaryForWasaController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ProjectID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                var result = db.Jobs.Where(x => x.ZoneID == ProjectID && x.CreatedDate>=dtFromToday && x.CreatedDate<=dtToToday).ToList();
            


                MyComplaintSummary summary = new MyComplaintSummary();
                summary.TotalComplaints = result.Count();
                summary.Resolved = result.Where(x => x.ComplaintStatusId == 3).Count();
                summary.InProgress = result.Where(x => x.ComplaintStatusId == 4).Count();
                summary.PassOn= result.Where(x => x.ComplaintStatusId == 1003).Count();
                summary.NewComplaints= result.Where(x => x.ComplaintStatusId == 2003).Count();
                summary.OpenComplaints = db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.CreatedDate<= dtFromToday).Count();
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



}