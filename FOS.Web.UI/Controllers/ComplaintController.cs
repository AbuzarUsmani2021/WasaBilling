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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

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
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();

            return View(objRetailer);
        }





        // Add Or Update Retailer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewComplaint([Bind(Exclude = "TID,SaleOfficers,Dealers")] KSBComplaintData newRetailer, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4, HttpPostedFileBase file5, HttpPostedFileBase file6)
        {

            Boolean boolFlag = true;
            FOSDataModel dbContext = new FOSDataModel();

            int id = dbContext.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.ID).FirstOrDefault();
            newRetailer.LaunchedByID = id;
            ////newRetailer.ComplaintTypeID = 0;
            ////newRetailer.PriorityId = 0;
            newRetailer.StatusID = 2003;





            string path1 = "";
            string path2 = "";
            string path3 = "";
            string path4 = "";
            string path5 = "";
            string path6 = "";
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRetailer != null)
                {
                    //if (newRetailer.ID == 0)
                    //{
                    //    //ComplaintValidator validator = new ComplaintValidator();
                    //    //results = validator.Validate(newRetailer);
                    //    //boolFlag = results.IsValid;
                    //}





                    if (boolFlag)
                    {

                        if (file1 != null)
                        {
                            var filename = Path.GetFileName(file1.FileName);
                            path1 = Path.Combine(Server.MapPath("/Images/ComplaintImages/"), filename);
                            file1.SaveAs(path1);
                            path1 = "/images/ComplaintImages/" + filename;

                        }

                        if (file2 != null)
                        {
                            var filename = Path.GetFileName(file2.FileName);
                            path2 = Path.Combine(Server.MapPath("~/Images/ComplaintImages/"), filename);
                            file2.SaveAs(path2);
                            path2 = "/images/ComplaintImages/" + filename;

                        }
                        if (file3 != null)
                        {
                            var filename = Path.GetFileName(file3.FileName);
                            path3 = Path.Combine(Server.MapPath("~/Images/ComplaintImages/"), filename);
                            file3.SaveAs(path3);
                            path3 = "/images/ComplaintImages/" + filename;

                        }
                        if (file4 != null)
                        {
                            var filename = Path.GetFileName(file4.FileName);
                            path4 = Path.Combine(Server.MapPath("~/Images/ComplaintImages/"), filename);
                            file4.SaveAs(path4);
                            path4 = "/images/ComplaintImages/" + filename;

                        }
                        if (file5 != null)
                        {
                            var filename = Path.GetFileName(file5.FileName);
                            path5 = Path.Combine(Server.MapPath("~/Images/ComplaintImages/"), filename);
                            file5.SaveAs(path5);
                            path5 = "/images/ComplaintImages/" + filename;

                        }
                        if (file6 != null)
                        {
                            var filename = Path.GetFileName(file6.FileName);
                            path6 = Path.Combine(Server.MapPath("~/Images/ComplaintVideos/"), filename);
                            file6.SaveAs(path6);
                            path6 = "/images/ComplaintImages/" + filename;

                        }

                        int Res = ManageRetailer.AddUpdateComplaint(newRetailer, path1, path2, path3, path4, path5, path6);

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

        public JsonResult GetProgressDetailList(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetProgressDetailList(ClientID, "Select");
            return Json(result);
        }

        public JsonResult GetUpdateComplaint(int ComplaintID)
        {
            var Response = ManageJobs.GetUpdateComplaint(ComplaintID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSiteId(int ClientID)
        {   
            var result = FOS.Setup.ManageCity.GetSiteIDList(ClientID);

            return Json(result);
        }

        public JsonResult GetComplaintProgressData(DTParameters param, int ComplaintId)
        {
            var dtsource = new List<ComplaintProgress>();
            dtsource = ManageJobs.GetComplaintProgressData(ComplaintId);
            foreach (var itm in dtsource)
            {
                if (itm.FaultTypeDetailID == 3030 || itm.FaultTypeDetailID == 3042 || itm.FaultTypeDetailID == 3049)
                {
                    itm.FaultTypeDetailName = db.JobsDetails.Where(x => x.JobID == itm.ComplaintID).Select(x => x.PRemarks).FirstOrDefault();
                }
                else if (itm.FaultTypeDetailID == null && itm.FaultTypeID == null)
                {
                    itm.FaultTypeID = db.Jobs.Where(x => x.ID == itm.ComplaintID).Select(x => x.FaultTypeId).FirstOrDefault();
                    itm.FaultTypeName = db.FaultTypes.Where(x => x.Id == itm.FaultTypeID).Select(x => x.Name).FirstOrDefault();
                    itm.FaultTypeDetailID = db.Jobs.Where(x => x.ID == itm.ComplaintID).Select(x => x.FaultTypeDetailID).FirstOrDefault();
                    itm.FaultTypeDetailName = db.FaultTypeDetails.Where(x => x.ID == itm.FaultTypeDetailID).Select(x => x.Name).FirstOrDefault();
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
                    itm.ProgressStatusName = db.JobsDetails.Where(x => x.JobID == itm.ComplaintID).Select(x => x.PRemarks).FirstOrDefault();
                }
            }
            return Json(dtsource);

        }

        public JsonResult GetProgressIDData(int ProgressID)
        {
            var Response = ManageJobs.GetProgressIDData(ProgressID);
            if (Response.FaultTypeDetailID == 3030 || Response.FaultTypeDetailID == 3042 || Response.FaultTypeDetailID == 3049)
            {
                Response.FaultTypeDetailName = db.JobsDetails.Where(x => x.JobID == Response.ComplaintID).Select(x => x.PRemarks).FirstOrDefault();
            }
            else if (Response.FaultTypeDetailID == null && Response.FaultTypeID == null)
            {
                Response.FaultTypeID = db.Jobs.Where(x => x.ID == Response.ComplaintID).Select(x => x.FaultTypeId).FirstOrDefault();
                Response.FaultTypeName = db.FaultTypes.Where(x => x.Id == Response.FaultTypeID).Select(x => x.Name).FirstOrDefault();
                Response.FaultTypeDetailID = db.Jobs.Where(x => x.ID == Response.ComplaintID).Select(x => x.FaultTypeDetailID).FirstOrDefault();
                Response.FaultTypeDetailName = db.FaultTypeDetails.Where(x => x.ID == Response.FaultTypeDetailID).Select(x => x.Name).FirstOrDefault();
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
                Response.ProgressStatusName = db.JobsDetails.Where(x => x.JobID == Response.ComplaintID).Select(x => x.PRemarks).FirstOrDefault();
            }
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComplaintDetail(int ComplaintId)
        {
            var Response = ManageRetailer.GetEditComplaint(ComplaintId);
            if (Response.FaulttypeDetailId == 3030 || Response.FaulttypeDetailId == 3042 || Response.FaulttypeDetailId == 3049)
            {
                Response.FaultTypeDetailOtherRemarks = db.JobsDetails.Where(x => x.JobID == Response.ID).OrderByDescending(x => x.ID).FirstOrDefault().PRemarks;
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
                Response.ProgressStatusName = db.JobsDetails.Where(x => x.JobID == Response.ID).OrderByDescending(x => x.ID).Select(x => x.PRemarks).FirstOrDefault();
            }
            Response.ProgressStatusOtherRemarks = db.JobsDetails.Where(x => x.JobID == Response.ID).OrderByDescending(x => x.ID).Select(x => x.PRemarks).FirstOrDefault();






            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEditComplaint(int ComplaintId)
        {
            var Response = ManageRetailer.GetEditComplaint(ComplaintId);
            
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