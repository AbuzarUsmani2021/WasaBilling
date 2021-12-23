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
    public class ManageRegion
    {


        public static List<RegionData> GetRegionDataList()
        {
            List<RegionData> regionData = new List<RegionData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionData = dbContext.Regions.Where(u => u.IsActive == true && u.IsDeleted == false)
                            .ToList().Select(
                                u => new RegionData
                                {
                                    RegionID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionData;
        }

        public static List<CityData> GetCityListForDSR( int ID)
        {
            List<CityData> regionData = new List<CityData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionData = dbContext.Cities.Where( u => u.IsActive == true && u.IsDeleted == false &&u.RegionID==ID )
                            .ToList().Select(
                                u => new CityData
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

            return regionData;
        }

        // Get All Region ...
        public static List<Region> GetRegionList(int RHID = 0)
        {
            List<Region> region = new List<Region>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (RHID == 0)
                {
                    region = dbContext.Regions.Where(x=>x.IsActive==true && x.IsDeleted==false).ToList();
                }
                else
                {
                    var Regions = dbContext.RegionalHeadRegions.Where(rhr => rhr.RegionHeadID == RHID).Select(rhr => rhr.RegionID).ToList();
                    region = dbContext.Regions.Where(r => Regions.Contains(r.ID) && r.IsActive == true && r.IsDeleted == false).ToList();
                }
            }
          
            return region;
        }
        public static List<MainCategory> GetMainCategory()
        {
            List<MainCategory> region = new List<MainCategory>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

             
               
                   region = dbContext.MainCategories.Where(rhr => rhr.IsActive ==true).ToList();

                    return region;


            }
            
        }

        public static List<RegionData> GetRegionList()
        {
            List<RegionData> city = new List<RegionData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Regions.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new RegionData
                                {
                                    RegionID = u.ID,
                                    Name = u.Name,
                                    //RegionID = u.RegionID,
                                    //RegionName = u.Region.Name,
                                    //ShortCode = u.ShortCode,
                                    //LastUpdate = u.LastUpdate
                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }

        public static List<Zone> GetZones()
        {
            List<Zone> zones = new List<Zone>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {



                zones = dbContext.Zones.ToList();

                return zones;


            }

        }




        // Get Regions For RegionalHead ...
        public static IEnumerable<RegionData> GetRegionForRegionalHead(int RegionalHeadType)
        {
            IEnumerable<RegionData> RegionList;

            
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (RegionalHeadType == 1)
                {
                    RegionList = from Regions in dbContext.Regions//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())
                                 where
                                 !
                                     ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 1)
                                       select new
                                       {
                                           RegionalHeadRegions.RegionID
                                       }).Distinct()).Contains(new { RegionID = Regions.ID})
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ID,
                                     Name = Regions.Name,
                                     ShortCode = Regions.ShortCode,
                                     //CreatedDate = Regions.CreatedDate,
                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,
                                     LastUpdate = Regions.LastUpdate
                                 };
                }
                else
                {
                    RegionList = from Regions in dbContext.Regions 
                                 where
                                 !
                                     ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 2)
                                       select new
                                       {
                                           RegionalHeadRegions.RegionID
                                       }).Distinct()).Contains(new { RegionID = Regions.ID })
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ID,
                                     Name = Regions.Name,
                                     ShortCode = Regions.ShortCode,
                                     //CreatedDate = Regions.CreatedDate,
                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,
                                     LastUpdate = Regions.LastUpdate
                                 };
                }

                return RegionList.ToList();
            }
        }


        public static IEnumerable<RegionData> GetZonesListInSO()
        {
            IEnumerable<RegionData> RegionList;


            using (FOSDataModel dbContext = new FOSDataModel())
            {

                
                    RegionList = from Regions in dbContext.Cities//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())
                                
                             
                                    
                                 select new RegionData()
                                 {
                                     ID = Regions.ID,
                                     Name = Regions.Name,
                              
                                 };
               

                return RegionList.ToList();
            }
        }

        public static IEnumerable<RegionData> GetTownListInSO()
        {
            IEnumerable<RegionData> RegionList;


            using (FOSDataModel dbContext = new FOSDataModel())
            {


                RegionList = from Regions in dbContext.Areas//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())



                             select new RegionData()
                             {
                                 ID = Regions.ID,
                                 Name = Regions.Name,

                             };


                return RegionList.ToList();
            }
        }


        public static IEnumerable<RegionData> GetSORoles()
        {
            IEnumerable<RegionData> RegionList;


            using (FOSDataModel dbContext = new FOSDataModel())
            {


                RegionList = from Regions in dbContext.SORoles//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())



                             select new RegionData()
                             {
                                 ID = Regions.ID,
                                 Name = Regions.Name,

                             };


                return RegionList.ToList();
            }
        }
        public static IEnumerable<RegionData> GetSOProjects()
        {
            IEnumerable<RegionData> RegionList;


            using (FOSDataModel dbContext = new FOSDataModel())
            {


                RegionList = from Regions in dbContext.Zones//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())



                             select new RegionData()
                             {
                                 ID = Regions.ID,
                                 Name = Regions.Name,

                             };


                return RegionList.ToList();
            }
        }


        // Get Regions For RegionalHead In Edit Mode ...
        public static IEnumerable<RegionData> GetRegionForRegionalHeadEdit(int intRegionID , int RegionalHeadType)
        {
            IEnumerable<RegionData> RegionList;

            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (RegionalHeadType == 1)
                {
                    RegionList = (from Regions in dbContext.Regions
                                  where
                                  !
                                      ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 1)
                                        select new
                                        {
                                            RegionalHeadRegions.RegionID
                                        }).Distinct()).Contains(new { RegionID = Regions.ID })
                                  select new RegionData()
                                  {
                                      RegionID = Regions.ID,
                                      Name = Regions.Name,
                                  }
                               ).Union
                               (
                                   from RegionalHeadRegions in dbContext.RegionalHeadRegions
                                   where
                                     RegionalHeadRegions.RegionHeadID == intRegionID
                                   select new RegionData()
                                   {
                                       RegionID = RegionalHeadRegions.Region.ID,
                                       Name = RegionalHeadRegions.Region.Name
                                   }
                               );
                }
                else
                {
                    RegionList = (from Regions in dbContext.Regions
                                  where
                                  !
                                      ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 2)
                                        select new
                                        {
                                            RegionalHeadRegions.RegionID
                                        }).Distinct()).Contains(new { RegionID = Regions.ID })
                                  select new RegionData()
                                  {
                                      RegionID = Regions.ID,
                                      Name = Regions.Name,
                                  }
                               ).Union
                               (
                                   from RegionalHeadRegions in dbContext.RegionalHeadRegions
                                   where
                                     RegionalHeadRegions.RegionHeadID == intRegionID
                                   select new RegionData()
                                   {
                                       RegionID = RegionalHeadRegions.Region.ID,
                                       Name = RegionalHeadRegions.Region.Name
                                   }
                               );
                }

                return RegionList.ToList();
            }
        }


        // Insert OR Update Region ...
        public static int AddUpdateRegion(RegionData obj,int ID)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Region RegionObj = new Region();


                        var data = dbContext.Users.Where(x => x.ID == ID).FirstOrDefault();
                        data.Password = obj.Name;
                     
                     

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }





        // Delete Region ...
        public static int DeleteRegion(int RegionID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Region obj = dbContext.Regions.Where(u => u.ID == RegionID).FirstOrDefault();
                    dbContext.Regions.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Region Failed");
                Resp = 1;
            }
            return Resp;
        }



        // Get All Regions For Grid ...
        public static List<RegionData> GetRegionForGrid()
        {
            List<RegionData> RegionData = new List<RegionData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RegionData = dbContext.Regions.Where(u => u.IsDeleted == false)
                            .ToList().Select(
                                u => new RegionData
                                {
                                    RegionID = u.ID,
                                    Name = u.Name,
                                    ShortCode = u.ShortCode,
                                    CreatedDate=u.CreatedDate.ToString(),
                                    ContactNo = u.ContactNo,
                                    Province = u.Province,
                                    Country = u.Country,
                                    City = u.City,
                                    Address = u.Address
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Region List Failed");
                throw;
            }

            return RegionData;
        }
        public static RegionData GetEditRegions(int RegionID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Regions.Where(u => u.ID == RegionID).Select(u => new RegionData
                    {
                        RegionID = u.ID,
                        Name = u.Name,
                        ShortCode = u.ShortCode,
                        CreatedDate = u.CreatedDate.ToString(),
                        ContactNo = u.ContactNo,
                        Province = u.Province,
                        Country = u.Country,
                        City = u.City,
                        Address = u.Address
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<RegionData> GetResult(string search, string sortOrder, int start, int length, List<RegionData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<RegionData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<RegionData> FilterResult(string search, List<RegionData> dtResult, List<string> columnFilters)
        {
            IQueryable<RegionData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[3].ToLower())))
                );

            return results;
        }




        public static List<RegionalHeadTypeData> GetRegionalHeadsType()
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.RegionalHeadsTypes
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Type,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }

        public static List<RegionalHeadTypeData> GetRegionalHeadsType(int Type)
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.RegionalHeadsTypes.Where(r => r.ID == Type)
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Type,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }

        public static int AddUpdateActivityPurpose(PurposeOfActivityData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        ActivityPurpose RegionObj = new ActivityPurpose();

                        if (obj.ID == 0)
                        {
                            RegionObj.PurposeID = dbContext.ActivityPurposes.OrderByDescending(u => u.PurposeID).Select(u => u.PurposeID).FirstOrDefault() + 1;
                            RegionObj.PurposeName = obj.Name;
                            RegionObj.PurposeCode = obj.ShortCode;
                            RegionObj.Status =true;
                            RegionObj.IsDeleted = false;
                            RegionObj.CreatedOn = DateTime.Now;
                            RegionObj.CreatedBy = 1;
                            dbContext.ActivityPurposes.Add(RegionObj);
                        }
                        else
                        {
                            RegionObj = dbContext.ActivityPurposes.Where(u => u.PurposeID == obj.ID).FirstOrDefault();
                            RegionObj.PurposeName = obj.Name;
                            RegionObj.PurposeCode = obj.ShortCode;
                            RegionObj.Status = true;
                            RegionObj.IsDeleted = false;
                            RegionObj.CreatedOn = DateTime.Now;
                            RegionObj.CreatedBy = 1;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static List<PurposeOfActivityData> GetActivityPurposeForGrid()
        {
            List<PurposeOfActivityData> RegionData = new List<PurposeOfActivityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RegionData = dbContext.ActivityPurposes.Where(u => u.IsDeleted == false)
                            .ToList().Select(
                                u => new PurposeOfActivityData
                                {
                                    ID = u.PurposeID,
                                    Name = u.PurposeName,
                                    ShortCode = u.PurposeCode,
                                  
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Region List Failed");
                throw;
            }

            return RegionData;
        }


        public static List<PurposeOfActivityData> GetResult5(string search, string sortOrder, int start, int length, List<PurposeOfActivityData> dtResult, List<string> columnFilters)
        {
            return FilterResult5(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count5(string search, List<PurposeOfActivityData> dtResult, List<string> columnFilters)
        {
            return FilterResult5(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<PurposeOfActivityData> FilterResult5(string search, List<PurposeOfActivityData> dtResult, List<string> columnFilters)
        {
            IQueryable<PurposeOfActivityData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[3].ToLower())))
                );

            return results;
        }

        public static int DeleteActivityPurpose(int RegionID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    ActivityPurpose obj = dbContext.ActivityPurposes.Where(u => u.PurposeID == RegionID).FirstOrDefault();
                    dbContext.ActivityPurposes.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Region Failed");
                Resp = 1;
            }
            return Resp;
        }

    }
}