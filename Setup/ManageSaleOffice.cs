using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageSaleOffice
    {

        // Get SalesOfficer Area Names With Line Break ...
        public static string GetSaleOfficerAreaName(int intSaleOfficerID)
        {
            String strAreaName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == intSaleOfficerID).FirstOrDefault();
            strAreaName = string.Join("<br/>", objSaleOfficer.Areas.Select(p => p.Name));

            return strAreaName;
        }

        public static List<SaleOfficerData> GetSaleOfficerListRelatedtoregionalHeadID(int? RHID, bool selectOption = false)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RHID == 0 && !selectOption)
                {
                    //  saleOfficerData = dbContext.SaleOfficers.OrderBy(p => p.Name).ToList();
                }
                else
                {
                    var list = dbContext.Tbl_SOREGIONS.Where(x => x.RegionID == RHID).Select(x => x.SaleofficerID).ToList();

                    saleOfficerData = dbContext.SaleOfficers.Where(c => list.Contains(c.ID))
                           .Select(
                               u => new SaleOfficerData
                               {
                                   ID = u.ID,
                                   Name = u.Name,

                               }).ToList();
                }

            }
            return saleOfficerData;
        }
        // Get SalesOfficer Area ID With Comma Seperator ...
        public static string GetSaleOfficerAreaID(int intSaleOfficerID)
        {
            String strAreaName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == intSaleOfficerID).FirstOrDefault();
            strAreaName = string.Join(",", objSaleOfficer.Areas.Select(p => p.ID));

            return strAreaName;
        }
        public static List<SaleOfficerData> GetAllSO()
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.SaleOfficers.ToList();

                foreach (var so in list)
                {
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }

        // Get All Regions Related To SalesOfficer ...
        public static List<RegionData> GetRegionRelatedToSaleOfficer(int SaleOfficerID, int RegionalHeadID)
        {
            List<RegionData> strRegionID = new List<RegionData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID && s.RegionalHeadID == RegionalHeadID).FirstOrDefault();
                    strRegionID = objSaleOfficer.RegionalHead.RegionalHeadRegions
                        .Where(r => r.RegionHeadID == RegionalHeadID)
                        .Select(u => new RegionData
                        {
                            RegionID = u.RegionID,
                            Name = u.Region.Name
                        }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }


            return strRegionID;
        }

        public static List<RetailerData> GetDistributorRetailedtoSOForDistributor(int? Region, int? CityID)
        {
            List<RetailerData> saleOfficerData = new List<RetailerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.Dealers.Where(u => u.RegionID == Region && u.CityID== CityID &&  u.IsActive==true).OrderBy(x=>x.ShopName).ToList();

                foreach (var so in list)
                {

                    saleOfficerData.Add(new RetailerData
                    {
                        ID = so.ID,
                        ShopName = so.ShopName
                    });
                }
            }
            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetAllSORelatedToRegion(int? Region)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.SaleOfficers.Where(u => u.RegionID == Region).ToList();

                foreach (var so in list)
                {
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetAllSORelatedToRegionForDistributor(int? Region)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.Tbl_SOREGIONS.Where(u => u.RegionID == Region).ToList();

                foreach (var so in list)
                {
                    var saO = dbContext.SaleOfficers.Where(x => x.ID == so.SaleofficerID).FirstOrDefault();
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.SaleofficerID,
                        Name = saO.Name
                    });
                }
            }
            return saleOfficerData;
        }


        public static List<CityData> GetAllCitiesRelatedToRegionForDistributor(int? Region)
        {
            List<CityData> saleOfficerData = new List<CityData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.Cities.Where(u => u.RegionID == Region && u.IsActive==true).ToList();

                foreach (var so in list)
                {
                   // var saO = dbContext.SaleOfficers.Where(x => x.ID == so.SaleofficerID).FirstOrDefault();
                    saleOfficerData.Add(new CityData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }
        public static List<SaleOfficerData> GetSaleOfficerByRegionalHeadID(int RegionalHeadID, bool orderBy, bool comboBox)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            if (comboBox)
            {
                saleOfficerData.Add(new SaleOfficerData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var list = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).OrderBy(p => p.Name).ToList();

                    foreach (var item in list)
                    {
                        saleOfficerData.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return saleOfficerData;
        }

        // Get All SalesOfficer Related To CityID ...
        public static List<SaleOfficerData> GetSaleOfficerByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return saleOfficerData;
        }

        // Get All SalesOfficers List For Grid ...
        public static List<SaleOfficerData> GetSaleOfficerList(bool orderby = false)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (orderby)
                    {
                        saleOfficerData = dbContext.SaleOfficers.Where(u => u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    UserName = u.UserName,
                                    Password = u.Password,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).OrderBy(x => x.Name).ToList();
                    }
                    else
                    {
                        saleOfficerData = dbContext.SaleOfficers.Where(u => u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    UserName = u.UserName,
                                    Password = u.Password,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                    }

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return saleOfficerData;
        }

        // Get All SalesOfficers List For Grid ...
        public static List<SaleOfficerData> GetSaleOfficerListForGrid(int RegionalHeadType, int RegionalHeadID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.RegionalHead.Type == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    UserName = u.UserName,
                                    Password = u.Password,
                                    RegionalHeadID = u.RegionalHeadID,
                                    RegionalHeadName = dbContext.RegionalHeads.Where(x => x.ID == RegionalHeadID).Select(x => x.Name).FirstOrDefault(),
                                    SoRoleID =(int)u.RoleID,
                                    SaleOfficersProjects =dbContext.SOProjects.Where(x=>x.SaleOfficerID== u.ID).Select(x=>x.ProjectID).ToList(),
                                    SOZones= dbContext.SOZoneAndTowns.Where(x => x.SOID == u.ID).Select(x => x.CityID).Distinct().ToList(),
                                    //SOTowns = dbContext.SOZoneAndTowns.Where(x => x.SOID == u.ID).Select(x => x.AreaID).Distinct().ToList(),
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return saleOfficerData;
        }

        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerList()
        {
            List<SaleOfficer> saleOfficerData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                saleOfficerData = dbContext.SaleOfficers.ToList();
            }
            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetSOByRegionalHeadId(int RHID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.SaleOfficers.Where(s => s.RegionalHeadID == RHID).ToList();

                foreach (var so in list)
                {
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }
        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerListRelatedtoregionalHeadID(int RHID, bool selectOption = false)
        {
            List<SaleOfficer> saleOfficerData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RHID == 0 && !selectOption)
                {
                    saleOfficerData = dbContext.SaleOfficers.OrderBy(p => p.Name).ToList();
                }
                else
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(s => s.RegionalHeadID == RHID).OrderBy(p => p.Name).ToList();
                }

            }
            return saleOfficerData;
        }




        //Insert OR Update SalesOfficers ...
        public static Boolean AddUpdateSaleOfficer(SaleOfficerData obj)
        {
            Boolean boolFlag = false;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SOZoneAndTown regionalHeadRegionObj;
                    SaleOfficer saleofficerObj = new SaleOfficer();

                    if (obj.ID == 0)
                    {
                        saleofficerObj.ID = dbContext.SaleOfficers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        saleofficerObj.Name = obj.Name;
                        saleofficerObj.UserName = obj.UserName;
                        saleofficerObj.Password = obj.Password;
                        saleofficerObj.RegionalHeadID = obj.HiddenRegionalHeadID;
                        saleofficerObj.CityID = 278;
                        saleofficerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                        saleofficerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                        saleofficerObj.IsActive = true;
                        saleofficerObj.IsDeleted = false;
                        saleofficerObj.RegionID = obj.RegionID;
                        saleofficerObj.CreatedDate = DateTime.Now;
                        saleofficerObj.LastUpdate = DateTime.Now;
                        saleofficerObj.RoleID = obj.SoRoleID;
                        //Created By Work Pending...
                        saleofficerObj.CreatedBy = 1;

                        dbContext.SaleOfficers.Add(saleofficerObj);

                       

                        String[] strRegionId = obj.RegionIDD.Split(',');
                        int[] myInts = strRegionId.Select(int.Parse).ToArray();
                        String[] strTownId = obj.TownIDD.Split(',');
                        int[] myIntss = strTownId.Select(int.Parse).ToArray();
                        String[] proId = obj.ProjectIDD.Split(',');
                        int[] proIds = proId.Select(int.Parse).ToArray();
                        foreach (var regionid in myInts)
                        { var list = dbContext.Areas.Where(x => x.CityID == regionid).ToList();

                            foreach (var item in list)
                            {
                                foreach (var items in myIntss)
                                {
                                    if (items == item.ID)
                                    {
                                        regionalHeadRegionObj = new SOZoneAndTown();
                                        regionalHeadRegionObj.SOID = saleofficerObj.ID;
                                        regionalHeadRegionObj.CityID = Convert.ToInt32(regionid);
                                        regionalHeadRegionObj.AreaID = items;



                                    

                                        dbContext.SOZoneAndTowns.Add(regionalHeadRegionObj);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                         
                        }

                        foreach (var item in proIds)
                        {
                            SOProject project = new SOProject();
                            project.ProjectID = item;
                            project.SaleOfficerID = saleofficerObj.ID;
                            dbContext.SOProjects.Add(project);
                            dbContext.SaveChanges();
                        }

                        

                       
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        saleofficerObj = dbContext.SaleOfficers.Where(u => u.ID == obj.ID).FirstOrDefault();
                        saleofficerObj.Name = obj.Name;
                        saleofficerObj.UserName = obj.UserName;
                        saleofficerObj.Password = obj.Password;
                        saleofficerObj.RegionalHeadID = obj.HiddenRegionalHeadID;
                        saleofficerObj.CityID = 278;
                        saleofficerObj.RoleID = obj.SoRoleID;
                        saleofficerObj.RegionID = obj.RegionID;
                        saleofficerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                        saleofficerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                        saleofficerObj.LastUpdate = DateTime.Now;

                        dbContext.SaveChanges();

                        String[] strRegionId = obj.RegionIDD.Split(',');
                        int[] myInts = strRegionId.Select(int.Parse).ToArray();
                        String[] strTownId = obj.TownIDD.Split(',');
                        int[] myIntss = strTownId.Select(int.Parse).ToArray();
                        String[] proId = obj.ProjectIDD.Split(',');
                        int[] proIds = proId.Select(int.Parse).ToArray();

                        var listing = dbContext.SOZoneAndTowns.Where(x => x.SOID == saleofficerObj.ID).ToList();
                        var SOProject = dbContext.SOProjects.Where(x => x.SaleOfficerID == saleofficerObj.ID).ToList();
                        foreach (var item in listing)
                        {
                            dbContext.SOZoneAndTowns.Remove(item);
                            dbContext.SaveChanges();
                        }

                        foreach (var item in SOProject)
                        {
                            dbContext.SOProjects.Remove(item);
                            dbContext.SaveChanges();
                        }


                        foreach (var regionid in myInts)
                        {
                            var list = dbContext.Areas.Where(x => x.CityID == regionid).ToList();

                            foreach (var item in list)
                            {
                                foreach (var items in myIntss)
                                {
                                    if (items == item.ID)
                                    {
                                        regionalHeadRegionObj = new SOZoneAndTown();
                                        regionalHeadRegionObj.SOID = saleofficerObj.ID;
                                        regionalHeadRegionObj.CityID = Convert.ToInt32(regionid);
                                        regionalHeadRegionObj.AreaID = items;





                                        dbContext.SOZoneAndTowns.Add(regionalHeadRegionObj);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }

                        }

                        foreach (var item in proIds)
                        {
                            SOProject project = new SOProject();
                            project.ProjectID = item;
                            project.SaleOfficerID = saleofficerObj.ID;
                            dbContext.SOProjects.Add(project);
                            dbContext.SaveChanges();
                        }
                    }

                    boolFlag = true;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add SalesOfficer Failed");
                boolFlag = false;
            }
            return boolFlag;
        }

        // Delete SalesOfficer ...
        public static int DeleteSaleOfficer(int SaleOfficerID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SaleOfficer objSaleOfficer = dbContext.SaleOfficers.Where(u => u.ID == SaleOfficerID).FirstOrDefault();
                    var objSaleOfficerArea = objSaleOfficer.Areas.ToList();
                    foreach (var area in objSaleOfficerArea)
                    {
                        objSaleOfficer.Areas.Remove(area);
                    }
                    dbContext.SaleOfficers.Remove(objSaleOfficer);
                    //obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete SalesOfficer Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int Reset(int SaleOfficerID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SaleOfficer objSaleOfficer = dbContext.SaleOfficers.Where(u => u.ID == SaleOfficerID).FirstOrDefault();


                    objSaleOfficer.IMEI = null;
                    //obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete SalesOfficer Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static List<SaleOfficerData> GetResult(string search, string sortOrder, int start, int length, List<SaleOfficerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<SaleOfficerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<SaleOfficerData> FilterResult(string search, List<SaleOfficerData> dtResult, List<string> columnFilters)
        {
            IQueryable<SaleOfficerData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.AreaName != null && p.AreaName.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower())
                || p.Phone2 != null && p.Phone2.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[3] == null || (p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(columnFilters[3].ToLower())))
                 && (columnFilters[4] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[3].ToLower())))
                  && (columnFilters[5] == null || (p.AreaName != null && p.AreaName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[6] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[7] == null || (p.Phone2 != null && p.Phone2.ToLower().Contains(columnFilters[5].ToLower())))
                );

            return results;
        }

        public static List<RegionalHeadData> GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeads.Where(u => u.Type == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<SaleOfficerData> GetFS(int RegionalHeadType)
        {
            List<SaleOfficerData> FSUserName = new List<SaleOfficerData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    FSUserName = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    UserName = u.UserName,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return FSUserName;
        }


        public static List<RegionalHeadData> GetRegionsofSO(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeadRegions.Where(u => u.RegionHeadID == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.RegionID,
                                    Name = dbContext.Regions.Where(x => x.ID == u.RegionID).Select(x => x.Name).FirstOrDefault(),
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<SaleOfficerData> GetSaleOfficerListByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.Zones.Where(u => u.ClientId == RegionalHeadID).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).OrderBy(x => x.Name).ToList();


                }
            }
            catch (Exception)
            {
                throw;
            }
            saleOfficerData.Insert(0, new SaleOfficerData


            {
                ID = 0,
                Name = "--Select Project--"
            });

            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetSaleOfficerListByRegionalHeadID()
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.Zones.ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).OrderBy(x => x.Name).ToList();


                }
            }
            catch (Exception)
            {
                throw;
            }
            saleOfficerData.Insert(0, new SaleOfficerData


            {
                ID = 0,
                Name = "--Select Project--"
            });

            return saleOfficerData;
        }


        public static List<SaleOfficer> GetAllSaleOfficerListRelatedtoregionalHeadIDd()
        {
            List<SaleOfficer> saleOfficerData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                saleOfficerData = dbContext.SaleOfficers.OrderBy(p => p.Name).ToList();



            }
            return saleOfficerData;
        }



        public static List<SaleOfficerData> GetAllSaleOfficerListRelatedtoClient(int ID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                saleOfficerData = dbContext.SaleOfficers.Where(x=>x.RegionID==ID).Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).ToList(); ;



            }
            return saleOfficerData;
        }
        public static List<Sp_GetItemsRelatedToJobID_Result> GetItemsAcctoID(int JobID)
        {
            List<Sp_GetItemsRelatedToJobID_Result> regionalHeadData = new List<Sp_GetItemsRelatedToJobID_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetItemsRelatedToJobID(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }


        public static List<Sp_GetDistributorOrdersAccToJobIDFinal1_0_Result> GetDistriburorOrdersAcctoID(int JobID)
        {
            List<Sp_GetDistributorOrdersAccToJobIDFinal1_0_Result> regionalHeadData = new List<Sp_GetDistributorOrdersAccToJobIDFinal1_0_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetDistributorOrdersAccToJobIDFinal1_0(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }



        public static List<Sp_GetComplaintDetails1_1_Result> GetComplaintDetailsToIDList(int JobID)
        {
            List<Sp_GetComplaintDetails1_1_Result> regionalHeadData = new List<Sp_GetComplaintDetails1_1_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetComplaintDetails1_1(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }


        public static List<Sp_GetRetailerOrdersAccToJobID1_1_Result> GetRetailerOrdersAcctoID(int JobID)
        {
            List<Sp_GetRetailerOrdersAccToJobID1_1_Result> regionalHeadData = new List<Sp_GetRetailerOrdersAccToJobID1_1_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetRetailerOrdersAccToJobID1_1(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }


        public static List<Sp_GetItemsRelatedtoStock_Result> GetStockAcctoID(int JobID)
        {
            List<Sp_GetItemsRelatedtoStock_Result> regionalHeadData = new List<Sp_GetItemsRelatedtoStock_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetItemsRelatedtoStock(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<SaleOfficerData> GetProjectsData()
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.Zones
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).OrderBy(x => x.Name).ToList();


                }
             
            }
            catch (Exception)
            {
                throw;
            }
            saleOfficerData.Insert(0, new SaleOfficerData
            {
                ID = 0,
                Name = "All"
            });
            return saleOfficerData;
            
        }
        public static List<ComplaintStatus> GetProjectsListForDashboard(List<int> list)
        {
            List<ComplaintStatus> city = new List<ComplaintStatus>();
            ComplaintStatus comlist;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    foreach (var item in list)
                    {
                        comlist = new ComplaintStatus();
                        comlist.ID = item;
                        comlist.Name = dbContext.Cities.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                        city.Add(comlist);
                    }

                }
                //city.Insert(0, new ComplaintStatus
                //{
                //    ID = 0,
                //    Name = "--All--"
                //});

            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }


    }
}