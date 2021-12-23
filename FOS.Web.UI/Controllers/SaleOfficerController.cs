using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FOS.Web.UI.Controllers
{
    public class SaleOfficerController : Controller
    {
        FOSDataModel dbContext = new FOSDataModel();
        #region Sale Officer

        [CustomAuthorize]
        //View
        public ActionResult SaleOfficer()
        {
            // Load RegionalHead Data ...

            var objSaleOffice = new SaleOfficerData();
            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            objSaleOffice.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objSaleOffice.Regions = FOS.Setup.ManageRegion.GetZonesListInSO();
            objSaleOffice.Towns = FOS.Setup.ManageRegion.GetTownListInSO();
            objSaleOffice.SORoles = FOS.Setup.ManageRegion.GetSORoles();
            objSaleOffice.SOProjects = FOS.Setup.ManageRegion.GetSOProjects();
            objSaleOffice.Cities = new List<CityData>();
            objSaleOffice.Areas = new List<Area>();
            return View(objSaleOffice);
        }

        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUpdateSaleOfficer([Bind(Exclude = "TID,RegionalHead")] SaleOfficerData newSaleOfficer)
        {
            Boolean boolFlag = true;
            Boolean PhoneNumberFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {

                if (newSaleOfficer != null)
                {

                    if (newSaleOfficer.ID == 0)
                    {
                        SaleOfficerValidator validator = new SaleOfficerValidator();
                        results = validator.Validate(newSaleOfficer);
                        boolFlag = results.IsValid;
                    }
                    boolFlag = true;

                    //if (newSaleOfficer.Phone1 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumber1Exist(newSaleOfficer.ID, newSaleOfficer.Phone1 == null ? "" : newSaleOfficer.Phone1) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newSaleOfficer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumber2Exist(newSaleOfficer.ID, newSaleOfficer.Phone2 == null ? "" : newSaleOfficer.Phone2) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newSaleOfficer.Phone1 != null && newSaleOfficer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumberExist(newSaleOfficer.ID, newSaleOfficer.Phone1 == null ? "" : newSaleOfficer.Phone1, newSaleOfficer.Phone2 == null ? "" : newSaleOfficer.Phone2) == 1)
                    //    {
                    //        PhoneNumberFlag = false;
                    //    }
                    //}

                    if (PhoneNumberFlag)
                    {
                        if (boolFlag)
                        {
                            if (ManageSaleOffice.AddUpdateSaleOfficer(newSaleOfficer))
                            {
                                return Content("1");
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
                    else
                    {
                        return Content("2");
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        public JsonResult GetCityListByRegionHeadID(int ID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(ID);
            return Json(result);
        }

        public JsonResult GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionalHeadAccordingToType(RegionalHeadType);
            return Json(result);
        }
        public JsonResult GetSORegions(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionsofSO(RegionalHeadType);
        
            return Json(result);
        }

        public JsonResult GetSOByRegionalHeadId(int RegionalHeadId)
        {
            var result = FOS.Setup.ManageSaleOffice.GetSOByRegionalHeadId(RegionalHeadId);
            return Json(result);
        }
        

        public JsonResult GetRetailersBySOID(int soId)
        {
            var result = FOS.Setup.ManageRetailer.GetRetailerBySOID(soId);
            return Json(result);
        }
        public JsonResult GetAreaListByCityID(int ID)
        {
            var result = FOS.Setup.ManageArea.GetAreaListByCityID(ID);
            return Json(result);
        }

        public JsonResult GetAreaForSaleOfficerEdit(int ID, int CityID)
        {
            var result = FOS.Setup.ManageArea.GetAreaListByCityIDEdit(ID, CityID);
            return Json(result);
        }

        //Get All Region Method...

        public JsonResult GetSOData(int SaleOfficerID)
        {
            var Response = ManageRetailer.GetSOData(SaleOfficerID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DataHandler(DTParameters param , int RegionalHeadType , int RegionalHeadID)
        {
            try
            {
                var dtsource = new List<SaleOfficerData>();

                int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (regionalheadID == 0)
                {
                    dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType , RegionalHeadID);
                }
                else {
                    RegionalHeadID = regionalheadID;
                    dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType , RegionalHeadID);
                }

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }
                foreach (var item in dtsource)
                {
                    foreach (var item1 in item.SaleOfficersProjects)
                    {
                        item.SaleOfficerProjectsName += dbContext.Zones.Where(x => x.ID == item1).Select(x => x.Name).FirstOrDefault() + "</br>";
                    }
                    foreach (var item2 in item.SOZones)
                    {
                        item.SaleOfficerZonesName += dbContext.Cities.Where(x => x.ID == item2).Select(x => x.Name).FirstOrDefault() + "</br>";
                    }
                  
                    item.SaleOfficerRoleName = dbContext.SORoles.Where(x => x.ID == item.SoRoleID).Select(x => x.Name).FirstOrDefault();

                }

                List<SaleOfficerData> data = ManageSaleOffice.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageSaleOffice.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<SaleOfficerData> result = new DTResult<SaleOfficerData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Region Method...
        public int DeleteSaleOfficer(int saleOfficerID)
        {
            return ManageSaleOffice.DeleteSaleOfficer(saleOfficerID);
        }

        public int ResetIMEI(int saleOfficerID)
        {
            return ManageSaleOffice.Reset(saleOfficerID);
        }

        #endregion Sale Officer

    }
}