using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FOS.Web.UI.Controllers
{
    public class SetupController : Controller
    {
        #region SchemeInformation
        public int DeleteScheme(int schemeID)
        {
            return FOS.Setup.ManageScheme.DeleteScheme(schemeID);
        }
        public JsonResult SchemeDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<SchemeData>();

                dtsource = ManageScheme.GetSchemesForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SchemeData> data = ManageScheme.GetSchemeResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageScheme.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<SchemeData> result = new DTResult<SchemeData>
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
        [CustomAuthorize]
        //View Work...
        public ActionResult SchemeInformation()
        {
            return View();
        }
        public JsonResult AddUpdateScheme(SchemeData tas)
        {

            try
            {
                //var serialize = JsonConvert.DeserializeObject<List<TblMasterScheme>>(cont);
                FOSDataModel dbContext = new FOSDataModel();
                ValidationResult results = new ValidationResult();
                //if (serialize != null)
                //{
                    TblMasterScheme ms = new TblMasterScheme();
                if (tas.SchemeID==0) {
                    //ms.RangeID = tas.RangeID;
                    ms.SchemeDateFrom = tas.SchemeDateFrom;
                    ms.SchemeDateTo = tas.SchemeDateTo;
                    ms.SchemeInfo = tas.SchemeInfo;
                    ms.isActive = true;
                    TblMasterScheme ms2 = dbContext.TblMasterSchemes.Where(x => x.isActive == true).FirstOrDefault();
                    ms2.isActive = false;
                    dbContext.TblMasterSchemes.Add(ms);
                    dbContext.SaveChanges();
                }
                else
                {
                    ms = dbContext.TblMasterSchemes.Where(u => u.MasterSchemeID == tas.SchemeID).FirstOrDefault();
                    ms.SchemeDateFrom = tas.SchemeDateFrom;
                    ms.SchemeDateTo = tas.SchemeDateTo;
                    ms.SchemeInfo = tas.SchemeInfo;
                    ms.isActive = true;
                    TblMasterScheme ms2 = dbContext.TblMasterSchemes.Where(x => x.isActive == true).FirstOrDefault();
                    ms2.isActive = false;
                    dbContext.SaveChanges();
                }
                //}
                return Json("1");
            }
            catch (Exception ex)
            {
                return Json("2");
            }


        }
        #endregion

        #region REGION

        [CustomAuthorize]
        //View Work...
        public ActionResult Region()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateRegion([Bind(Exclude = "TID")] RegionData newRegion)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.RegionID == 0)
                    {
                        RegionValidator validator = new RegionValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageRegion.AddUpdateRegion(newRegion, userID);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult RegionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<RegionData>();

                dtsource = ManageRegion.GetRegionForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionData> data = ManageRegion.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionData> result = new DTResult<RegionData>
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

        public JsonResult GetEditProjects(int CityID)
        {
            var Response = ManageCity.GetEditProjects(CityID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEditRegions(int RegionID)
        {
            FOSDataModel db = new FOSDataModel();
            var Response = ManageRegion.GetEditRegions(RegionID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }




        //Delete Region Method...
        public int DeleteRegion(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteRegion(RegionID);
        }

        #endregion REGION

        #region CITY

        [CustomAuthorize]
        //View Work...
        public ActionResult City()
        {
            // Load Region Data For City Records ...
            var objCity = new CityData();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);

            return View(objCity);
        }

        [HttpPost]  
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateCity([Bind(Exclude = "TID")] CityData newCity)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (newCity != null)
                    {
                        if (newCity.ID == 0)
                        {
                            CityValidator validator = new CityValidator();
                            results = validator.Validate(newCity);
                            boolFlag = results.IsValid;
                        }

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddUpdateCity(newCity);
                            if (Response == 1)
                            {
                                return Content("1");
                            }
                            else if (Response == 2)
                            {
                                return Content("2");
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
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult CityDataHandler(DTParameters param, Int32 RegionID)
        {
            try
            {
                var dtsource = new List<CityData>();

                dtsource = ManageCity.GetCityForGrid(RegionID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<CityData> data = ManageCity.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageCity.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<CityData> result = new DTResult<CityData>
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
        public int DeleteCity(int cityID)
        {
            return FOS.Setup.ManageCity.DeleteCity(cityID);
        }

        // Get One City For Edit
        public JsonResult GetEditCity(int CityID)
        {
            var Response = ManageCity.GetEditCity(CityID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        #endregion CITY

        #region AREA

        [CustomAuthorize]
        // View ...
        public ActionResult Area()
        {
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<Region> RegionObj = FOS.Setup.ManageRegion.GetRegionList(RHID);
            var objRegion = RegionObj.FirstOrDefault();
            List<CityData> CityObj = FOS.Setup.ManageCity.GetCityListByRegionID(objRegion.ID);
            ViewData["CityObj"] = CityObj;

            var objArea = new AreaData();
            objArea.Regions = RegionObj;
            objArea.Cities = CityObj;

            return View(objArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateArea([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateArea(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult AreaDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetAreaForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetCityListByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionID(RegionID);
            return Json(result);
        }


        public JsonResult GetSOListByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
            return Json(result);
        }


        //Delete Region...
        public int DeleteArea(int areaID)
        {
            return FOS.Setup.ManageArea.DeleteArea(areaID);
        }

        #endregion AREA

        #region Project

        [CustomAuthorize]
        //Project Uses Zones Table
        public ActionResult Project()
        {
            // Load Region Data For City Records ...
            var objCity = new ZoneData();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);
            return View(objCity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateProject([Bind(Exclude = "TID")] ZoneData newCity)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (newCity != null)
                    {
                        if (newCity.ID == 0)
                        {
                            zoneValidator validator = new zoneValidator();
                            results = validator.Validate(newCity);
                            boolFlag = results.IsValid;
                        }

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddUpdateProject(newCity);
                            if (Response == 1)
                            {
                                return Content("1");
                            }
                            else if (Response == 2)
                            {
                                return Content("2");
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
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult ProjectDataHandler(DTParameters param, Int32 RegionID)
        {
            try
            {
                var dtsource = new List<ZoneData>();
                dtsource = ManageCity.GetProjectsForGrid(RegionID);
                List<String> columnSearch = new List<string>();
                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }
                List<ZoneData> data = ManageCity.GetResultProject(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageCity.CountProject(param.Search.Value, dtsource, columnSearch);
                DTResult<ZoneData> result = new DTResult<ZoneData>
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
        public int DeleteProjects(int cityID)
        {
            return FOS.Setup.ManageCity.DeleteProjects(cityID);
        }

        // Get One City For Edit
       

       

        #endregion CITY

        //#region CITY

        //[CustomAuthorize]
        ////View Work...
        //public ActionResult Zone()
        //{
        //    // Load Region Data For City Records ...
        //    var objCity = new ZonesData();
        //    int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
        //    objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);
        //    objCity.Zones = FOS.Setup.ManageRegion.GetZones();
        //    return View(objCity);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddUpdateZone([Bind(Exclude = "TID")] ZonesData newCity)
        //{
        //    Boolean boolFlag = true;
        //    ValidationResult results = new ValidationResult();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (newCity != null)
        //            {
        //                if (newCity.ID == 0)
        //                {
        //                    zoneValidator validator = new zoneValidator();
        //                    results = validator.Validate(newCity);
        //                    boolFlag = results.IsValid;
        //                }

        //                if (boolFlag)
        //                {
        //                    int Response = ManageCity.AddUpdateZones(newCity);
        //                    if (Response == 1)
        //                    {
        //                        return Content("1");
        //                    }
        //                    else if (Response == 2)
        //                    {
        //                        return Content("2");
        //                    }
        //                    else
        //                    {
        //                        return Content("0");
        //                    }
        //                }
        //                else
        //                {
        //                    IList<ValidationFailure> failures = results.Errors;
        //                    StringBuilder sb = new StringBuilder();
        //                    sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
        //                    foreach (ValidationFailure failer in results.Errors)
        //                    {
        //                        sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
        //                        Response.StatusCode = 422;
        //                        return Json(new { errors = sb.ToString() });
        //                    }
        //                }
        //            }
        //        }

        //        return Content("0");
        //    }
        //    catch (Exception exp)
        //    {
        //        return Content("Exception : " + exp.Message);
        //    }
        //}

        ////Get All Region Method...
        //public JsonResult ZoneDataHandler(DTParameters param, Int32 RegionID)
        //{
        //    try
        //    {
        //        var dtsource = new List<ZonesData>();

        //        dtsource = ManageCity.GetZonesForGrid(RegionID);

        //        List<String> columnSearch = new List<string>();

        //        foreach (var col in param.Columns)
        //        {
        //            columnSearch.Add(col.Search.Value);
        //        }

        //        List<ZonesData> data = ManageCity.GetResult4(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
        //        int count = ManageCity.Count4(param.Search.Value, dtsource, columnSearch);
        //        DTResult<ZonesData> result = new DTResult<ZonesData>
        //        {
        //            draw = param.Draw,
        //            data = data,
        //            recordsFiltered = count,
        //            recordsTotal = count
        //        };
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}

        ////Delete Region Method...
        //public int DeleteZone(int cityID)
        //{
        //    return FOS.Setup.ManageCity.DeleteCity(cityID);
        //}

        //// Get One City For Edit
        //public JsonResult GetEditZone(int CityID)
        //{
        //    var Response = ManageCity.GetEditCity(CityID);
        //    return Json(Response, JsonRequestBehavior.AllowGet);
        //}

        //#endregion CITY


        #region ActivityPurpose

        [CustomAuthorize]
        //View Work...
        public ActionResult ActivityPurpose()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateActivityPurpose([Bind(Exclude = "TID")] PurposeOfActivityData newRegion)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.ID == 0)
                    {
                        ActivityPurposeValidator validator = new ActivityPurposeValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageRegion.AddUpdateActivityPurpose(newRegion);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult ActivityPurposeDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<PurposeOfActivityData>();

                dtsource = ManageRegion.GetActivityPurposeForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<PurposeOfActivityData> data = ManageRegion.GetResult5(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count5(param.Search.Value, dtsource, columnSearch);
                DTResult<PurposeOfActivityData> result = new DTResult<PurposeOfActivityData>
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
        public int DeleteActivityPurpose(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteActivityPurpose(RegionID);
        }

        #endregion ActivityPurpose

        #region AccessGrid

        public JsonResult AccessDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetAccessForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Tbl_AccessModel> data = ManageArea.GetResult7(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count7(param.Search.Value, dtsource, columnSearch);
                DTResult<Tbl_AccessModel> result = new DTResult<Tbl_AccessModel>
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


        #region SOREGIONSGrid

        public JsonResult SORegionsDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetSORegionsForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Tbl_AccessModel> data = ManageArea.GetResult7(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count7(param.Search.Value, dtsource, columnSearch);
                DTResult<Tbl_AccessModel> result = new DTResult<Tbl_AccessModel>
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


        #endregion SOREGIONSGrid


        #endregion ActivityPurpose


        [CustomAuthorize]
        public ActionResult SchemeInfo()
        {
            FOSDataModel db = new FOSDataModel();
            var objSaleOffice = new SchemeData();
            List<CategoryData> catData = new List<CategoryData>();
            catData = ManageCategory.GetCat();
            catData.Insert(0, new CategoryData
            {
                MainCategID = 0,
                MainCategDesc = "Select Range"
            });
            objSaleOffice.RangeData = catData;
            return View(objSaleOffice);

        }

        public JsonResult ItemDataHandler(DTParameters param, int? RangeID)
        {
            try
            {
                var dtsource = new List<Items>();

                dtsource = ManageItem.GetItemList(RangeID);


                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public JsonResult SubmitItem(string cont, SchemeData tas)
        {

            try
            {
                var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);
                FOSDataModel dbContext = new FOSDataModel();
                ValidationResult results = new ValidationResult();
                if (serialize != null)
                {
                    TblMasterScheme ms = new TblMasterScheme();
                    TblDetailScheme ds = new TblDetailScheme();
                    //ms.RangeID = tas.RangeID;
                    ms.SchemeDateFrom = tas.SchemeDateFrom;
                    ms.SchemeDateTo = tas.SchemeDateTo;
                    ms.SchemeInfo = tas.SchemeInfo;
                    dbContext.TblMasterSchemes.Add(ms);
                    dbContext.SaveChanges();
                    ms.MasterSchemeID = dbContext.TblMasterSchemes.OrderByDescending(u => u.MasterSchemeID).Select(u => u.MasterSchemeID).FirstOrDefault();
                    if (ms.MasterSchemeID > 0)
                    {
                        foreach (var items in serialize)
                        {
                            ds.ItemID = items.ItemId;
                            ds.ItemName = items.ItemName;
                            ds.Packing = items.ItemPacking.ToString();
                            ds.TradePrice = items.ItemPrice.ToString();
                            ds.Scheme = items.Scheme;
                            ds.MasterID = ms.MasterSchemeID;
                            dbContext.TblDetailSchemes.Add(ds);
                            dbContext.SaveChanges();
                        }
                    }
                }
                return Json("1");
            }
            catch (Exception ex)
            {
                return Json("2");
            }


        }







        #region SubDivision

        [CustomAuthorize]
        // View ...
        public ActionResult SubDivision()
        {
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<Region> RegionObj = FOS.Setup.ManageRegion.GetRegionList(RHID);
            var objRegion = RegionObj.FirstOrDefault();
            List<CityData> CityObj = FOS.Setup.ManageCity.GetCityListByRegionID(objRegion.ID);
            ViewData["CityObj"] = CityObj;
            var city = CityObj.FirstOrDefault();
            var objArea = new AreaData();
            objArea.Regions = RegionObj;
            objArea.Cities = CityObj;
            objArea.Areas = FOS.Setup.ManageCity.GetAreaList(city.ID);
            return View(objArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateSubDivision([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateSubDivision(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult SubDivisionDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetSubDivisionForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetAreaListByCityID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetAreaList(RegionID);
            return Json(result);
        }


        //public JsonResult GetSOListByRegionID(int RegionID)
        //{
        //    var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
        //    return Json(result);
        //}


        //Delete Region...
        public int DeleteSubDivision(int areaID)
        {
            return FOS.Setup.ManageArea.DeleteSubDivision(areaID);
        }

        #endregion AREA





    }
}