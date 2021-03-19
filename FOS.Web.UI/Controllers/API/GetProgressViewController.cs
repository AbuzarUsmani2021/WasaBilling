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

                   

                    var result = dbContext.Sp_GetProgressView1_3(ID).ToList();

                    var JobId = db.JobsDetails.Where(x => x.ID == ID).FirstOrDefault();

                    var jobtblID = db.Jobs.Where(x => x.ID == JobId.JobID).FirstOrDefault();

                    foreach (var item in result)
                    {
                        comlist = new MyProgressStatusView();
                        comlist.Picture1 = item.Picture1;
                        comlist.Picture2 = item.Picture2;
                        comlist.Picture3 = item.Picture3;
                        comlist.Video = item.Video;
                        comlist.Audio = item.Audio;
                        //comlist.ComplaintID = jobtblID.TicketNo;
                        comlist.TicketNo = jobtblID.TicketNo;
                        comlist.ProgressStatusID = item.ProgressStatusID;
                        comlist.LaunchDate = item.JobDate;
                        comlist.ProgressStatusName = item.ProgressStatusName;
                        comlist.ProgressStatusRemarks = item.ProgressStatusOtherRemarks;
                        comlist.FaultTypeDetailOtherRemarks = item.FaultTypeDetailOtherRemarks;
                        if (comlist.FaultTypeDetailOtherRemarks == "Others")
                        {
                            comlist.FaultTypeDetailOtherRemarks = comlist.FaultTypeDetailOtherRemarks + " /" + item.faultTypeDetailRemarks;
                        }
                        comlist.ProgressRemarks = item.PRemarks;
                        comlist.SaleOfficerName = item.SaleofficerName;
                        comlist.FaultTypeId = item.FaultTypeId;
                        comlist.ResolutionHour = jobtblID.ResolvedHours;
                        comlist.FaultTypeDetailID = item.FaultTypeDetailID;
                        comlist.PriorityId = jobtblID.PriorityId;
                        comlist.ComplainttypeID = jobtblID.ComplainttypeID;
                        comlist.AssignedSaleOfficerID = JobId.AssignedToSaleOfficer;
                        comlist.PersonName = jobtblID.PersonName;
                        comlist.IsPublished = JobId.IsPublished;
                        comlist.ComplaintStatusID = item.ComplaintStatusID;
                        comlist.SiteID = JobId.RetailerID;
                        comlist.SiteCode = JobId.Retailer.RetailerCode;
                        comlist.SiteName = JobId.Retailer.Name;
                        comlist.AudioDate = JobId.AudioDate;
                        comlist.VideoDate = JobId.VideoDate;
                        if (item.ComplaintStatusID == 3)
                        {
                            comlist.ProgressStatusName = db.WorkDones.Where(x => x.ID == item.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                        }
                        //comlist.ComplaintStatusName = db.ComplaintStatus.Where(x => x.Id == jobtblID.ComplaintStatusId).Select(x => x.Name).FirstOrDefault();
                        //// comlist.FaultTypeName = db.FaultTypes.Where(x => x.Id == jobtblID.FaultTypeId).Select(x => x.Name).FirstOrDefault();
                        //comlist.FaultTypeDetailName = db.FaultTypeDetails.Where(x => x.ID == jobtblID.FaultTypeDetailID).Select(x => x.Name).FirstOrDefault();
                        list.Add(comlist);
                    }


                    if (list != null && list.Count > 0)
                    {
                        return Ok(new
                        {
                            ProgressViewEdit = list

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
                ProgressViewEdit = paramm
            });

        }


    }

    public class MyProgressStatusView
    {

        public int ComplaintID { get; set; }
        public int? ResolutionHour { get; set; }
        public int? AssignedSaleOfficerID { get; set; }
        public string TicketNo { get; set; }
        public string Picture1 { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public string Picture2 { get; set; }
        public string Picture3 { get; set; }
        public string AssignedSaleOfficerName { get; set; }
        public DateTime? LaunchDate { get; set; }
        public DateTime? AudioDate { get; set; }
        public DateTime? VideoDate { get; set; }
        public string SiteName { get; set; }
        public string ProgressStatusName { get; set; }
        public string ProgressStatusRemarks { get; set; }
        public string LaunchedByName { get; set; }
        public int? SiteID { get; set; }
        public int? ProgressStatusID { get; set; }
        public int? SaleOfficerID { get; set; }
        public string SiteCode { get; set; }
        public int? FaultTypeId { get; set; }
       
        public int? PriorityId { get; set; }
        public int? ComplaintStatusId { get; set; }
        public int? FaultTypeDetailID { get; set; }
        public int? ComplainttypeID { get; set; }
        public int? ComplaintStatusID { get; set; }
        public int? IsPublished { get; set; }
        public string PriorityName { get; set; }
        public string ComplainttypeName { get; set; }
        public string PersonName { get; set; }
        public string ComplaintStatusName { get; set; }
        public string FaulttypeName { get; set; }
        public string FaultTypeDetailName { get; set; }
        public string FaultTypeDetailOtherRemarks { get; set; }
        public string SaleOfficerName { get; set; }

        public string InitialRemarks { get; set; }

        public string ProgressRemarks { get; set; }
    }
}