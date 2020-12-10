using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using FOS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace FOS.Web.UI.Controllers
{
    public class DropdownsController : Controller
    {

        private FOSDataModel db = new FOSDataModel();

        //Fault Type Dropdown Start
        public ActionResult AllDropdowns()
        {
            ViewBag.FaultTypes = FOS.Setup.ManageDropdowns.GetAllFaulttype();
            return View();
        }

        //Fault Type Dropdown END


        // Fauly Type Detail Start

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateFaulttypedetail(DropdownData ftd)
        {
            if (ftd.FId!=0)
            {
                int Response = ManageDropdowns.AddUpdateFaulttypedetail(ftd);
                if (Response == 2)
                {
                    return Content("2");
                }
                else if (Response == 3)
                {
                    return Content("3");
                }
                else if (Response == 4)
                {
                    return Content("4");
                }
                else
                {
                    return Content("0");
                }
            }
            else
            { 
            return Content("1");
            }
        }
        public JsonResult GetFaulttypedetail(DTParameters param, int FaultType)
        {
            try
            {
                DTResult<DropdownData> result = null;
                var dtsource = new List<DropdownData>();
                dtsource = ManageDropdowns.FaulttypedetailFilteredData(FaultType);
                List<String> columnSearch = new List<String>();
                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }
                List<DropdownData> data = ManageDropdowns.GetResult12(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageDropdowns.Count(param.Search.Value, dtsource, columnSearch);
                result = new DTResult<DropdownData>
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

        public JsonResult GetEditFaulttypedetail(int ftd)
        {
            var Response = ManageDropdowns.GetEditFaulttypedetail(ftd);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public int DeleteFaulttypedetail(int ftd)
        {
            return FOS.Setup.ManageDropdowns.DeleteFaulttypedetail(ftd);
        }

        // Fauly Type Detail END

        // Progress Status Start

     
        public JsonResult GetProgressStatusData(DTParameters param, int FaultType)
        {
            try
            {
                DTResult<DropdownData> result = null;
                var dtsource = new List<DropdownData>();
                dtsource = ManageDropdowns.GetProgressStatusData(FaultType);
                List<String> columnSearch = new List<String>();
                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }
                List<DropdownData> data = ManageDropdowns.GetProgressStatusResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageDropdowns.GetProgressStatusCount(param.Search.Value, dtsource, columnSearch);
                result = new DTResult<DropdownData>
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

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateProgressStatus(DropdownData PSData)
        {

            if (PSData.ProgressStatusFaultTypeID != 0)
            {
                int Response = ManageDropdowns.AddUpdateProgressStatus(PSData);
                if (Response == 5)
                {
                    return Content("5");
                }
                else if (Response == 6)
                {
                    return Content("6");
                }
                else if (Response == 7)
                {
                    return Content("7");
                }
                else
                {
                    return Content("0");
                }
            }
            else
            { 
            return Content("1");
            }
        }

        public JsonResult GetEditProgressStatus(int ProgressStatusID)
        {
            var Response = ManageDropdowns.GetEditProgressStatus(ProgressStatusID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public int DeleteProgressStatus(int ProgressStatusID)
        {
            return FOS.Setup.ManageDropdowns.DeleteProgressStatus(ProgressStatusID);
        }

        // Progress Status END



        // Work Done Start

    
        public JsonResult GetWorkDoneTable(DTParameters param, int WorkDoneFaulttypeID)
        {
            try
            {
                DTResult<DropdownData> result = null;
                var dtsource = new List<DropdownData>();
                dtsource = ManageDropdowns.GetWorkDoneTable(WorkDoneFaulttypeID);
                List<String> columnSearch = new List<String>();
                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }
                List<DropdownData> data = ManageDropdowns.GetWorkDoneResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageDropdowns.GetWorkDoneCount(param.Search.Value, dtsource, columnSearch);
                result = new DTResult<DropdownData>
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

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateWorkDone(DropdownData WorkDoneData)
        {
            if (WorkDoneData.WorkDoneFaulttypeID != 0)
            {
                int Response = ManageDropdowns.AddUpdateWorkDone(WorkDoneData);
                if (Response == 8)
                {
                    return Content("8");
                }
                else if (Response == 9)
                {
                    return Content("9");
                }
                else if (Response == 10)
                {
                    return Content("10");
                }
                else
                {
                    return Content("0");
                }
            }
            else
            {
                return Content("1");
            }
        }

        public JsonResult GetEditWorkDoneData(int WorkDoneID)
        {
            var Response = ManageDropdowns.GetEditWorkDoneData(WorkDoneID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public int DeleteWorkDoneData(int WorkDoneID)
        {
            return FOS.Setup.ManageDropdowns.DeleteWorkDoneData(WorkDoneID);
        }

        // Work Done End







    }
}
