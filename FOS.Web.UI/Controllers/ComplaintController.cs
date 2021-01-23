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
            var result = FOS.Setup.ManageCity.GetProgressDetailList(ClientID, "Select");
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