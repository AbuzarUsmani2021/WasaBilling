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
    public class GetProgressListController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ComplaintID,int RoleID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<MyProgressView> list = new List<MyProgressView>();
                MyProgressView comlist;
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (ComplaintID > 0)
                {
                    object[] param = { ComplaintID };


                    var ID = db.Jobs.Where(x => x.ID == ComplaintID).Select(x => x.SaleOfficerID).FirstOrDefault();
                    if (RoleID == 2)
                    {

                        var result = dbContext.Sp_GetProgressListFinal1_1(ComplaintID).ToList();

                        foreach (var item in result)
                        {
                            comlist = new MyProgressView();
                            comlist.ID = item.ID;
                            comlist.ComplaintID = item.ComplaintID;
                            comlist.ComplaintNo = item.ComplaintNo;
                            comlist.LaunchDate = item.LaunchDate;
                            comlist.InitialRemarks = item.InitialRemarks;
                            comlist.IsPublished = item.IsPublished;
                            comlist.SiteName = item.SiteName;
                            comlist.FaultType = item.FaultType;
                            comlist.FaultTypeDetail = item.FaultTypeDetail;
                            comlist.SiteCode = item.SiteCode;
                            comlist.JobDate = item.JobDate;
                            comlist.ProgressStatusID = item.ProgressStatusID;
                            comlist.ProgressStatusName = item.ProgressStatusName;
                            comlist.PRemarks = item.PRemarks;
                            comlist.SaleofficerName = item.SaleofficerName;
                            comlist.LaunchedByName = db.SaleOfficers.Where(x=>x.ID==ID).Select(x=>x.Name).FirstOrDefault();
                            comlist.ChildFaultType = item.ChildFaultType;
                            comlist.ChildFaultTypeDetailID = item.ChildFaultTypeDetailID;
                            comlist.ChildComplaintStatusID = item.ChildComplaintStatusID;
                            comlist.ChildFaulttypeName =  db.FaultTypes.Where(x => x.Id == item.ChildFaultType).Select(x => x.Name).FirstOrDefault();
                            comlist.ChildFaultTypeDetailName =  db.FaultTypeDetails.Where(x => x.ID == item.ChildFaultTypeDetailID ).Select(x => x.Name).FirstOrDefault();


                            if (comlist.ChildFaultTypeDetailID == 3042 || comlist.ChildFaultTypeDetailID == 3030 || comlist.ChildFaultTypeDetailID == 3049)
                            {
                                comlist.ChildFaultTypeDetailName = comlist.ChildFaultTypeDetailName + " /" + item.faultTypeDetailRemarks;
                            }

                            comlist.ChildComplaintStatus = item.ChildComplaintStatus;

                            if (item.ChildComplaintStatusID == 3)
                            {
                                comlist.ProgressStatusName = db.WorkDones.Where(x => x.ID == item.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                            }

                            list.Add(comlist);

                        }

                        if (list != null && list.Count > 0)
                        {
                            return Ok(new
                            {
                                ProgressList = list

                            });
                        }
                    }
                    else
                    {
                        var result = dbContext.Sp_GetProgressList1_4Final(ComplaintID).ToList();
                        foreach (var item in result)
                        {
                            comlist = new MyProgressView();
                            comlist.ID = item.ID;
                            comlist.ComplaintID = item.ComplaintID;
                            comlist.ComplaintNo = item.ComplaintNo;
                            comlist.LaunchDate = item.LaunchDate;
                            comlist.InitialRemarks = item.InitialRemarks;
                            comlist.IsPublished = item.IsPublished;
                            comlist.SiteName = item.SiteName;
                            comlist.FaultType = item.FaultType;
                            comlist.FaultTypeDetail = item.FaultTypeDetail;
                            comlist.SiteCode = item.SiteCode;
                            comlist.JobDate = item.JobDate;
                            comlist.ProgressStatusID = item.ProgressStatusID;
                            comlist.ProgressStatusName = item.ProgressStatusName;
                            comlist.PRemarks = item.PRemarks;
                            comlist.SaleofficerName = item.SaleofficerName;
                            comlist.LaunchedByName = db.SaleOfficers.Where(x => x.ID == ID).Select(x => x.Name).FirstOrDefault();
                            comlist.ChildFaultType = item.ChildFaultType;
                            comlist.ChildFaultTypeDetailID = item.ChildFaultTypeDetailID;
                            comlist.ChildComplaintStatusID = item.ChildComplaintStatusID;
                            comlist.ChildFaulttypeName = db.FaultTypes.Where(x => x.Id == item.ChildFaultType).Select(x => x.Name).FirstOrDefault();
                            comlist.ChildFaultTypeDetailName = db.FaultTypeDetails.Where(x => x.ID == item.ChildFaultTypeDetailID).Select(x => x.Name).FirstOrDefault();


                            if (comlist.ChildFaultTypeDetailID == 3042 || comlist.ChildFaultTypeDetailID == 3030 || comlist.ChildFaultTypeDetailID == 3049)
                            {
                                comlist.ChildFaultTypeDetailName = comlist.ChildFaultTypeDetailName + " /" + item.faultTypeDetailRemarks;
                            }

                            comlist.ChildComplaintStatus = item.ChildComplaintStatus;

                            if (item.ChildComplaintStatusID == 3)
                            {
                                comlist.ProgressStatusName = db.WorkDones.Where(x => x.ID == item.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                            }

                            list.Add(comlist);

                        }

                        if (list != null && list.Count > 0)
                        {
                            return Ok(new
                            {
                                ProgressList = list

                            });
                        }

                    }

                   

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ProgressList GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                ProgressList = paramm
            });

        }


    }


    public class MyProgressView
    {
        public int? ID { get; set; }
        public int? ComplaintID { get; set; }
   
        public string ComplaintNo { get; set; }
        public DateTime? LaunchDate { get; set; }
        public string InitialRemarks { get; set; }
        public int? IsPublished { get; set; }
        public string SiteName { get; set; }


        public string FaultType { get; set; }
        public string FaultTypeDetail { get; set; }
        public string SiteCode { get; set; }
        public DateTime? JobDate { get; set; }
        public int? ProgressStatusID { get; set; }
        public string ProgressStatusName { get; set; }
        public string PRemarks { get; set; }
        public string SaleofficerName { get; set; }
        public string LaunchedByName { get; set; }
        public int? ChildFaultType { get; set; }
        public int? ChildFaultTypeDetailID { get; set; }
        public int? ChildComplaintStatusID { get; set; }
        public string ChildFaulttypeName { get; set; }
        public string ChildFaultTypeDetailName { get; set; }
        public string ChildComplaintStatus { get; set; }
    }
}