using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FOS.Web.UI.Controllers
{
    public class ComplaintController : Controller
    {
        FOSDataModel db = new FOSDataModel();

        #region Complaints THINGS

        [CustomAuthorize]
        //View
        public ActionResult NewComplaint()
        {
            List<RegionData> regionalHeadData = new List<RegionData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetRegionalList();
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetProjectsData();
            //var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            var objRetailer = new KSBComplaintData();
            objRetailer.Client = regionalHeadData;
            var userID = Convert.ToInt32(Session["UserID"]);

            if (userID == 1025)
            {
                objRetailer.Projects = FOS.Setup.ManageCity.GetProjectsList();
            }
            else if (userID == 1026 || userID == 1027)
            {
                var soid = db.Users.Where(x => x.ID == userID).Select(x => x.SOIDRelation).FirstOrDefault();

                var list = db.SOProjects.Where(x => x.SaleOfficerID == soid).Select(x => x.ProjectID).Distinct().ToList();

                var Projectlist = FOS.Setup.ManageCity.GetProjectsListForUsers(list);
                objRetailer.Projects = Projectlist;
            }
            else
            {
                objRetailer.Projects = FOS.Setup.ManageCity.GetProjectsList();
            }
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
            objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
            objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
            objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
            objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
            objRetailer.Sites = FOS.Setup.ManageCity.GetSitesList();

            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();

            return View(objRetailer);
        }





        // Add Or Update Retailer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewComplaint([Bind(Exclude = "TID,SaleOfficers,Dealers")] KSBComplaintData newRetailer, HttpPostedFileBase Picture1, HttpPostedFileBase Picture2)
        {

            FOSDataModel dbContext = new FOSDataModel();
            newRetailer.StatusID = 2003;
            newRetailer.SaleOfficerID= (int) Session["SORelationID"];
            var TeamID = (int)Session["TeamID"];


            string path1 = "";
            string path2 = "";

            if (Picture1 != null)
            {
                var filename = Path.GetFileName(Picture1.FileName);
                path1 = Path.Combine(Server.MapPath("/Images/ComplaintImages/"), filename);
                Picture1.SaveAs(path1);
                path1 = "/Images/ComplaintImages/" + filename;
            }
            if (Picture2 != null)
            {
                var filename = Path.GetFileName(Picture2.FileName);
                path2 = Path.Combine(Server.MapPath("~/Images/ComplaintImages/"), filename);
                Picture2.SaveAs(path2);
                path2 = "/Images/ComplaintImages/" + filename;
            }
            int Res = ManageRetailer.AddUpdateComplaint(newRetailer, path1, path2);

            if (TeamID == 4)
            {
                return RedirectToAction("WASADashboard","Home");
            }
            else 
            {
                return RedirectToAction("Home", "Home");
            }



        }

        #endregion


        [CustomAuthorize]
        public ActionResult UpdateComplaints()
        {
            List<RegionData> regionalHeadData = new List<RegionData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetRegionalList();
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetProjectsData();
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            var objRetailer = new KSBComplaintData();
            objRetailer.Client = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
            objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
            objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
            objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
            objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();

            return View(objRetailer);
        }

        public JsonResult GetSitesList(int ClientID, int ProjectID, int CityID, int AreaID, int SubDivisionID)
        {
            var result = FOS.Setup.ManageCity.GetSiteList(ClientID, ProjectID, CityID, AreaID, SubDivisionID, "--Select Site--");
            return Json(result);
        }


        public JsonResult GetFaultTypeDetailList(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetFaulttypedetaileList(ClientID, "--Select Fault Type Detail--");
            return Json(result);
        }

        public JsonResult WorkDoneDetailList(int ClientID)
        {
            var result = FOS.Setup.ManageCity.WorkDoneDetailList(ClientID, "--Select Work Done Status--");
            return Json(result);
        }

        public JsonResult GetProgressDetailList(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetProgressDetailListForReport(ClientID, "--Select Progress Status--");
            return Json(result);
        }

        public JsonResult GetProgressDetailListForReport(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetProgressDetailListForReport(ClientID, "--Select Progress Status--");
            return Json(result);
        }

        public JsonResult GetUpdateComplaint(int ComplaintID)
        {
            var Response = ManageJobs.GetUpdateComplaint(ComplaintID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComplaintClientRemarks(int ComplaintId)
        {
            var Response = ManageJobs.GetComplaintClientRemarks(ComplaintId);
            return Json(Response);
        }


        public JsonResult GetSiteId(int ClientID)
        {   
            var result = FOS.Setup.ManageCity.GetSiteIDList(ClientID);

            return Json(result);
        }

        public JsonResult GetComplaintChildData(DTParameters param, int ComplaintId)
        {
            var dtsource = new List<ComplaintProgress>();
            dtsource = ManageJobs.GetComplaintChildData(ComplaintId);
            foreach (var itm in dtsource)
            {
                if (itm.FaultTypeDetailName == "Others")
                {
                    itm.FaultTypeDetailName ="Others/" + itm.FaultTypeDetailRemarks;
                }
                if (itm.ComplaintStatus == "Resolved")
                {
                    itm.ProgressStatusName = db.WorkDones.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                else if (itm.ComplaintStatus == null)
                {
                    itm.ComplaintStatus = "New Complaint";
                }
                else
                {
                    itm.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                if (itm.ProgressStatusName == "Others")
                {
                    itm.ProgressStatusName ="Others/" + itm.ProgressStatusName;
                }
                if (itm.ProgressRemarks == null || itm.ProgressRemarks == "")
                {
                    itm.ProgressRemarks = "null";
                }
                if (itm.ProgressStatusName == null)
                {
                    itm.ProgressStatusName = "null";
                }
            }
            return Json(dtsource);

        }
        public JsonResult GetComplaintChildDataForKSB(DTParameters param, int ComplaintId)
        {
            var dtsource = new List<ComplaintProgress>();
            dtsource = ManageJobs.GetComplaintChildDataForKSB(ComplaintId);
            foreach (var itm in dtsource)
            {
                if (itm.FaultTypeDetailName == "Others")
                {
                    itm.FaultTypeDetailName = "Others/" + itm.FaultTypeDetailRemarks;
                }
                if (itm.ComplaintStatus == "Resolved")
                {
                    itm.ProgressStatusName = db.WorkDones.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                else if (itm.ComplaintStatus == null)
                {
                    itm.ComplaintStatus = "New Complaint";
                }
                else
                {
                    itm.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                if (itm.ProgressStatusName == "Others")
                {
                    itm.ProgressStatusName = "Others/" + itm.ProgressStatusName;
                }
                if (itm.ProgressRemarks == null || itm.ProgressRemarks == "")
                {
                    itm.ProgressRemarks = "null";
                }
                if (itm.ProgressStatusName == null)
                {
                    itm.ProgressStatusName = "null";
                }
            }
            return Json(dtsource);

        }

        public JsonResult GetProgressIDData(int ProgressID)
        {
            var Response = ManageJobs.GetProgressIDData(ProgressID);
            if (Response.FaultTypeDetailName == "Others")
            {
                Response.FaultTypeDetailName ="Others/" +Response.FaultTypeDetailRemarks;
            }
            if (Response.ComplaintStatus == "Resolved")
            {
                Response.ProgressStatusName = db.WorkDones.Where(x => x.ID == Response.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
            }
            else if (Response.ComplaintStatus == null)
            {
                Response.ComplaintStatus = "New Complaint";
            }
            else
            {
                Response.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == Response.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
            }
            if (Response.ProgressStatusName == "Others")
            {
                Response.ProgressStatusName ="Others/" +  Response.ProgressStatusName;
            }
            if (Response.ProgressStatusName ==null)
            {
                Response.ProgressStatusName = "null";
            }
            if (Response.ProgressRemarks == null || Response.ProgressRemarks == "")
            {
                Response.ProgressRemarks = "null";
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChatBoxData(int ComplaintID)
        {
            List<CityData> Response = null;
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                var SiteID = dbContext.Jobs.Where(x => x.ID == ComplaintID).Select(x => x.SiteID).FirstOrDefault();
                var SiteName = dbContext.Retailers.Where(x => x.ID == SiteID).FirstOrDefault();

                Response = dbContext.ChatBoxes.Where(a => a.ComplaintID == ComplaintID).Select(a => new CityData
                {
                    ID = a.ID,
                    Remarks = a.Remarks,
                    ComplaintID = a.ComplaintID,
                    ShopName = a.DateInstall.ToString(),
                    SOID = a.SOID,
                    SaleOfficerName = dbContext.SaleOfficers.Where(x => x.ID == a.SOID).Select(x => x.Name).FirstOrDefault(),
                    SiteName = SiteName.Name + "(" + SiteName.RetailerCode + ")",

                }).ToList();
            }
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostSMS(ChatBox SMSData)
        {
            var Response = true;
            var job = db.Jobs.Where(x => x.ID == SMSData.ComplaintID).FirstOrDefault();
            ChatBox retailerObj = new ChatBox();
            retailerObj.ComplaintID = SMSData.ComplaintID;
            retailerObj.Remarks = SMSData.Remarks;
            retailerObj.DateInstall = DateTime.UtcNow.AddHours(5);
            retailerObj.SOID =SMSData.SOID;
            db.ChatBoxes.Add(retailerObj);
            db.SaveChanges();
            string type = "SMS";
            string message = "There is a new message in Complaint No  " + job.TicketNo + " Kindly Visit it ";

            if (job.ZoneID != 9)
            {

                var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                List<string> list = new List<string>();

                foreach (var item in SOIds)
                {
                    var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                    if (id != null)
                    {
                        foreach (var items in id)
                        {
                            var result = new ComplaintController().PushNotificationForEdit(message, items, (int)SMSData.ComplaintID, type);
                        }
                    }
                }



            }
            else
            {
                // Notification For Progressive Management
                var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 6 && x.RoleID == 2).Select(x => x.ID).ToList();
                List<string> list1 = new List<string>();
                foreach (var item in SOIdss)
                {
                    var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                    if (id != null)
                    {
                        foreach (var items in id)
                        {
                            var result = new ComplaintController().PushNotificationForEdit(message, items, job.ID, type);
                        }
                    }
                    //if (list1 != null)
                    //{
                    //    var result = new CommonController().PushNotification(message, list1, rm.ComplaintID, type);
                    //}
                }
            }

            return Json(Response);
        }



        public JsonResult DeleteProgressIDData(int ProgressID)
        {

            int? JobDetailID=  db.Tbl_ComplaintHistory.Where(x => x.ID == ProgressID).Select(x => x.JobDetailID).FirstOrDefault();
            var NotificationSeen = db.NotificationSeens.Where(x => x.JobDetailID == JobDetailID).ToList();
            foreach (var item in NotificationSeen)
            {
                db.NotificationSeens.Remove(item);
            }
            var ComplaintNotification = db.ComplaintNotifications.Where(x => x.JobDetailID== JobDetailID).FirstOrDefault();
            db.ComplaintNotifications.Remove(ComplaintNotification);
            var JobDetailRecord = db.JobsDetails.Where(x => x.ID == JobDetailID).FirstOrDefault();
            db.JobsDetails.Remove(JobDetailRecord);
            var Tbl_ComplaintHistoryRecord = db.Tbl_ComplaintHistory.Where(x => x.ID == ProgressID).FirstOrDefault();
            db.Tbl_ComplaintHistory.Remove(Tbl_ComplaintHistoryRecord);
            var Result = true;
            db.SaveChanges();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProgressPublish(int ProgressID)
        {
            var Result = true;
            var jobDetail = new JobsDetail();
            var job = new Job();
            var jobhistory = new Tbl_ComplaintHistory();
            
                int flag = 0;
                jobhistory = db.Tbl_ComplaintHistory.Where(u => u.ID == ProgressID).FirstOrDefault();
                if (jobhistory.IsPublished != 1)
                {
                    jobhistory.IsPublished = 1;
                    flag = 1;
                }
                jobDetail = db.JobsDetails.Where(u => u.ID == jobhistory.JobDetailID).FirstOrDefault();
                jobDetail.IsPublished =1;
                jobDetail.ProgressStatusID = jobhistory.ProgressStatusID;
                jobDetail.AssignedToSaleOfficer = jobhistory.AssignedToSaleOfficer;
                jobDetail.ProgressStatusRemarks = jobhistory.ProgressStatusRemarks;
                jobDetail.ActivityType = jobhistory.FaultTypeDetailRemarks;
                jobDetail.DateComplete = DateTime.UtcNow.AddHours(5);
                job = db.Jobs.Where(u => u.ID == jobhistory.JobID).FirstOrDefault();
                job.FaultTypeId = jobhistory.FaultTypeId;
                job.FaultTypeDetailID = jobhistory.FaultTypeDetailID;
                job.ComplaintStatusId = jobhistory.ComplaintStatusId;
                job.ResolvedAt = DateTime.UtcNow.AddHours(5);
                job.LastUpdated = DateTime.UtcNow.AddHours(5);


                db.SaveChanges();
                if (flag == 1)

                {
                    // Notification Send to KSB CC
                    string type = "Progress";
                    string message = "There is an Update Which is Published By CC in Complaint No" + job.TicketNo + " Kindly View it.";
                    if (job.ZoneID != 9)
                    {
                        var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                        List<string> list = new List<string>();
                        foreach (var item in SOIds)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list.Add(items);
                                }
                            }
                        }

                        if (list != null)
                        {
                            var result = new ComplaintController().PushNotification(message, list, job.ID, type);
                        }

                        // Notification For KSB Management
                        var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Select(x => x.ID).ToList();
                        List<string> list1 = new List<string>();
                        foreach (var item in SOIdss)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list1.Add(items);
                                }
                            }
                        }

                        if (list1 != null)
                        {
                            var result = new ComplaintController().PushNotification(message, list1, job.ID, type);
                        }

                        // Notification Send to Wasa

                        var AreaID = Convert.ToInt32(job.Areas);

                        var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == job.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        List<string> list2 = new List<string>();
                        foreach (var item in IdsforWasa)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list2.Add(items);
                                }
                            }
                        }
                        if (list2 != null)
                        {
                            var result2 = new ComplaintController().PushNotificationForWasa(message, list2, job.ID, type);
                        }
                    }
                    else
                    {
                        // Notification For Progressive Management
                        var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 6 && x.RoleID == 2).Select(x => x.ID).ToList();
                        List<string> list1 = new List<string>();
                        foreach (var item in SOIdss)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list1.Add(items);
                                }
                            }
                            if (list1 != null)
                            {
                                var result = new ComplaintController().PushNotification(message, list1, job.ID, type);
                            }
                        }


                        var AreaID = Convert.ToInt32(job.Areas);

                        var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == job.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        List<string> list2 = new List<string>();
                        foreach (var item in IdsforWasa)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list2.Add(items);
                                }
                            }
                        }
                        if (list2 != null)
                        {
                            var result2 = new ComplaintController().PushNotificationForWasa(message, list2, job.ID, type);
                        }
                    }
                

             
            }
            return Json(Result, JsonRequestBehavior.AllowGet);


        }
        public bool PushNotification(string Message, List<string> deviceIDs, int ID, string type)
        {
            var AppId = ConfigurationManager.AppSettings["OneSignalAppID"];

            var DevIDs = deviceIDs;
            foreach (var item in DevIDs)
            {
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    app_id = AppId,
                    contents = new { en = Message },
                    data = new { ComplaintID = ID, PushType = type },
                    include_player_ids = new string[] { item }
                };



                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;

                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                    return false;
                }

                System.Diagnostics.Debug.WriteLine(responseContent);



            }

            return true;

        }
        public bool PushNotificationForEdit(string Message, string deviceIDs, int ID, string type)
        {
            var AppId = ConfigurationManager.AppSettings["OneSignalAppID"];

            var DevIDs = deviceIDs;

            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = AppId,
                contents = new { en = Message },
                data = new { ComplaintID = ID, PushType = type },
                include_player_ids = new string[] { DevIDs }
            };



            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                return false;
            }

            System.Diagnostics.Debug.WriteLine(responseContent);





            return true;

        }
        public bool PushNotificationForWasa(string Message, List<string> deviceIDs, int ID, string type)
        {
            var AppId = ConfigurationManager.AppSettings["OneSignalAppIDForWasa"];

            var DevIDs = deviceIDs;
            foreach (var item in DevIDs)
            {
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    app_id = AppId,
                    contents = new { en = Message },
                    data = new { ComplaintID = ID, PushType = type },
                    include_player_ids = new string[] { item }
                };



                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;

                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                    return false;
                }

                System.Diagnostics.Debug.WriteLine(responseContent);



            }

            return true;

        }


        public JsonResult GetCurrentComplaintDetail(int ComplaintId)
        {
            var Response = ManageRetailer.GetCurrentComplaintDetail(ComplaintId);
            //Response.LastUpdated = Convert.ToString(Response.ResolvedAt);
            if (Response.FaultTypesDetailName == "Others")
            {
                Response.FaultTypesDetailName = "Others/" + Response.FaultTypeDetailOtherRemarks;
            }
            Response.ProgressStatusId = db.JobsDetails.Where(x => x.JobID == Response.ID).OrderByDescending(x => x.ID).Select(x => x.ProgressStatusID).FirstOrDefault();
            Response.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == Response.ProgressStatusId).OrderByDescending(x => x.ID).Select(x => x.Name).FirstOrDefault();
            if (Response.StatusName == "Resolved")
            {
                Response.ProgressStatusName = db.WorkDones.Where(x => x.ID == Response.ProgressStatusId).Select(x => x.Name).FirstOrDefault();

            }
            else
            {
                Response.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == Response.ProgressStatusId).Select(x => x.Name).FirstOrDefault();

            }
            if (Response.ProgressStatusName == "Others")
            {
                Response.ProgressStatusName = "Other/" + db.JobsDetails.Where(x => x.JobID == Response.ID).OrderByDescending(x => x.ID).Select(x => x.PRemarks).FirstOrDefault();
            }
            if (Response.ProgressStatusName == "" || Response.ProgressStatusName ==null || Response.ProgressStatusId==null)
            {
                Response.ProgressStatusName =  "null";
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetActualComplaintDetail(int ComplaintId)
        {
            var Response = ManageRetailer.GetActualComplaintDetail(ComplaintId);
            if (Response.FaultTypesDetailName == "Others")
            {
                Response.FaultTypesDetailName ="Others/"+Response.FaultTypeDetailOtherRemarks;
            }
            if (Response.Name == null || Response.Name =="")
            {
                Response.Name = "null";
            }
            if (Response.Remarks == null || Response.Remarks == "")
            {
                Response.Remarks = "null";
            }
            if (Response.FaultTypesDetailName == null || Response.FaultTypesDetailName == "")
            {
                Response.FaultTypesDetailName = "null";
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEditComplaint(int ComplaintId)
        {
            //var Response = ManageRetailer.GetEditComplaint(ComplaintId);
            
            return Json(Response, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateComplaints(JobsData newRetailer)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRetailer != null)
                {
                    if (newRetailer.ID == 0)
                    {
                        //ComplaintValidator validator = new ComplaintValidator();
                        //results = validator.Validate(newRetailer);
                        //boolFlag = results.IsValid;
                    }

                    //if (newRetailer.Phone1 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumber1Exist(newRetailer.ID, newRetailer.Phone1 == null ? "" : newRetailer.Phone1) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newRetailer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumber2Exist(newRetailer.ID, newRetailer.Phone2 == null ? "" : newRetailer.Phone2) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newRetailer.Phone1 != null && newRetailer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumberExist(newRetailer.ID, newRetailer.Phone1 == null ? "" : newRetailer.Phone1, newRetailer.Phone2 == null ? "" : newRetailer.Phone2) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckCNICExist(newRetailer.CNIC, newRetailer.ID) == 1)
                    //{
                    //    return Content("3");
                    //}
                    //else
                    //{
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckAccountNoExist(newRetailer.AccountNo, newRetailer.ID) == 1)
                    //{
                    //    return Content("4");
                    //}
                    //else
                    //{
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckCardNoExist(newRetailer.CardNumber, newRetailer.ID) == 1)
                    //{
                    //    return Content("5");
                    //}
                    //else
                    //{
                    //}

                    if (boolFlag)
                    {
                        //try
                        //{

                        //    newRetailer.CreatedBy = SessionManager.Get<int>("UserID");
                        //    newRetailer.UpdatedBy = SessionManager.Get<int>("UserID");

                        //}
                        //catch { newRetailer.CreatedBy = 1; }

                        //  int Res = ManageRetailer.AddUpdateComplaint(newRetailer);
                        int Res = 1;
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else if (Res == 4)
                        {
                            return Content("4");
                        }
                        else if (Res == 5)
                        {
                            return Content("5");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }

                }
                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }



        #region OpenComplaints

        public ActionResult OpenComplaints()
        {
            List<RegionData> regionalHeadData = new List<RegionData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetRegionalList();
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetProjectsData();
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            var objRetailer = new KSBComplaintData();
            objRetailer.Client = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
            objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
            objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
            objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
            objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();

            return View(objRetailer);
        }


        #endregion



    }
}