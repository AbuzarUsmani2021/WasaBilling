using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.Xml;
using FOS.Web.UI.DataSets;
using CrystalDecisions.CrystalReports.Engine;
using Shared.Diagnostics.Logging;
using FOS.Web.UI.Common;
using System.Web.Hosting;
using FOS.Web.UI.Controllers.API;

namespace FOS.Web.UI.Controllers
{
    public class RetailerController : Controller
    {
        private FOSDataModel db = new FOSDataModel();
        #region Retailer 

        [CustomAuthorize]
        // View
        public ActionResult Retailer()
        {
            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetSaleOfficerList();
            var objSaleOff = SaleOfficerObj.FirstOrDefault();


            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList();


            List<DealerData> DealerObj = ManageDealer.GetDealerListBySaleOfficerID(objSaleOff.ID);

            var objRetailer = new RetailerData();

            objRetailer.RegionalHead = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Dealers = DealerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
           // objRetailer.Banks = ManageRetailer.GetBanks();
         
            return View(objRetailer);
        }


        // Sites Start

        public JsonResult AllSitesData(DTParameters param)
        {
            try
            {
                var dtsource = new List<RetailerData>();
                dtsource = ManageRetailer.AllSitesData(param.ProjectId);
                List<String> columnSearch = new List<string>();
                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }
                List<RetailerData> data = ManageRetailer.GetAllSitesResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRetailer.CountAllSites(param.Search.Value, dtsource, columnSearch);
                DTResult<RetailerData> result = new DTResult<RetailerData>
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

        public ActionResult NewRetailer()
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
         

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetSaleOfficerListByRegionalHeadID(regId);
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            FOSDataModel dbContext = new FOSDataModel();
            var objRetailer = new RetailerData();
            objRetailer.Client = regionalHeadData;
            var userID = Convert.ToInt32(Session["UserID"]);

            if (userID == 1025)
            {
                objRetailer.Projects = FOS.Setup.ManageCity.GetProjectsList();
            }
            else if(userID==1026|| userID == 1027)
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

           // objRetailer.SaleOfficers = ManageSaleOffice.GetSaleOfficerListByRegionalHeadID();
            objRetailer.Dealers = DealerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();

            return View(objRetailer);
        }
       
