using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetProgressViewController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try

            {
                List<MyProgressStatusView> list = new List<MyProgressStatusView>();
                MyProgressStatusView comlist;
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (ID > 0)
                {
                    object[] param = { ID };

                   

                    var result = dbContext.Sp_GetProgressView1_1(ID).ToList();

                    var JobId = db.JobsDetails.Where(x => x.ID == ID).Select(x => x.JobID).FirstOrDefault();

                    var jobtblID = db.Jobs.Where(x => x.ID == JobId).Select(x => x.FaultTypeId).FirstOrDefault();

                    foreach (var item in result)
                    {
                        comlist = new MyProgressStatusView();
                        comlist.Picture1 = item.Picture1;
                        comlist.Picture2 = item.Picture2;
                        comlist.Picture3 = item.Picture3;
                        comlist.ProgressStatusID = item.ProgressStatusID;
                        comlist.JobDate = item.JobDate;
                        comlist.ProgressStatusName = item.ProgressStatusName;
                        comlist.ProgressStatusOtherRemarks = item.ProgressStatusOtherRemarks;
                        comlist.FaultTypeDetailOtherRemarks = item.FaulttypeDetailOtherRemarks;
                        comlist.PRemarks = item.PRemarks;
                        comlist.SaleOfficerName = item.SaleofficerName;
                        comlist.FaultTypeID = jobtblID;

                        list.Add(comlist);
                    }


                    if (list != null && list.Count > 0)
                    {
                        return Ok(new
                        {
                            ProgressView = list

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ProgressView GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                ProgressView = paramm
            });

        }


    }

    public class MyProgressStatusView
    {
       
        public string Picture1 { get; set; }
        public string Picture2 { get; set; }
        public string Picture3 { get; set; }
        public DateTime? JobDate { get; set; }

        public int? ProgressStatusID { get; set; }

        public string ProgressStatusName { get; set; }
        public string ProgressStatusOtherRemarks { get; set; }

        public string FaultTypeDetailOtherRemarks { get; set; }

        public string PRemarks { get; set; }
   
        public string SaleOfficerName { get; set; }

   

        public int? FaultTypeID { get; set; }
    }
}