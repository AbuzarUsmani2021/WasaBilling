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
    public class NationalUserController : Controller
    {

        #region Sale Officer

        [CustomAuthorize]
        //View
        public ActionResult NationalUser()
        {
            // Load RegionalHead Data ...

            //var objSaleOffice = new NationalUserData();
            //objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            //objSaleOffice.RegionData = ManageRegion.GetRegionDataList();
            //objSaleOffice.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            //objSaleOffice.SaleOfficerData = ManageSaleOffice.GetAllSO();
            //return View(objSaleOffice);


            var objSaleOffice = new NationalUserData();
            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            objSaleOffice.RegionData = ManageRegion.GetRegionDataList();
            objSaleOffice.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objSaleOffice.SaleOfficerData = ManageSaleOffice.GetAllSO();
            objSaleOffice.Cities = new List<CityData>();
            objSaleOffice.Areas = new List<Area>();
            return View(objSaleOffice);
        }

        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSaleOfficersAccess([Bind(Exclude = "TID,RegionalHead")] NationalUserData userData)
        {
            Boolean boolFlag = true;

            ValidationResult results = new ValidationResult();
            try
            {
                if (userData != null)
                {
                    FOSDataModel dbcontext = new FOSDataModel();
                    var salelist = ManageSaleOffice.GetSaleOfficerListRelatedtoregionalHeadID(userData.RegionID);
                    if (salelist.Count > 0)
                    {
                        foreach (var list in salelist)
                        {
                            Tbl_Access acc = new Tbl_Access();
                            acc.RegionID = userData.RegionID;
                            acc.SaleOfficerID = list.ID;
                            acc.ReportedDown = list.ID;
                            acc.RepotedUP = userData.SOID;
                            acc.CreatedOn = DateTime.Now;
                            acc.Status = true;
                            acc.IsDeleted = false;
                            dbcontext.Tbl_Access.Add(acc);
                            dbcontext.SaveChanges();

                        }
                    }
                }
                return Content("1");

            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }


        }
        public JsonResult GetSOListByRegionalHeadID(int RegionalHeadID)
        {
            var result = ManageSaleOffice.GetSOByRegionalHeadId(RegionalHeadID);
            return Json(result);
        }
        public JsonResult NationalUserAccessDataHandler(DTParameters param, Int32 CityID)
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
        public JsonResult GetCityListByRegionHeadID(int ID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(ID);
            return Json(result);
        }
        public JsonResult GetSalesOfficer(int RegionID)
        {

            using (FOSDataModel model = new FOSDataModel())
            {
                var result = model.spGetSaleOfficer(RegionID).ToList();
             
                return Json(result);
            }

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
        public JsonResult DataHandler(DTParameters param, int RegionalHeadType, int RegionalHeadID)
        {
            try
            {
                var dtsource = new List<SaleOfficerData>();

                int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (regionalheadID == 0)
                {
                    dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType, RegionalHeadID);
                }
                else
                {
                    RegionalHeadID = regionalheadID;
                    dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType, RegionalHeadID);
                }

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
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
        public int DeleteAccess(int ID)
        {
            return FOS.Setup.ManageArea.DeleteNationalUserAccess(ID);




        }
        #endregion Sale Officer


    }
}