        [HttpPost]
        public JsonResult NewUpdateRetailer([Bind(Exclude = "TID,SaleOfficers,Dealers")] RetailerData newRetailer)
        {
            int result = 0;
            if (Request.Files["Picture1"] != null)
            {
                var file = Request.Files["Picture1"];
                if (file.FileName != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    var extension = System.IO.Path.GetExtension(filename).ToLower();
                    var path = HostingEnvironment.MapPath(Path.Combine("/Images/SitesImages/", filename));
                    file.SaveAs(path);
                    newRetailer.Picture1 = "/Images/SitesImages/" + filename;
                }
            }
            if (newRetailer.ClientID != 0 && newRetailer.SaleOfficerID != 0 && newRetailer.AreaID != 0 && newRetailer.CityID != 0 && newRetailer.SubDivisionID != 0 && newRetailer.Name != null && newRetailer.RetailerCode != null)
            {

                newRetailer.CreatedBy = SessionManager.Get<int>("UserID");
                newRetailer.UpdatedBy = SessionManager.Get<int>("UserID");
                result = ManageRetailer.AddUpdateRetailer(newRetailer);
                if (result == 6)
                {
                    result = 6;
                }
                else if (result == 7)
                {
                    result = 7;
                }
                else if (result == 8)
                {
                    result = 8;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEditSites(int SiteID)
        {
            var Response = ManageRetailer.GetEditSites(SiteID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public int DeleteSiteData(int SiteID)
        {
            return FOS.Setup.ManageRetailer.DeleteSiteData(SiteID);
        }







        // Get One City For Edit
        public JsonResult GetEditRetailer(int RetailerID)
        {
            var Response = ManageRetailer.GetEditRetailer(RetailerID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        


        // RetailerData Handler
        public JsonResult RetailerDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<RetailerData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetRetailerForGrid();
                }
                else
                {
                    dtsource = ManageRetailer.GetRetailerForGrid(RegionalheadID);
                }

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RetailerData> data = ManageRetailer.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRetailer.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RetailerData> result = new DTResult<RetailerData>
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

        
        // Get Dealer List By Sales Officer
        public JsonResult GetDealerListBySaleOfficerID(int id , int rid)
        {
            var dealers = Common.CacheManager.Get<List<DealerData>>("DealerList");
            var result = dealers.Where(u => u.RegionalHeadID == rid).ToList();

            return Json(result);
        }

        // Get Dealer List By Sales Officer
        public JsonResult GetDealerListBySaleOfficerIDR(int ZoneID)
        {
            List<DealerData> DealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    DealerData = dbContext.Dealers.Where(u => u.ZoneID == ZoneID && u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.ShopName,
                                    DealerCode = u.DealerCode,
                                }).ToList();
                }
                return Json(DealerData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult GetCityListByIDR(int id)
        {
            List<CityData> DealerData = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    DealerData = dbContext.Regions.Where(u => u.ID == id && u.IsDeleted == false)
                            .Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                   
                                }).ToList();
                }
                return Json(DealerData);
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Get Dealer List By Sales Officer
        public JsonResult LoadCitiesListRelatedToRegionalHead(int CityID)
        {
            var result = FOS.Setup.ManageCity.LoadCitiesListRelatedToRegionalHead(CityID);
            return Json(result);
        }

        // Get Area List By Sales Officer
        public JsonResult GetAreaListBySaleOfficerID(int ID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetSaleOfficerAreaID(ID);
            return Json(result);
        }

        public void ExportToExcel()
        {
       
            // Example data
           StringWriter sw = new StringWriter();

           sw.WriteLine("\"Fixed ID\",\"Person Name\",\"Customer Name\",\"Distributor Name\",\"Sale Officer Name\",\"City Name\",\"Area Name\",\"Address\",\"Retailer Code\",\"CNIC\",\"Phone1\",\"Phone2\",\"Retailer Type\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Retailers"+DateTime.Now+".csv");
            Response.ContentType = "application/octet-stream";

            var retailers = ManageRetailer.GetRetailersForExportinExcel();

            foreach (var retailer in retailers)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\"",
                
                retailer.ID,
                retailer.Name,
                retailer.ShopName,
                retailer.DealerName,
                retailer.SaleOfficerName,
                retailer.CItyName,
                retailer.AreaName,
                retailer.Address,
                retailer.RetailerCode,
                retailer.CNIC,           
                retailer.Phone1 ,
                retailer.Phone2,
                retailer.RetailerType
             

                ));
            }
            Response.Write(sw.ToString());
            Response.End();

            
           }


        #endregion

        #region Pending Retailer

        [CustomAuthorize]
        // View 
        public ActionResult PendingRetailer()
        {
            // Load Dealer Data ...
            RetailerPendingData objRetailer = new RetailerPendingData();
            List<DealerData> DealerObj = ManageDealer.GetDealerList();
            objRetailer.Dealers = DealerObj;
            Common.CacheManager.Store("DealerList", DealerObj);

            return View(objRetailer);
        }


        // Update Retailer From Pending Retailers 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRetailerStatus(RetailerPendingData objRetailer)
        {
            try
            {
                String[] strRetailerID = objRetailer.strRetailerID.TrimEnd(',').Split(',');
                String[] strDealerID = objRetailer.strDealerID.TrimEnd(',').Split(',');
                var RetailersAndDealers = strRetailerID.Zip(strDealerID, (r, d) => new { Retailer = r, Dealer = d });
                //foreach (var RetailerID in strRetailerID)
                //{
                //    foreach (var DealerID in strDealerID)
                //    {
                //        if (ManageRetailer.UpdateDealerID(Convert.ToInt32(DealerID) , Convert.ToInt32(RetailerID)))
                //        {
                //        }
                //    }
                //}

                foreach (var RD in RetailersAndDealers)
                {
                    if (ManageRetailer.UpdateDealerID(Convert.ToInt32(RD.Retailer)))
                     {
                     }
                }


                return Content("1");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        
        // Update Retailer
        public String UpdateRetailer(int ID)
        {
            String strFlag = "0";
            try
            {
                if (ManageRetailer.UpdateDealerID(ID))
                {
                    strFlag = "1";
                }
            }
            catch (Exception)
            {
                strFlag = "0";
            }

            return strFlag;
        }


        // Pending Retailer Data Handler
        public JsonResult PendingRetailerDataHandler()
        {
            try
            {
                var dtsource = new List<RetailerPendingData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetPendingRetailerForGrid();
                }
                else
                {
                    dtsource = ManageRetailer.GetPendingRetailerForGrid(RegionalheadID);
                }
                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        // Delete Retailer
        public int DeleteRetailer(int ID)
        {
            return ManageRetailer.DeleteRetailer(ID);
        }

        public int DeleteDistributor(int retailerID)
        {
            return ManageRetailer.DeleteDistributor(retailerID);
        }

        #endregion

        #region Retailer Map View

        [CustomAuthorize]
        // View ...
        public ActionResult RetailerMapView()
        {
             ViewData["RegionalHead"] = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            
             ViewData["SaleOfficer"] = FOS.Setup.ManageSaleOffice.GetSaleOfficerList(true);
             ViewData["Region"] = FOS.Setup.ManageRegion.GetRegionForGrid();
             ViewData["City"] = FOS.Setup.ManageCity.GetCityList();
             ViewData["Zone"] = FOS.Setup.ManageZone.GetZoneList();
          
            return View();
        }

        [HttpPost]
        public JsonResult GetRetailerLocationsByAllFilters(LatLongModel model)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageRetailer.GetRetailerLocations(model.RegionalHeadID, model.DealerID, model.SaleOfficerID, model.RegionID, model.CityID, model.ZoneID);

            return Json(retailerData);
        }

        [HttpPost]
        public JsonResult GetRetailerLocationsCount(LatLongModel model)
        {
            string count = FOS.Setup.ManageRetailer.GetRetailerLocationsCount(model.RegionalHeadID, model.DealerID, model.SaleOfficerID, model.RegionID, model.CityID, model.ZoneID);

            return Json(count);
        }

        public JsonResult GetRetailerLocations()
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageRetailer.GetRetailerLocations();

            return Json(retailerData);
        }

        public ActionResult GetOneRetailerLocation(int RetailerID)
        {
            var retailerData = FOS.Setup.ManageRetailer.GetOneRetailerLocation(RetailerID);

            return Json(retailerData, JsonRequestBehavior.AllowGet); ;
        }


        public JsonResult GetRetailerLocationsByRegionalHead(int RegionalHeadID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageRetailer.GetRetailerLocationsByRegionalHeadID(RegionalHeadID);

            return Json(retailerData);
        }

        public JsonResult GetRetailerLocationsByDealer(int DealerID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageRetailer.GetRetailerLocationsByDealer(DealerID);

            return Json(retailerData);
        }

        public JsonResult GetRetailerLocationsBySaleOfficer(int SaleOfficerID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageRetailer.GetRetailerLocationsBySaleOfficer(SaleOfficerID);

            return Json(retailerData);
        }

        public JsonResult GetAllDealersListRelatedToRegionalHead(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageRetailer.GetAllDealersListRelatedToRegionalHead(RegionalHeadID);
            return Json(result);
        }

        public JsonResult GetRetailerLocationsByRegion(int RegionID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageRetailer.GetRetailerLocationsByRegion(RegionID);

            return Json(retailerData);
        }

        public JsonResult GetRetailerLocationsByCity(int CityID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageRetailer.GetRetailerLocationsByCity(CityID);

            return Json(retailerData);
        }

        public JsonResult GetAllRegionsListRelatedToSaleOfficer(int SaleOfficerID, int RegionalHeadID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionRelatedToSaleOfficer(SaleOfficerID, RegionalHeadID);
            return Json(result);
        }

        public JsonResult GetAllCitiesListRelatedToRegion(int RegionID)
        {
            var result = FOS.Setup.ManageRetailer.GetCitiesRelatedToRegion(RegionID);
            return Json(result);
        }

        public JsonResult GetAllDealers()
        {
            return Json(FOS.Setup.ManageDealer.GetDealerList());
        }

        public JsonResult GetAllSalesOfficer()
        {
            return Json(FOS.Setup.ManageSaleOffice.GetSaleOfficerList());
        }

        public JsonResult GetAllRegions()
        {
            return Json(FOS.Setup.ManageRegion.GetRegionForGrid());
        }

        public JsonResult GetAllCities()
        {
            return Json(FOS.Setup.ManageCity.GetCityList());
        }



        #endregion

        #region New Retailer

        [CustomAuthorize]
        // View
       


        #endregion

        #region FOS Wise Date Wise Intake Delivered Report-1A

        public ActionResult FosDateWiseReport(string StartingDate, string EndingDate, string TID, string FOSID, string message)
        {
            try
            {
                string[] mul = null;
                try
                {
                    mul = message.Split(',');
                    if (mul.Length == 1)
                    {
                        mul = null;
                    }
                }
                catch (Exception exe)
                {
                }
                TID = TID == "" ? null : TID;
                FOSID = FOSID == "" ? null : FOSID;
                SqlDataAdapter sda;
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime Start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime End = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                var rlist = objRetailers.GetRetailersForExportinExcelReportC(Start, End, TID, FOSID, mul);
                //var rlist = objRetailers.GetRetailersForExportinExcelReport(Start, End);
                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol8 = new DataColumn("visiteddate", typeof(System.DateTime));
                dtNewTable.Columns.Add(dcol8);
                foreach (var rowfiles in rlist)
                {
                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                    dtrow[1] = rowfiles.retailername;
                    dtrow[2] = rowfiles.saleofficername;
                    dtrow[3] = rowfiles.shopname;
                    dtrow[4] = rowfiles.cityname;
                    dtrow[5] = rowfiles.retailertype;
                    dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                    dtrow[7] = Convert.ToDateTime(rowfiles.visiteddate);
                    dtNewTable.Rows.Add(dtrow);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/FosDateWise.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "FosDateWise.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("FosDateWiseReport_{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }

        #endregion

        #region FOS Wise Month Wise Intake Delivered Report

        [HttpGet]
        public FileResult FosMonthWiseReport(string StartingDate, string EndingDate, string TID, string FOSID, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            TID = TID == "" ? null : TID;
            FOSID = FOSID == "" ? null : FOSID;
            SqlDataAdapter sda;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            //var monthwise = objRetailers.GetRetailersForMonthWise(start, end).OrderBy(m => Convert.ToDateTime(m.monthss)).ThenBy(m => m.cityname);
            var monthwise = objRetailers.FOSGetRetailersForMonthWise(start, end, TID, FOSID, mul).OrderBy(m => Convert.ToDateTime(m.monthss)).ThenBy(m => m.cityname);
            rptDataSet obj = new rptDataSet();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("monthss", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in monthwise)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.monthss);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/FosMonthWiseRpt.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "FosMonthWise.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("FosMonthWiseReport_{0}.xls", DateTime.Now.ToShortDateString()));
                //return new FileContentResult(StreamToByte(stream), "application/vnd.ms-excel");
            }

            //return View();
        }


        public static byte[] StreamToByte(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        #endregion

        #region FOS Wise Date Wise Intake Report-1A

        public ActionResult FosDateWiseIntakeReport(string StartingDate, string EndingDate, string TID, string FOSID, string message)
        {
            try
            {
                string[] mul = null;
                try
                {
                    mul = message.Split(',');
                    if (mul.Length == 1)
                    {
                        mul = null;
                    }
                }
                catch (Exception exe)
                {
                }
                TID = TID == "" ? null : TID;
                FOSID = FOSID == "" ? null : FOSID;
                SqlDataAdapter sda;
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime Start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime End = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                var rlist = objRetailers.FosWiseIntakeReport(Start, End, TID, FOSID, mul);
                //var rlist = objRetailers.GetRetailersForExportinExcelReport(Start, End);
                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol8 = new DataColumn("visiteddate", typeof(System.DateTime));
                dtNewTable.Columns.Add(dcol8);
                foreach (var rowfiles in rlist)
                {
                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                    dtrow[1] = rowfiles.retailername;
                    dtrow[2] = rowfiles.saleofficername;
                    dtrow[3] = rowfiles.shopname;
                    dtrow[4] = rowfiles.cityname;
                    dtrow[5] = rowfiles.retailertype;
                    dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                    dtrow[7] = Convert.ToDateTime(rowfiles.visiteddate);
                    dtNewTable.Rows.Add(dtrow);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/FosDateWiseIntake.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "FosDateWiseIntake.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("FosDateWiseIntakeReport_{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }

        #endregion

        #region FOS Wise Month Wise Intake Report

        [HttpGet]
        public FileResult FosMonthWiseIntakeReport(string StartingDate, string EndingDate, string TID, string FOSID, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            TID = TID == "" ? null : TID;
            FOSID = FOSID == "" ? null : FOSID;
            SqlDataAdapter sda;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            //var monthwise = objRetailers.GetRetailersForMonthWise(start, end).OrderBy(m => Convert.ToDateTime(m.monthss)).ThenBy(m => m.cityname);
            var monthwise = objRetailers.FOSMonthWiseIntake(start, end, TID, FOSID, mul).OrderBy(m => Convert.ToDateTime(m.monthss)).ThenBy(m => m.cityname);
            rptDataSet obj = new rptDataSet();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("monthss", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in monthwise)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.monthss);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/FosMonthWiseIntakeReport.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "FosMonthWiseIntakeReport.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("FosMonthWiseIntakeReport_{0}.xls", DateTime.Now.ToShortDateString()));
                //return new FileContentResult(StreamToByte(stream), "application/vnd.ms-excel");
            }

            //return View();
        }

        #endregion

        #region City Wise Date Wise Intake Delivered Report

        public ActionResult CityDateWiseRetailerReport(string StartingDate, string EndingDate, string TID, string Shoptypeid, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            TID = TID == "" ? null : TID;
            Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
            SqlDataAdapter sda;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime Start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime End = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            var rlist = objRetailers.CityGetRetailersForExportinExcelReportC(Start, End, TID, Shoptypeid, mul);
            rptDataSet obj = new rptDataSet();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("visiteddate", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in rlist)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.visiteddate);
                dtNewTable.Rows.Add(dtrow);
            }
            
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/RetailerReport1.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if(type=="pdf")
            {
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf",  "Retailers.pdf");
            }
            else
            {
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/ms-excel", string.Format("CityDateWiseRetailerReport_{0}.xls", DateTime.Now.ToShortDateString()));
            }

            return Json(null);
            //return View();
        }

        #endregion

        #region City Wise Date Wise Intake Report

        public ActionResult CityDateWiseIntakeReport(string StartingDate, string EndingDate, string TID, string Shoptypeid, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            TID = TID == "" ? null : TID;
            Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
            SqlDataAdapter sda;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime Start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime End = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            var rlist = objRetailers.CityDateWiseIntake(Start, End, TID, Shoptypeid, mul);
            rptDataSet obj = new rptDataSet();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("visiteddate", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in rlist)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.visiteddate);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/CityDateWiseIntakeReport.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "CityDateWiseIntakeReport.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("CityDateWiseIntakeReport_{0}.xls", DateTime.Now.ToShortDateString()));
            }

            return Json(null);
            //return View();
        }

        #endregion

        #region City Wise Month Wise Intake Delivered Report        
        [HttpGet]
        public ActionResult CityMonthWiseRetailerReport(string StartingDate, string EndingDate, string TID, string Shoptypeid, string message)
        {
            try
            {
                string[] mul = null;
                try
                {
                    mul = message.Split(',');
                    if (mul.Length == 1)
                    {
                        mul = null;
                    }
                }
                catch (Exception exe)
                {
                }
                TID = TID == "" ? null : TID;
                Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
                SqlDataAdapter sda;
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                var monthwise = objRetailers.CityGetRetailersForMonthWise(start, end, TID, Shoptypeid, mul).OrderBy(m => Convert.ToDateTime(m.monthss)).ThenBy(m => m.cityname);
                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol8 = new DataColumn("monthss", typeof(System.DateTime));
                dtNewTable.Columns.Add(dcol8);
                foreach (var rowfiles in monthwise)
                {
                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                    dtrow[1] = rowfiles.retailername;
                    dtrow[2] = rowfiles.saleofficername;
                    dtrow[3] = rowfiles.shopname;
                    dtrow[4] = rowfiles.cityname;
                    dtrow[5] = rowfiles.retailertype;
                    dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                    dtrow[7] = Convert.ToDateTime(rowfiles.monthss);
                    dtNewTable.Rows.Add(dtrow);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/CityMonthWiseRpt.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "CityMonthWise.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("CityMonthWiseRetailerReport_{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }
            
        }
        #endregion

        #region City Wise Month Wise Intake Report
        [HttpGet]
        public ActionResult CityMonthWiseIntakeReport(string StartingDate, string EndingDate, string TID, string Shoptypeid, string message)
        {
            try
            {
                string[] mul = null;
                try
                {
                    mul = message.Split(',');
                    if (mul.Length == 1)
                    {
                        mul = null;
                    }
                }
                catch (Exception exe)
                {
                }
                TID = TID == "" ? null : TID;
                Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
                SqlDataAdapter sda;
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                var monthwise = objRetailers.CityMonthWiseIntake(start, end, TID, Shoptypeid, mul).OrderBy(m => Convert.ToDateTime(m.monthss)).ThenBy(m => m.cityname);
                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol8 = new DataColumn("monthss", typeof(System.DateTime));
                dtNewTable.Columns.Add(dcol8);
                foreach (var rowfiles in monthwise)
                {
                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                    dtrow[1] = rowfiles.retailername;
                    dtrow[2] = rowfiles.saleofficername;
                    dtrow[3] = rowfiles.shopname;
                    dtrow[4] = rowfiles.cityname;
                    dtrow[5] = rowfiles.retailertype;
                    dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                    dtrow[7] = Convert.ToDateTime(rowfiles.monthss);
                    dtNewTable.Rows.Add(dtrow);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/CityMonthWiseIntakeReport.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "CityMonthWiseIntakeReport.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("CityMonthWiseIntakeReport_{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }
        #endregion       

        #region Retail Shop Wise Date Wise Intake Delivered Report
        public ActionResult RetailerShopDateWiseReport(string StartingDate, string EndingDate, string CID, string Shoptypeid, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            CID = CID == "" ? null : CID;
            Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime Start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime End = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            var rlist = objRetailers.ShopGetRetailersForExportinExcelReportC(Start, End, CID, Shoptypeid, mul);
            //var rlist = objRetailers.GetRetailersForExportinExcelReport(Start, End);
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("visiteddate", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in rlist)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.visiteddate);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/ShopDateWise.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ShopDateWise.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("RetailerShopDateWise_{0}.xls", DateTime.Now.ToShortDateString()));
            }

            return View();
        }

        #endregion

        #region Retail Shop Wise Date Wise Intake Report
        public ActionResult RetailShopDateWiseIntakReport(string StartingDate, string EndingDate, string CID, string Shoptypeid, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            CID = CID == "" ? null : CID;
            Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime Start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime End = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            var rlist = objRetailers.RetailShopDateWiseIntake(Start, End, CID, Shoptypeid, mul);
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("visiteddate", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in rlist)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.visiteddate);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/ShopDateWise.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ShopDateWise.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("RetailerShopDateWise_{0}.xls", DateTime.Now.ToShortDateString()));
            }

            return View();
        }

