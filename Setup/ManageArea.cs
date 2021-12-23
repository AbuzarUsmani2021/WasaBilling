using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageArea
    {

        // Get All Areas List ...
        public static List<AreaData> GetAreaList()
        {
            List<AreaData> area = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    area = dbContext.Areas
                            .Select(
                                u => new AreaData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    RegionID = u.RegionID,
                                    //RegionName = u.City.Region.Name,
                                    //CityID = u.CityID,
                                    //CityName = u.City.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            area.Insert(0, new AreaData
            {
                ID = 0,
                Name = "--Select Town--"
            });
            return area;
        }



        public static List<RetailerData> GetSitesList()
        {
            List<RetailerData> area = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    area = dbContext.Retailers
                            .Select(
                                u => new RetailerData
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

            return area;
        }
        public static int DeleteNationalUserAccess(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_Access obj = dbContext.Tbl_Access.Where(u => u.ID == ID).FirstOrDefault();
                    obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static int AddUpdateAccess(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_Access AreaObj = new Tbl_Access();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Tbl_Access.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.SaleOfficerID = obj.SOID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RepotedUP = obj.SOID1;
                            AreaObj.ReportedDown = obj.SOID2;
                            AreaObj.Status = true;
                            AreaObj.CreatedOn = DateTime.Now;
                            //  AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.IsDeleted = false;

                            dbContext.Tbl_Access.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Tbl_Access.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.SaleOfficerID = obj.SOID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RepotedUP = obj.RepotedTo;
                            AreaObj.ReportedDown = obj.ReportedFor;
                            AreaObj.Status = true;
                            AreaObj.CreatedOn = DateTime.Now;

                            AreaObj.IsDeleted = false;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Get All Areas List By CityID ...
        public static List<AreaData> GetAreaListByCityID()
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var query = from saleoff in dbContext.SaleOfficers
                                select saleoff.Areas;

                    var qArea = dbContext.SaleOfficers.SelectMany(p => p.Areas);

                    var area = from areas in dbContext.Areas
                               where
                               !
                                   ((from saleoffarea in qArea
                                     select new
                                     {
                                         saleoffarea.ID
                                     }).Distinct()).Contains(new { ID = areas.ID })
                               orderby areas.Name
                               select new AreaData()
                               {
                                   ID = areas.ID,
                                   Name = areas.Name,
                               };

                    return area.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int DeleteAccess(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_Access obj = dbContext.Tbl_Access.Where(u => u.ID == ID).FirstOrDefault();
                    dbContext.Tbl_Access.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int DeleteSORegions(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_SOREGIONS obj = dbContext.Tbl_SOREGIONS.Where(u => u.ID == ID).FirstOrDefault();
                    dbContext.Tbl_SOREGIONS.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static List<Area> GetAreaListByJOBCityID(int SOID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<Area> areaData = new List<Area>();
                    var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();
                    var SO = dbContext.SaleOfficers.Where(s => s.ID == SOID).FirstOrDefault();

                    var AreasIDs = dbContext.Jobs.Where(j => j.SaleOfficerID == SO.ID && j.CityID == CityID && j.IsDeleted == false).Select(j => j.Areas).ToList();

                    if (AreasIDs.Count() != 0)
                    {
                        var query = from val in FOS.Shared.Common.AppUtility.ListToCommaValues(AreasIDs).Split(',')
                                    select int.Parse(val);

                        areaData = dbContext.Areas.Where(a => a.CityID == CityID && !query.Contains(a.ID)).ToList();
                    }
                    else
                    {
                        areaData = dbContext.Areas.Where(a => a.CityID == CityID).ToList();
                    }
                    

                    return areaData.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get All Areas List By CityID ...
        public static List<AreaData> GetAreaListByCityID(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<AreaData> ChooseAreas = dbContext.Areas.AsEnumerable().Where(a => a.CityID == CityID).Select(a => new AreaData 
                    {
                        ID = a.ID,
                        Name = a.Name,
                        IsActive = a.IsActive
                    }).ToList();

                    ChooseAreas.Insert(0, new AreaData
                    {
                        ID = 0,
                        Name = "--Select Town--"
                    });

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




        public static List<AreaData> GetSubDivisionListByArea(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<AreaData> ChooseAreas = dbContext.SubDivisions.AsEnumerable().Where(a => a.AreaID == CityID).Select(a => new AreaData
                    {
                        ID = a.ID,
                        Name = a.Name,
                  
                    }).ToList();

                    ChooseAreas.Insert(0, new AreaData
                    {
                        ID = 0,
                        Name = "--Select Subdivision--"
                    });

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SubCategories> GetSubCatForAPI(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<SubCategories> ChooseAreas = dbContext.SubCategories.Where(a => a.MainCategID == CityID).Select(a => new SubCategories
                    {
                        ID = a.SubCategID,
                        SubName = a.SubCategDesc,
                        IsActive=a.IsActive
                       
                    }).ToList();

                

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetCitiesForAPI(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Cities.Where(a => a.RegionID == CityID).Select(a => new CityData
                    {
                        ID = a.ID,
                        Name = a.Name,
                        IsActive = a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetTownsForAPI(int CityID,int SOID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<CityData> cityList = new List<CityData>();
                    CityData cty;

                    var list = dbContext.SOZoneAndTowns.Where(x => x.SOID == SOID).ToList();
                    var list2 = list.GroupBy(x => x.CityID).Select(x => x.Key);

                    foreach (var data in list2)
                    {
                        cty = new CityData();

                        cty.ID = (int)data;
                        cty.Name = dbContext.Cities.Where(x => x.ID == data).Select(x => x.Name).FirstOrDefault();
                        cty.ClientId=(int)dbContext.Cities.Where(x => x.ID == data).Select(x => x.ZoneId).FirstOrDefault();


                        cityList.Add(cty);
                    }
                    cityList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });

                    //if (CityID == 9)
                    //{

                    //        cityList = dbContext.Cities.Where(c => c.IsDeleted == false).OrderBy(c => c.Name)
                    //                    .Select(
                    //                        u => new CityData
                    //                        {
                    //                            ID = u.ID,
                    //                            Name = u.Name,

                    //                        }).ToList();


                    //    cityList.Insert(0, new CityData
                    //    {
                    //        ID = 0,
                    //        Name = "Select"
                    //    });
                    //}
                    //else
                    //{

                    //        cityList = dbContext.Cities.Where(c => c.IsDeleted == false && c.ZoneId == CityID).OrderBy(c => c.Name)
                    //                    .Select(
                    //                        u => new CityData
                    //                        {
                    //                            ID = u.ID,
                    //                            Name = u.Name,

                    //                        }).ToList();


                    //    cityList.Insert(0, new CityData
                    //    {
                    //        ID = 0,
                    //        Name = "Select"
                    //    });
                    //}
                    return cityList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int AddUpdateSORegions(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_SOREGIONS AreaObj = new Tbl_SOREGIONS();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Tbl_SOREGIONS.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.SaleofficerID = obj.SOID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RegionalHeadID = dbContext.SaleOfficers.Where(x=>x.ID==obj.SOID).Select(x=>x.RegionalHeadID).FirstOrDefault();
                          
                            AreaObj.Status = true;
                          
                         
                          

                            dbContext.Tbl_SOREGIONS.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Tbl_SOREGIONS.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.SaleofficerID = obj.SOID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RegionalHeadID = dbContext.SaleOfficers.Where(x => x.ID == obj.SOID).Select(x => x.RegionalHeadID).FirstOrDefault();
                            AreaObj.Status = true;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static List<SubCategoryA> GetSubCatAForAPI(int MainCat, int SubCat)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {


                    List<SubCategoryA> Items = dbContext.SubCtegoryAs.Where(a => a.MainCategID == MainCat && a.SubCategID == SubCat).Select(a => new SubCategoryA
                    {
                        SubCategoryAID = a.SubCategIDA,
                        SubCategoryAName = a.SubCategADesc,
                        IsActive=a.IsActive

                    }).ToList();



                    return Items.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public static List<Items> GetItemsForAPI(int MainCat)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                  

                    List<Items> Items = dbContext.Items.Where(a => a.MainCategID == MainCat).Select(a => new Items
                    {
                        ItemId = a.ItemID,
                        ItemName = a.ItemName,
                        ItemPrice=a.Price,
                        ItemPacking=a.Packing,
                        IsActive = a.IsActive,
                        SortOrder=a.SortOrder
                       
                        

                    }).ToList();



                    return Items.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<AreaData> GetAreaListByCityIDForDealers(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<AreaData> ChooseAreas = dbContext.Areas.Where(a => a.CityID == CityID
                    && a.IsActive && !a.IsDeleted).Select(a => new AreaData
                    {
                        ID = a.ID,
                        Name = a.Name,
                        CityName = a.City.Name,
                        CityID = a.CityID
                    }).ToList();

                  

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Get All Areas By CityID In Edit Mode ...
        public static List<Area> GetAreaListByCityIDJobEdit(int JobID, int SOID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<Area> areaData = new List<Area>();

                    var SO = dbContext.SaleOfficers.Where(s => s.ID == SOID).FirstOrDefault();

                    var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    var ChooseAreas = dbContext.Cities.Where(a => a.ID == CityID).SelectMany(a => a.Areas);

                    var AreasIDs = dbContext.Jobs.Where(j => j.ID == JobID && j.SaleOfficerID == SOID && j.CityID == CityID && j.IsDeleted == false).Select(j => j.Areas).FirstOrDefault();

                    if (AreasIDs != null)
                    {
                        var query = from val in AreasIDs.Split(',')
                                    select int.Parse(val);

                        areaData = dbContext.Areas.Where(a => a.CityID == CityID && query.Contains(a.ID)).ToList();
                    }
                    else
                    {
                        areaData = dbContext.Areas.Where(a => a.CityID == CityID).ToList();
                    }


                    return areaData.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<AreaData> GetAreasForAPI( int CityID, int SOID)
        {
            List<AreaData> cityList = new List<AreaData>();
            AreaData cty;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    var list = dbContext.Areas.Where(x => x.CityID==CityID).Select(x=>x.ID).ToList();
                    

                    foreach (var data in list)
                    {
                        cty = new AreaData();

                        cty.ID = (int)data;
                        cty.Name = dbContext.Areas.Where(x => x.ID == data).Select(x => x.Name).FirstOrDefault();

                        cityList.Add(cty);

                    }


                    //cityList = dbContext.Areas.Where(a =>  a.CityID == CityID).Select(a => new AreaData
                    //{
                    //    ID = a.ID,
                    //    Name = a.Name,
                        

                    //}).ToList();

                    cityList.Insert(0, new AreaData
                    {
                        ID = 0,
                        Name = "Select"
                    });

                    return cityList.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<CityData> GetSubDivisionForAPI(int RegionID, int CityID)
        {
            try
            {
                List<CityData> cityList = null;
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    cityList = dbContext.SubDivisions.Where(a => a.CityID == RegionID && a.AreaID == CityID).Select(a => new CityData
                    {
                        ID = a.ID,
                        Name = a.DisplayFieldinArea,
                        

                    }).ToList();
                    cityList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });


                    return cityList.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetSMSForAPI(int RegionID)
        {
            try
            {
              

                List<CityData> cityList = null;
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    var SiteID = dbContext.Jobs.Where(x => x.ID == RegionID).Select(x => x.SiteID).FirstOrDefault();
                    var SiteName = dbContext.Retailers.Where(x => x.ID == SiteID).FirstOrDefault();
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    cityList = dbContext.ChatBoxes.Where(a => a.ComplaintID == RegionID).Select(a => new CityData
                    {
                        ID = a.ID,
                        Remarks = a.Remarks,
                        ComplaintID = a.ComplaintID,
                       LaunchAt=a.DateInstall,
                       SOID=a.SOID,
                       SaleOfficerName=dbContext.SaleOfficers.Where(x=>x.ID==a.SOID).Select(x=>x.Name).FirstOrDefault()+ "(" + dbContext.SaleOfficers.Where(x => x.ID == a.SOID).Select(x => x.UserName).FirstOrDefault() + ")",
                       SiteName= SiteName.Name+"("+SiteName.RetailerCode + ")",
                       
                    }).ToList();
                   


                    return cityList.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetRetailersForAPI(int RegionID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Retailers.Where(a => a.RegionID == RegionID && a.CityID==CityID).Select(a => new CityData
                    {
                        ID = a.ID,
                        ShopName = a.ShopName,
                        IsActive = a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetRetailersForAPIFinal(int RegionID, int CityID, int saleofficerid)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Retailers.Where(a => a.RegionID == RegionID && a.CityID == CityID && a.SaleOfficerID ==saleofficerid).Select(a => new CityData
                    {
                        ID = a.ID,
                        ShopName = a.ShopName,
                        IsActive = a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetSitesForAPIFinal(int ProjectID,int RegionID, int CityID, int SubDiv)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Retailers.Where(a => a.CityID == RegionID && a.ZoneID==ProjectID  && a.AreaID == CityID && a.SubDivisionID == SubDiv).Select(a => new CityData
                    {
                        ID = a.ID,
                        Name = a.Name + " " + a.RetailerCode,
                        ShortCode=a.RetailerCode,
                        

                    }).ToList();
                    ChooseAreas.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });


                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetSitesForAPIFinals(int ProjectID, int RegionID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Retailers.Where(a => a.CityID == RegionID && a.ZoneID == ProjectID && a.AreaID == CityID ).Select(a => new CityData
                    {
                        ID = a.ID,
                        Name = a.Name + " " + a.RetailerCode,
                        ShortCode = a.RetailerCode,


                    }).ToList();
                    ChooseAreas.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });


                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetComplaintsForAPIFinals(int ProjectID, int RegionID, int CityID)
        {
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);
            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            try
            {
                var city = Convert.ToString(CityID);
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                   // var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Jobs.Where(a => a.CityID == RegionID && a.ZoneID == ProjectID && a.Areas == city && a.CreatedDate >= dtFromToday && a.CreatedDate <= dtToToday).Select(a => new CityData
                    {
                        ID = a.ID,
                        Name = a.TicketNo + " " + dbContext.Retailers.Where(x=>x.ID==a.SiteID).Select(x=>x.Name).FirstOrDefault(),
                        ShortCode = a.VisitType,


                    }).ToList();
                  


                    return ChooseAreas.ToList();

                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }







        public static List<CityData> GetDistributorForAPI(int RegionID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Dealers.Where(a => a.RegionID == RegionID && a.CityID == CityID).Select(a => new CityData
                    {
                        ID = a.ID,
                        ShopName = a.ShopName,
                        IsActive = a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static void DeleteRetailerAPI(int RetailerID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    var retailers = dbContext.Retailers.Where(a => a.ID == RetailerID).FirstOrDefault();
                    retailers.IsDeleted = true;
                    dbContext.SaveChanges();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }










        //public static List<AreaData> GetRetailerForAPI(int ID)
        //{
        //    try
        //    {
        //        using (FOSDataModel dbContext = new FOSDataModel())
        //        {


        //            List<AreaData> Items = dbContext.Areas.Where(a => a.RegionID == RegionID && a.CityID == CityID).Select(a => new AreaData
        //            {
        //                ID = a.ID,
        //                Name = a.Name,
        //                IsActive = a.IsActive

        //            }).ToList();



        //            return Items.ToList();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}








        // Get All Areas By CityID In Edit Mode ...
        public static List<AreaData> GetAreaListByCityIDEdit(int intSaleOfficerID, int intCityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var query = from saleoff in dbContext.SaleOfficers
                                where saleoff.Areas.Any(s => s.CityID == intCityID)
                                select saleoff.Areas;

                    var qArea = dbContext.SaleOfficers.Where(p => p.CityID == intCityID).SelectMany(p => p.Areas);

                    var qAreaEdit = dbContext.SaleOfficers.Where(p => p.ID == intSaleOfficerID).SelectMany(p => p.Areas);

                    var area = (from areas in dbContext.Areas
                                where areas.CityID == intCityID &&
                                !
                                    ((from saleoffarea in qArea
                                      select new
                                      {
                                          saleoffarea.ID
                                      }).Distinct()).Contains(new { ID = areas.ID })
                                orderby areas.Name
                                select new AreaData()
                                {
                                    ID = areas.ID,
                                    Name = areas.Name,
                                }).Union
                                (
                                     from saleoffarea in qAreaEdit
                                     orderby saleoffarea.Name
                                     select new AreaData()
                                     {
                                         ID = saleoffarea.ID,
                                         Name = saleoffarea.Name,
                                     }
                                );

                    return area.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Insert OR Update Area ...
        public static int AddUpdateArea(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Area AreaObj = new Area();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Areas.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = (int)obj.CityID;
                            AreaObj.ShortCode = obj.ShortCode;
                            AreaObj.IsActive = true;
                            AreaObj.CreatedDate = DateTime.Now;
                            AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.CreatedBy = 1;

                            dbContext.Areas.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Areas.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = (int)obj.CityID;
                            AreaObj.ShortCode = obj.ShortCode;
                            AreaObj.LastUpdate = DateTime.Now;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        // Delete Area...
        public static int DeleteArea(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Area obj = dbContext.Areas.Where(u => u.ID == AreaID).FirstOrDefault();
                    dbContext.Areas.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static int DeleteSubDivision(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SubDivision obj = dbContext.SubDivisions.Where(u => u.ID == AreaID).FirstOrDefault();
                    dbContext.SubDivisions.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }

        // Get All Areas List For Grid ...
        public static List<AreaData> GetAreaForGrid(int intCityID)
        {
            List<AreaData> areaData = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Areas.Where(u => u.CityID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new AreaData
                                {
                                    ID = u.ID,
                                    RegionID = u.RegionID,
                                    CityID = u.CityID,
                                    Name = u.Name,
                                    RegionName = "",
                                    CityName = u.City.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }



        public static List<AreaData> GetSubDivisionForGrid(int intCityID)
        {
            List<AreaData> areaData = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.SubDivisions.Where(u => u.CityID == intCityID)
                            .ToList().Select(
                                u => new AreaData
                                {
                                    ID = u.ID,
                                    AreaID=u.AreaID,
                                    CityID = u.CityID,
                                    Name = u.Name,
                                    RegionName = "",
                                  ShortCode=u.Code,
                                    AreaName= u.Area.Name
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static List<Tbl_AccessModel> GetAccessForGrid(int intCityID)
        {
            List<Tbl_AccessModel> areaData = new List<Tbl_AccessModel>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Tbl_Access.Where(u => u.RegionID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new Tbl_AccessModel
                                {
                                  ID = u.ID,
                                  RegionID = u.RegionID,
                                  RegionName= u.Region.Name,
                                  SaleOfficerID=u.SaleOfficerID,
                                  RepotedTo=u.RepotedUP,
                                  ReportedToName=dbContext.SaleOfficers.Where(x=>x.ID==u.RepotedUP).Select(x=>x.Name).FirstOrDefault(),
                                  SaleOfficerName=u.SaleOfficer.Name,
                                 ReportedFor = u.ReportedDown,
                                 ReportedForName = dbContext.SaleOfficers.Where(x => x.ID == u.ReportedDown).Select(x => x.Name).FirstOrDefault()

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static List<SubCategories> GetSubCatForGrid(int intCityID)
        {
            List<SubCategories> areaData = new List<SubCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.SubCategories.Where(u => u.MainCategID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new SubCategories
                                {
                                    ID = u.SubCategID,
                                    SubName = u.SubCategDesc,

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }


        public static List<Tbl_AccessModel> GetSORegionsForGrid(int intCityID)
        {
            List<Tbl_AccessModel> areaData = new List<Tbl_AccessModel>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Tbl_SOREGIONS.Where(u => u.RegionID == intCityID && u.Status == true)
                            .ToList().Select(
                                u => new Tbl_AccessModel
                                {
                                    ID = u.ID,
                                    RegionID = u.RegionID,
                                    RegionName = dbContext.Regions.Where(x=>x.ID==u.RegionID).Select(x=>x.Name).FirstOrDefault(),
                                    SaleOfficerID = u.SaleofficerID,
                                    SaleOfficerName= dbContext.SaleOfficers.Where(x => x.ID == u.SaleofficerID).Select(x => x.Name).FirstOrDefault(),
                                    RHID=u.RegionalHeadID,
                                    RegionalHeadName = dbContext.RegionalHeads.Where(x => x.ID == u.RegionalHeadID).Select(x => x.Name).FirstOrDefault(),


                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static int AddUpdateSubCAt(SubCategories obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        SubCategory AreaObj = new SubCategory();

                        if (obj.ID == 0)
                        {
                            AreaObj.SubCategID = dbContext.SubCategories.OrderByDescending(u => u.SubCategID).Select(u => u.SubCategID).FirstOrDefault() + 1;
                            AreaObj.MainCategID = obj.MainCategoryID;
                            AreaObj.SubCategDesc = obj.SubName;
                            AreaObj.IsActive = true ;
                            AreaObj.IsDeleted = false;
                            AreaObj.IsActive = true;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;

                            dbContext.SubCategories.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.SubCategories.Where(u => u.SubCategID == obj.ID).FirstOrDefault();
                            AreaObj.SubCategDesc = obj.SubName;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                            AreaObj.IsActive = true;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        public static int DeleteSubCat(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SubCategory obj = dbContext.SubCategories.Where(u => u.SubCategID == AreaID).FirstOrDefault();
                    dbContext.SubCategories.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete SubCategory Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static List<AreaData> GetResult(string search, string sortOrder, int start, int length, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<AreaData> FilterResult(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            IQueryable<AreaData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[6].ToLower())))
                );

            return results;
        }


        public static List<SubCategories> GetResult1(string search, string sortOrder, int start, int length, List<SubCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count1(string search, List<SubCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<SubCategories> FilterResult1(string search, List<SubCategories> dtResult, List<string> columnFilters)
        {
            IQueryable<SubCategories> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SubName != null && p.SubName.ToLower().Contains(search.ToLower()) )
       
                ));

            return results;
        }
        public static List<Items> GetResult2(string search, string sortOrder, int start, int length, List<Items> dtResult, List<string> columnFilters)
        {
            return FilterResult2(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count2(string search, List<Items> dtResult, List<string> columnFilters)
        {
            return FilterResult2(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<Items> FilterResult2(string search, List<Items> dtResult, List<string> columnFilters)
        {
            IQueryable<Items> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.ItemName != null && p.ItemName.ToLower().Contains(search.ToLower()) )
                
                ));

            return results;
        }


        public static List<SubCategoryA> GetResult3(string search, string sortOrder, int start, int length, List<SubCategoryA> dtResult, List<string> columnFilters)
        {
            return FilterResult3(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count3(string search, List<SubCategoryA> dtResult, List<string> columnFilters)
        {
            return FilterResult3(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<SubCategoryA> FilterResult3(string search, List<SubCategoryA> dtResult, List<string> columnFilters)
        {
            IQueryable<SubCategoryA> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SubCategoryAName != null && p.SubCategoryAName.ToLower().Contains(search.ToLower()))

                ));

            return results;
        }




        public static List<Items> GetItemsForGrid(int intCityID)
        {
            List<Items> areaData = new List<Items>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Items.Where(u => u.MainCategID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new Items
                                {
                                    ItemId = u.ItemID,
                                    ItemName = u.ItemName,
                                    ItemPrice = u.Price,
                                    ItemCode=u.ItemCode,
                                    ItemPacking=u.Packing,
                                    SortOrder=u.SortOrder
                                 
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }



        public static int AddUpdateItems(Items obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Item AreaObj = new Item();

                        if (obj.ItemId == 0)
                        {
                            AreaObj.ItemID = dbContext.Items.OrderByDescending(u => u.ItemID).Select(u => u.ItemID).FirstOrDefault() + 1;
                            AreaObj.ItemName = obj.ItemName;
                            AreaObj.ItemDesc = null;
                            AreaObj.ItemCode = obj.ItemCode;
                            AreaObj.Price = obj.ItemPrice;
                            AreaObj.Packing = obj.ItemPacking;
                            AreaObj.MainCategID = obj.MainCategoryID;
                            AreaObj.SubCategID = 1;
                            AreaObj.SubCategIDA = 1;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.SortOrder = obj.SortOrder;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;

                            dbContext.Items.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Items.Where(u => u.ItemID == obj.ItemId).FirstOrDefault();
                            AreaObj.ItemName = obj.ItemName;
                            AreaObj.ItemDesc = null;
                            AreaObj.ItemCode = obj.ItemCode;
                            AreaObj.Price = obj.ItemPrice;
                            AreaObj.Packing = obj.ItemPacking;
                            AreaObj.MainCategID = obj.MainCategoryID;
                            AreaObj.SubCategID = 1;
                            AreaObj.SubCategIDA = 1;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                            AreaObj.SortOrder = obj.SortOrder;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Item Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static int DeleteItem(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Item obj = dbContext.Items.Where(u => u.ItemID == AreaID).FirstOrDefault();
                    dbContext.Items.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Items Failed");
                Resp = 1;
            }
            return Resp;
        }




        public static int AddUpdateSubCatA(SubCategoryA obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        SubCtegoryA AreaObj = new SubCtegoryA();

                        if (obj.SubCategoryAID == 0)
                        {
                            AreaObj.SubCategIDA = dbContext.SubCtegoryAs.OrderByDescending(u => u.SubCategIDA).Select(u => u.SubCategIDA).FirstOrDefault() + 1;
                            AreaObj.SubCategADesc = obj.SubCategoryAName;
                          
                            AreaObj.MainCategID = obj.MainCategoryID;
                            AreaObj.SubCategID = obj.ID;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;

                            dbContext.SubCtegoryAs.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.SubCtegoryAs.Where(u => u.SubCategIDA == obj.SubCategoryAID).FirstOrDefault();
                            AreaObj.SubCategADesc = obj.SubCategoryAName;
                        
                            AreaObj.MainCategID = obj.MainCategoryID;
                            AreaObj.SubCategID = obj.ID;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Item Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        public static int AddUpdateSubDivision(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        SubDivision AreaObj = new SubDivision();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Areas.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.Name = obj.Name;
                            AreaObj.AreaID = obj.AreaID;
                            AreaObj.CityID = obj.CityID;
                            AreaObj.Code = obj.ShortCode;
                          

                            dbContext.SubDivisions.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.SubDivisions.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.Name = obj.Name;
                            AreaObj.AreaID = obj.AreaID;
                            AreaObj.CityID = obj.CityID;
                            AreaObj.Code = obj.ShortCode;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static List<SubCategoryA> GetSubCatAForGrid(int intCityID, int SubCat)
        {
            List<SubCategoryA> areaData = new List<SubCategoryA>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.SubCtegoryAs.Where(u => u.MainCategID == intCityID && u.SubCategID == SubCat && u.IsDeleted == false)
                            .ToList().Select(
                                u => new SubCategoryA
                                {
                                    SubCategoryAID = u.SubCategIDA,
                                    SubCategoryAName = u.SubCategADesc,
                                    MainCatName=u.MainCategory.MainCategDesc,
                                    SubCatName=u.SubCategory.SubCategDesc

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static int DeleteSubCatA(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SubCtegoryA obj = dbContext.SubCtegoryAs.Where(u => u.SubCategIDA == AreaID).FirstOrDefault();
                    dbContext.SubCtegoryAs.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Items Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static List<Tbl_AccessModel> GetResult7(string search, string sortOrder, int start, int length, List<Tbl_AccessModel> dtResult, List<string> columnFilters)
        {
            return FilterResult7(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count7(string search, List<Tbl_AccessModel> dtResult, List<string> columnFilters)
        {
            return FilterResult7(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<Tbl_AccessModel> FilterResult7(string search, List<Tbl_AccessModel> dtResult, List<string> columnFilters)
        {
            IQueryable<Tbl_AccessModel> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower()))
                //&& (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[5].ToLower())))
                //&& (columnFilters[6] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[6].ToLower())))
                ));

            return results;
        }

        public static int AddUpdateAccess(Tbl_AccessModel obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_Access AreaObj = new Tbl_Access();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Tbl_Access.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.SaleOfficerID = obj.SaleOfficerID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RepotedUP = obj.RepotedTo;
                            AreaObj.ReportedDown = obj.ReportedFor;
                            AreaObj.Status = true;
                            AreaObj.CreatedOn = DateTime.Now;
                          //  AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.IsDeleted = false;

                            dbContext.Tbl_Access.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Tbl_Access.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.SaleOfficerID = obj.SaleOfficerID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RepotedUP = obj.RepotedTo;
                            AreaObj.ReportedDown = obj.ReportedFor;
                            AreaObj.Status = true;
                            AreaObj.CreatedOn = DateTime.Now;
                       
                            AreaObj.IsDeleted = false;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

    }
}