        #endregion

        #region Retail Shop Wise Month Wise Intake Delivered Report
       
        [HttpGet]
        public ActionResult RetailerShopMonthWiseReport(string StartingDate, string EndingDate, string CID, string Shoptypeid, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            CID = CID == "" ? null : CID;
            Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
            SqlDataAdapter sda;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            var monthwise = objRetailers.ShopGetRetailersForMonthWise(start, end, CID, Shoptypeid, mul);
            //var monthwise = objRetailers.GetRetailersForMonthWise(start, end).OrderBy(m => Convert.ToDateTime(m.monthss)).ThenBy(m => m.cityname);
            rptDataSet obj = new rptDataSet();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("monthss", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in monthwise)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.monthss);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/ShopMonthWiseRpt.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ShopMonthWiseRpt.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("RetailerShopMonthWiseReport_{0}.xls", DateTime.Now.ToShortDateString()));
            }

            return View();
        }
        #endregion

        #region Retail Shop Wise Month Wise Intake Report

        [HttpGet]
        public ActionResult RetailShopMonthWiseIntakReport(string StartingDate, string EndingDate, string CID, string Shoptypeid, string message)
        {
            string[] mul = null;
            try
            {
                mul = message.Split(',');
                if (mul.Length == 1)
                {
                    mul = null;
                }
            }
            catch (Exception exe)
            {
            }
            CID = CID == "" ? null : CID;
            Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
            SqlDataAdapter sda;
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            var monthwise = objRetailers.RetailShopMonthWiseIntake(start, end, CID, Shoptypeid, mul);
            rptDataSet obj = new rptDataSet();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("monthss", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in monthwise)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.monthss);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/ShopMonthWiseRpt.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ShopMonthWiseRpt.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("RetailerShopMonthWiseReport_{0}.xls", DateTime.Now.ToShortDateString()));
            }

            return View();
        }
        #endregion

        #region City Fos Wise Report

        [HttpGet]
        public ActionResult CityWiseFosReport(string StartingDate, string TID, string message)
        {
            try
            {
                string[] mul = null;
                try
                {
                    mul = message.Split(',');
                    if (mul.Length == 1)
                    {
                        mul = null;
                    }
                }
                catch (Exception exe)
                {
                }
                TID = TID == "" ? null : TID;
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                var monthwise = objRetailers.getCityWiseFOS(start, TID, mul);


                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol3 = new DataColumn("cityname", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol5 = new DataColumn("saleofficername", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol7 = new DataColumn("till_last_mnth_sale", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("crnt_mnth_sale", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol8 = new DataColumn("avg_last_two_mnth_sale", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol8);
                foreach (var rowfiles in monthwise)
                {
                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = rowfiles.cityname;
                    dtrow[1] = rowfiles.saleofficername;
                    dtrow[2] = Convert.ToInt32(rowfiles.till_last_mnth_sale);
                    dtrow[3] = Convert.ToInt32(rowfiles.crnt_mnth_sale);
                    dtrow[4] = Convert.ToInt32(rowfiles.avg_last_two_mnth_sale);
                    dtNewTable.Rows.Add(dtrow);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/CityFosWise.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "CityFosWise.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/xls", "CityFosWise.xls");
                }
                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "City Market Retailer Wise Report Not Working");
                return null;
            }
        }

        #endregion

        #region City Market Retailer Wise Report

        [HttpGet]
        public ActionResult CityMktRtlrWiseReport(string StartingDate, string CID, string Shoptypeid, string message)
        {
            try
            {
                string[] mul = null;
                try
                {
                    mul = message.Split(',');
                    if (mul.Length == 1)
                    {
                        mul = null;
                    }
                }
                catch (Exception exe)
                {
                }
                CID = CID == "" ? null : CID;
                Shoptypeid = Shoptypeid == "" ? null : Shoptypeid;
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                //var monthwise = objRetailers.ShopGetRetailersForMonthWise(start, end, CID, Shoptypeid, mul);
                var monthwise = objRetailers.getCityMktRtlrWise(start, CID, Shoptypeid, mul);


                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol3 = new DataColumn("cityname", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol5 = new DataColumn("market", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol4 = new DataColumn("retailer", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol1 = new DataColumn("category", typeof(System.String));
                dtNewTable.Columns.Add(dcol1);
                DataColumn dcol7 = new DataColumn("till_last_mnth_sale", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("crnt_mnth_sale", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol8 = new DataColumn("avg_last_two_mnth_sale", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol8);
                foreach (var rowfiles in monthwise)
                {
                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = rowfiles.cityname;
                    dtrow[1] = rowfiles.market;
                    dtrow[2] = rowfiles.retailer;
                    dtrow[3] = rowfiles.category;
                    dtrow[4] = Convert.ToInt32(rowfiles.till_last_mnth_sale);
                    dtrow[5] = Convert.ToInt32(rowfiles.crnt_mnth_sale);
                    dtrow[6] = Convert.ToInt32(rowfiles.avg_last_two_mnth_sale);
                    dtNewTable.Rows.Add(dtrow);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/CityMktRtlrWise.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "CityMktRtlrWise.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/xls", "CityMktRtlrWise.xls");
                }
                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "City Market Retailer Wise Report Not Working");
                return null;
            }
            
            
        }

        #endregion

        #region Retailer Points Summary report PR1 & PR2

        [HttpGet]
        public ActionResult RetailerPointSummary()
        {
            ManageRetailer objRetailers = new ManageRetailer();
            var getData = objRetailers.getRetailersPointSummary();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol3 = new DataColumn("cityName", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol5 = new DataColumn("RetailerName", typeof(System.String));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol4 = new DataColumn("RetailerType", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol1 = new DataColumn("CNIC", typeof(System.String));
            dtNewTable.Columns.Add(dcol1);
            DataColumn dcol7 = new DataColumn("Claim_Amount", typeof(System.Decimal));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol8 = new DataColumn("Transferred_Amount", typeof(System.Decimal));
            dtNewTable.Columns.Add(dcol8);
            DataColumn dcol2 = new DataColumn("Balance_Amount", typeof(System.Decimal));
            dtNewTable.Columns.Add(dcol2);
            foreach (var rowfiles in getData)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = rowfiles.cityName;
                dtrow[1] = rowfiles.RetailerName;
                dtrow[2] = rowfiles.RetailerType;
                dtrow[3] = rowfiles.CNIC;
                dtrow[4] = Convert.ToDecimal(rowfiles.Claim_Amount);
                dtrow[5] = Convert.ToDecimal(rowfiles.Transferred_Amount);
                dtrow[6] = Convert.ToDecimal(rowfiles.Balance_Amount);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/RetailerPointSummary.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "RetailerPointSummaryReport.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/xls", "RetailerPointSummaryReport.xls");
            }
            return View();
        }
        #endregion

        #region POS availability report MR1

        [HttpGet]
        public ActionResult PosAvailabilityReport(string StartingDate, string EndingDate)
        {
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime start = Convert.ToDateTime(StartingDate);
            DateTime end = Convert.ToDateTime(EndingDate);
            var objData = objRetailers.PosAvailability(start, end);
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol3 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol5 = new DataColumn("market", typeof(System.String));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol4 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol1 = new DataColumn("category", typeof(System.String));
            dtNewTable.Columns.Add(dcol1);
            DataColumn dcol7 = new DataColumn("fos", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol8 = new DataColumn("pos_availability", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in objData)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = rowfiles.cityname;
                dtrow[1] = rowfiles.market;
                dtrow[2] = rowfiles.shopname;
                dtrow[3] = rowfiles.category;
                dtrow[4] = rowfiles.fos; ;
                dtrow[5] = Convert.ToInt32(rowfiles.pos_availability);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/PosAvailabilityReport.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "PosAvailabilityReport.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/xls", "PosAvailabilityReport.xls");
            }
            return View();
        }

        #endregion

        #region City Wise Painters report CW4

        [HttpGet]
        public ActionResult CityWisePaintersReport(string StartingDate)
        {
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            var objData = objRetailers.getCityWisePainters(start);
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol3 = new DataColumn("city", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol5 = new DataColumn("pname", typeof(System.String));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol4 = new DataColumn("cnic", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol1 = new DataColumn("points_redeemed_1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol1);
            DataColumn dcol7 = new DataColumn("points_redeemed_5kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol8 = new DataColumn("total_points_redeemed", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol8);
            DataColumn dcol6 = new DataColumn("points_transferred", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol9 = new DataColumn("balance_points", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol9);
            foreach (var rowfiles in objData)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = rowfiles.city;
                dtrow[1] = rowfiles.pname;
                dtrow[2] = rowfiles.cnic;
                dtrow[3] = Convert.ToInt32(rowfiles.points_redeemed_1kg);
                dtrow[4] = Convert.ToInt32(rowfiles.points_redeemed_5kg);
                dtrow[5] = Convert.ToInt32(rowfiles.total_points_redeemed);
                dtrow[6] = Convert.ToInt32(rowfiles.points_transferred);
                dtrow[7] = Convert.ToInt32(rowfiles.balance_points);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/CityWisePaintersReport.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "CityWisePaintersReport.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/xls", "CityWisePaintersReport.xls");
            }
            return View();
        }
        #endregion

        #region Update Approval Retailer

        [CustomAuthorize]
        // View 
        public ActionResult UpdateApproval()
        {
            // Load Dealer Data ...
            RetailerPendingData objRetailer = new RetailerPendingData();
            List<DealerData> DealerObj = ManageDealer.GetDealerList();
            objRetailer.Dealers = DealerObj;
            Common.CacheManager.Store("DealerList", DealerObj);

            return View(objRetailer);
        }


        // Update Retailer From Pending Retailers 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateApprovalSave(RetailerPendingData objRetailer)
        {
            try
            {
                String[] strRetailerID = objRetailer.strRetailerID.TrimEnd(',').Split(',');
                String[] strDealerID = objRetailer.strDealerID.TrimEnd(',').Split(',');
                var RetailersAndDealers = strRetailerID.Zip(strDealerID, (r, d) => new { Retailer = r, Dealer = d });
                //foreach (var RetailerID in strRetailerID)
                //{
                //    foreach (var DealerID in strDealerID)
                //    {
                //        if (ManageRetailer.UpdateDealerID(Convert.ToInt32(DealerID) , Convert.ToInt32(RetailerID)))
                //        {
                //        }
                //    }
                //}

                foreach (var RD in RetailersAndDealers)
                {
                    ManageRetailer.ApproveRetailerForDelete(Convert.ToInt32(RD.Retailer), RetActionEnum.Update, SessionManager.Get<int>("UserID"));
                }


                return Content("1");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }


        // 
        public String UpdateApprovalApproved(int TID)
        {
            String strFlag = "0";
            try
            {
                int res = ManageRetailer.ApproveRetailerForApproval(TID, RetActionEnum.Update, SessionManager.Get<int>("UserID"));

                if(res == 0)
                {
                    strFlag = "1";
                }
            }
            catch (Exception)
            {
                strFlag = "0";
            }

            return strFlag;
        }


        // Pending Retailer Data Handler
        public JsonResult UpdateApprovalDataHandler()
        {
            try
            {
                var dtsource = new List<RetailerPendingData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.Update);
                }
                else
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.Update, RegionalheadID);
                }
                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        // Delete Retailer
        public int DeleteUpdateApproval(int TID)
        {
            return ManageRetailer.DeleteRetailerForApproval(TID, SessionManager.Get<int>("UserID"));
        }


        #endregion
        
        #region Proposed additions Approval Retailer

        [CustomAuthorize]
        // View 
        public ActionResult ProposedAddApproval()
        {
            // Load Dealer Data ...
            RetailerPendingData objRetailer = new RetailerPendingData();
            List<DealerData> DealerObj = ManageDealer.GetDealerList();
            objRetailer.Dealers = DealerObj;
            Common.CacheManager.Store("DealerList", DealerObj);

            return View(objRetailer);
        }


        // Update Retailer From Pending Retailers 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProposedAddApprovalSave(RetailerPendingData objRetailer)
        {
            try
            {
                String[] strRetailerID = objRetailer.strRetailerID.TrimEnd(',').Split(',');
                String[] strDealerID = objRetailer.strDealerID.TrimEnd(',').Split(',');
                var RetailersAndDealers = strRetailerID.Zip(strDealerID, (r, d) => new { Retailer = r, Dealer = d });
                //foreach (var RetailerID in strRetailerID)
                //{
                //    foreach (var DealerID in strDealerID)
                //    {
                //        if (ManageRetailer.UpdateDealerID(Convert.ToInt32(DealerID) , Convert.ToInt32(RetailerID)))
                //        {
                //        }
                //    }
                //}

                foreach (var RD in RetailersAndDealers)
                {
                    ManageRetailer.ApproveRetailerForApproval(Convert.ToInt32(RD.Retailer), RetActionEnum.Add, SessionManager.Get<int>("UserID"));
                }


                return Content("1");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }


        // Update Retailer
        public String ProposedAddApprovalApproved(int TID)
        {
            String strFlag = "0";
            try
            {
                int res = ManageRetailer.ApproveRetailerForApproval(TID, RetActionEnum.Add, SessionManager.Get<int>("UserID"));
                if (res == 0)
                {
                    strFlag = "1";
                }
            }
            catch (Exception)
            {
                strFlag = "0";
            }

            return strFlag;
        }


        // Pending Retailer Data Handler
        public JsonResult ProposedAddApprovalDataHandler()
        {
            try
            {
                var dtsource = new List<RetailerPendingData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.Add);
                }
                else
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.Add, RegionalheadID);
                }
                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        // Delete Retailer
        public int DeleteProposedAddApproval(int TID)
        {
            return ManageRetailer.DeleteRetailerForApproval(TID, SessionManager.Get<int>("UserID"));
        }


        #endregion

        #region Reset Locations Approval Retailer

        [CustomAuthorize]
        // View 
        public ActionResult ResetLocApproval()
        {
            // Load Dealer Data ...
            RetailerPendingData objRetailer = new RetailerPendingData();
            List<DealerData> DealerObj = ManageDealer.GetDealerList();
            objRetailer.Dealers = DealerObj;
            Common.CacheManager.Store("DealerList", DealerObj);

            return View(objRetailer);
        }


        // Update Retailer From Pending Retailers 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetLocApprovalSave(RetailerPendingData objRetailer)
        {
            try
            {
                String[] strRetailerID = objRetailer.strRetailerID.TrimEnd(',').Split(',');
                String[] strDealerID = objRetailer.strDealerID.TrimEnd(',').Split(',');
                var RetailersAndDealers = strRetailerID.Zip(strDealerID, (r, d) => new { Retailer = r, Dealer = d });
                //foreach (var RetailerID in strRetailerID)
                //{
                //    foreach (var DealerID in strDealerID)
                //    {
                //        if (ManageRetailer.UpdateDealerID(Convert.ToInt32(DealerID) , Convert.ToInt32(RetailerID)))
                //        {
                //        }
                //    }
                //}

                foreach (var RD in RetailersAndDealers)
                {
                    ManageRetailer.ResetRetailerLatLongApproval(Convert.ToInt32(RD.Retailer));
                }


                return Content("1");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }


        // Update Retailer
        public String ResetLocApprovalApproved(int TID)
        {
            String strFlag = "0";
            try
            {
                int res = ManageRetailer.ResetRetailerLatLongApproval(TID);

                if(res == 0)
                {
                    strFlag = "1";
                }
            }
            catch (Exception)
            {
                strFlag = "0";
            }

            return strFlag;
        }


        // Pending Retailer Data Handler
        public JsonResult ResetLocApprovalDataHandler()
        {
            try
            {
                var dtsource = new List<RetailerPendingData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.ResetLoc);
                }
                else
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.ResetLoc, RegionalheadID);
                }
                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        // Delete Retailer
        public int ResetLocApprovalApproval(int TID)
        {
            return ManageRetailer.DeleteRetailerForApproval(TID, SessionManager.Get<int>("UserID"));
        }


        #endregion

        #region Update Locations Approval Retailer

        [CustomAuthorize]
        // View 
        public ActionResult UpdateLocApproval()
        {
            // Load Dealer Data ...
            RetailerPendingData objRetailer = new RetailerPendingData();
            List<DealerData> DealerObj = ManageDealer.GetDealerList();
            objRetailer.Dealers = DealerObj;
            Common.CacheManager.Store("DealerList", DealerObj);

            return View(objRetailer);
        }


        // Update Retailer From Pending Retailers 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateLocApprovalSave(RetailerPendingData objRetailer)
        {
            try
            {
                String[] strRetailerID = objRetailer.strRetailerID.TrimEnd(',').Split(',');
                String[] strDealerID = objRetailer.strDealerID.TrimEnd(',').Split(',');
                var RetailersAndDealers = strRetailerID.Zip(strDealerID, (r, d) => new { Retailer = r, Dealer = d });
                //foreach (var RetailerID in strRetailerID)
                //{
                //    foreach (var DealerID in strDealerID)
                //    {
                //        if (ManageRetailer.UpdateDealerID(Convert.ToInt32(DealerID) , Convert.ToInt32(RetailerID)))
                //        {
                //        }
                //    }
                //}

                foreach (var RD in RetailersAndDealers)
                {
                    if (ManageRetailer.UpdateDealerID(Convert.ToInt32(RD.Retailer)))
                    {
                    }
                }


                return Content("1");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }


        // Update Retailer
        public String UpdateLocApprovalApproved(int ID)
        {
            String strFlag = "0";
            try
            {
                if (ManageRetailer.UpdateDealerID(ID))
                {
                    strFlag = "1";
                }
            }
            catch (Exception)
            {
                strFlag = "0";
            }

            return strFlag;
        }


        // Pending Retailer Data Handler
        public JsonResult UpdateLocApprovalDataHandler()
        {
            try
            {
                var dtsource = new List<RetailerPendingData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.UpdateLoc);
                }
                else
                {
                    dtsource = ManageRetailer.GetPendingRetailerForApprovalGrid(RetActionEnum.UpdateLoc, RegionalheadID);
                }
                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        // Delete Retailer
        public int UpdateLocApprovalApproval(int ID)
        {
            return ManageRetailer.DeleteRetailer(ID);
        }


        #endregion

        #region export to excel reports

        public void ExportToCSV(int rptType, int regHdId, int distId, int soId, int cityId, int zoneId, string month)
        {
            
            // Example data
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Retailer Name\",\"Shop Name\",\"Dealer Name\",\"Sales Officer Name\",\"City Name\",\"Area Name\",\"Address\",\"Phone1\",\"Phone2\",\"Retailer Type\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Retailers" + DateTime.Now + ".csv");
            Response.ContentType = "application/octet-stream";

            List<RetailerData> retailers = null;
            if (rptType == 1)
            {
                retailers = FOS.Setup.ManageRetailer.GetRetailerLocations(regHdId, distId, soId, 0, cityId, zoneId);
            }
            else if (rptType == 2)
            {
                retailers = FOS.Setup.ManageRetailer.GetRetailerLocations(regHdId, distId, soId, 0, cityId, zoneId, "untagged");
            }
            else if (rptType == 3 || rptType == 4)
            {
                retailers = new List<RetailerData>();
                var plannedDealers = ManageDealer.PlannedDistributors(new PlannedRetailerFilter
                {
                    RegionalHeadID = regHdId,
                    DealerID = distId,
                    CityID = cityId,
                    month = month
                });

                foreach (var dealer in plannedDealers)
                {
                    if (rptType == 3)
                    {
                        if (dealer.RetailersPlanned.Count > 0)
                        {
                            foreach (var ret in dealer.RetailersPlanned)
                            {
                                retailers.Add(ManageRetailer.GetRetailerByID(ret.ID));
                            }
                        }
                    }
                    if (rptType == 4)
                    {
                        if (dealer.RetailersUnplanned.Count > 0)
                        {
                            foreach (var ret in dealer.RetailersUnplanned)
                            {
                                retailers.Add(ManageRetailer.GetRetailerByID(ret.ID));
                            }
                        }
                    }
                }
            }

            foreach (var retailer in retailers)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",

                retailer.Name,
                retailer.ShopName,
                retailer.DealerName,
                retailer.SaleOfficerName,
                retailer.CItyName,
                retailer.AreaName,
                retailer.Address,

                retailer.Phone1,
                retailer.Phone2,
                retailer.RetailerType
                ));

            }
            Response.Write(sw.ToString());
            Response.End();
        }


        #endregion

        #region Delete Retailer
        
        public int UndoRetailer(int ID)
        {
            return ManageRetailer.UndoRetailer(ID);
        }
        public String  DeleteApprovalApproved(int TID)
        {
            String strFlag = "0";
            try
            {
                int res = ManageRetailer.ApproveRetailerForDelete(TID, RetActionEnum.Update, SessionManager.Get<int>("UserID"));

                if (res == 0)
                {
                    strFlag = "1";
                }
            }
            catch (Exception)
            {
                strFlag = "0";
            }

            return strFlag;
        }
        public JsonResult DeleteApprovalDataHandler()
        {
            try
            {
                var dtsource = new List<RetailerPendingData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetPendingRetailerForDelete(RetActionEnum.Update);
                }
                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [CustomAuthorize]
        // View 
        public ActionResult DeleteRetailerApproval()
        {
            // Load Dealer Data ...
            //RetailerPendingData objRetailer = new RetailerPendingData();
            //List<DealerData> DealerObj = ManageDealer.GetDealerList();
            //objRetailer.Dealers = DealerObj;
            //Common.CacheManager.Store("DealerList", DealerObj);

            return View();
        }

        #endregion



        #region SiteEquipment

        [CustomAuthorize]
        // View
        public ActionResult SiteEquipmentDetail()
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

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetSaleOfficerListByRegionalHeadID(regId);
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            var objRetailer = new SiteEquipmentDetailData();
            objRetailer.Client = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Dealers = DealerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.EquipmentBrand = ManageRetailer.GetEquipmentBrandList();
            objRetailer.EquipmentCategory = ManageRetailer.GetEquipmentCategoryList();
            var cat = objRetailer.EquipmentCategory.FirstOrDefault();

            objRetailer.EquipmentType = ManageRetailer.GetEquipmentTypeList(cat.ID);
            return View(objRetailer);
        }


        // Add Or Update Retailer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUpdateSiteEquipment([Bind(Exclude = "TID,SaleOfficers,Dealers")] SiteEquipmentDetailData newRetailer)


        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRetailer != null)
                {
                  

                    if (boolFlag)
                    {
                       

                        int Res = ManageRetailer.AddUpdateSiteEquipment(newRetailer);

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


        public JsonResult SiteEquipmentDataHandler(DTParameters param, Int32 SiteID)
        {
            try
            {
                var dtsource = new List<SiteEquipmentDetailData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (RegionalheadID == 0)
                {
                    dtsource = ManageRetailer.GetSiteDetailForGrid(SiteID);
                }
             
                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SiteEquipmentDetailData> data = ManageRetailer.GetResultForSiteDetail(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRetailer.CountForSiteDetail(param.Search.Value, dtsource, columnSearch);
                DTResult<SiteEquipmentDetailData> result = new DTResult<SiteEquipmentDetailData>
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


        public JsonResult GetEditSiteDetail(int RetailerID)
        {
            var Response = ManageRetailer.GetEditSiteDetailInfo(RetailerID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
    public class LatLongModel
    {
        public int RegionalHeadID { get; set; }
        public int DealerID { get; set; }
        public int SaleOfficerID { get; set; }
        public int RegionID { get; set; }
        public int CityID { get; set; }
        public int ZoneID { get; set; }
    }
}