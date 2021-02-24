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
    public class ManageCity
    {

        //Get data Of Complaint Registration Start
        public static List<ComplaintStatus> GetProjectsListForComplaintRegistration(List<int?> list)
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
                        comlist.Name = dbContext.Zones.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                        city.Add(comlist);
                    }

                }
                city.Insert(0, new ComplaintStatus
                {
                    ID = 0,
                    Name = "--Select Project--"
                });

            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }
        public static List<CityData> GetCityListForComplaintRegistration(List<int?> list)
        {
            List<CityData> city = new List<CityData>();
            CityData comlist;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    foreach (var item in list)
                    {
                        comlist = new CityData();
                        comlist.ID = (int)item;
                        comlist.Name = dbContext.Cities.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                        city.Add(comlist);
                    }

                }
                city.Insert(0, new CityData
                {
                    ID = 0,
                    Name = "--Select Zone--"
                });

            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }
        public static List<AreaData> GetAreaListForComplaintRegistration(List<int?> list)
        {
            List<AreaData> city = new List<AreaData>();
            AreaData comlist;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    foreach (var item in list)
                    {
                        comlist = new AreaData();
                        comlist.ID = (int)item;
                        comlist.Name = dbContext.Areas.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                        city.Add(comlist);
                    }

                }
                city.Insert(0, new AreaData
                {
                    ID = 0,
                    Name = "--Select Town--"
                });

            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }
        public static List<RetailerData> GetSitesList()
        {
            List<RetailerData> Sites = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Sites = dbContext.Retailers.Where(c => c.IsDeleted == false && c.IsActive == true)
                            .Select
                            (
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    RetailerCode = u.RetailerCode

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            Sites.Insert(0, new RetailerData
            {
                ID = 0,
                Name = "--Select Sites--"
            });

            return Sites;
        }
        public static List<FaultTypeData> GetFaultTypesList()
        {
            List<FaultTypeData> city = new List<FaultTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.FaultTypes
                            .Select
                            (
                                u => new FaultTypeData
                                {
                                    Id = u.Id,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            city.Insert(0, new FaultTypeData
            {
                Id = 0,
                Name = "--Select Fault Type--"
            });


            return city;
        }
        public static List<FaultTypeData> GetFaultTypesDetailList()
        {
            List<FaultTypeData> city = new List<FaultTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.FaultTypeDetails
                            .Select
                            (
                                u => new FaultTypeData
                                {
                                    Id = u.ID,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            city.Insert(0, new FaultTypeData
            {
                Id = 0,
                Name = "--Select Fault Type Detail--"
            });

            return city;
        }
        public static List<PriorityData> GetPrioritiesList()
        {
            List<PriorityData> city = new List<PriorityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.ComplaintPriorities.Where(x => x.IsDeleted == false)
                            .Select
                            (
                                u => new PriorityData
                                {
                                    ID = u.Id,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            city.Insert(0, new PriorityData
            {
                ID = 0,
                Name = "--Select Priority--"
            });

            return city;
        }
        public static List<ComplaintStatus> GetComplaintStatusList()
        {
            List<ComplaintStatus> city = new List<ComplaintStatus>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.ComplaintStatus.Where(x => x.IsDeleted == false)
                            .Select
                            (
                                u => new ComplaintStatus
                                {
                                    ID = u.Id,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }
        public static List<ComplaintLaunchedBy> GetLaunchedByList()
        {
            List<ComplaintLaunchedBy> city = new List<ComplaintLaunchedBy>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.ComplaintlaunchedBies.Where(x => x.IsDeleted == false)
                            .Select
                            (
                                u => new ComplaintLaunchedBy
                                {
                                    ID = u.Id,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }
        public static List<ComplaintStatus> GetComplaintTypeList()
        {
            List<ComplaintStatus> city = new List<ComplaintStatus>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.ComplaintTypes.Where(x => x.IsDeleted == false)
                            .Select
                            (
                                u => new ComplaintStatus
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
            city.Insert(0, new ComplaintStatus
            {
                ID = 0,
                Name = "--Select Type--"
            });

            return city;
        }

        //Get data Of Complaint Registration END



        public static List<SaleOfficerData> GetSaleOfficersByRegionID(int intRegionHeadID)
        {
            List<SaleOfficerData> so = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    so = dbContext.SaleOfficers.Where(c => c.RegionalHeadID == intRegionHeadID && c.IsDeleted == false)
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).ToList();
                    so.Insert(0, new SaleOfficerData
                    {
                        ID = 0,
                        Name = "--Select Sale Officer--"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return so;
        }
        public static List<RegionalHeadData> getRegionalHeadsByRegionID(int intRegionID)
        {
            List<RegionalHeadData> so = new List<RegionalHeadData>();
            RegionalHeadData rhd;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<RegionalHeadRegion> rhr = dbContext.RegionalHeadRegions.Where(x => x.RegionID == intRegionID).ToList();
                    foreach (var item in rhr)
                    {
                        rhd = new RegionalHeadData();
                        rhd.ID = item.RegionHeadID;
                        rhd.Name = dbContext.RegionalHeads.Where(x => x.ID == item.RegionHeadID).Select(x => x.Name).FirstOrDefault();
                        so.Add(rhd);
                        //List<sp_getRegionalHeads_Result> spRH = dbContext.sp_getRegionalHeads(item.RegionHeadID).ToList();
                        //so = dbContext.RegionalHeads.Where(c => c.ID == item.RegionHeadID && c.IsDeleted == false)
                        //        .Select(
                        //            u => new RegionalHeadData
                        //            {
                        //                ID = u.ID,
                        //                Name = u.Name
                        //            }).ToList();
                    }

                    so.Insert(0, new RegionalHeadData
                    {
                        ID = 0,
                        Name = "--Select Regional Head--"
                    });

                }
            }
            catch (Exception)
            {
                throw;
            }

            return so;
        }

       
        public static List<SaleOfficerData> GetFieldOfficersList(int TeamID)
        {
            List<SaleOfficerData> city = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SaleOfficers.Where(x => x.RoleID == 3 && x.RegionalHeadID== TeamID)
                            .Select
                            (
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
            city.Insert(0, new SaleOfficerData
            {
                ID = 0,
                Name = "--Select Field Officer--"
            });

            return city;
        }
        public static List<SaleOfficerData> GetFieldOfficersList()
        {
            List<SaleOfficerData> city = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SaleOfficers.Where(x => x.RoleID == 3)
                            .Select
                            (
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
            city.Insert(0, new SaleOfficerData
            {
                ID = 0,
                Name = "--Select Field Officer--"
            });

            return city;
        }
        // Get All Region Method ...
        public static List<CityData> GetCityList()
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.IsDeleted == false && c.IsActive == true)
                            .Select
                            (
                                u => new CityData
                                {
                                    ID = u.ID,
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

            city.Insert(0, new CityData
            {
                ID = 0,
                Name = "--Select Zone--"
            });

            return city;
        }

      



        public static List<CityData> GetCityListForReport()
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.IsDeleted == false && c.IsActive == true)
                            .Select
                            (
                                u => new CityData
                                {
                                    ID = u.ID,
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

            city.Insert(0, new CityData
            {
                ID = 0,
                Name = "All"
            });

            return city;
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

        
        public static List<FaultTypeData> GetFaultTypesListForReport()
        {
            List<FaultTypeData> city = new List<FaultTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.FaultTypes
                            .Select
                            (
                                u => new FaultTypeData
                                {
                                    Id = u.Id,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            city.Insert(0, new FaultTypeData
            {
                Id = 0,
                Name = "All"
            });


            return city;
        }




       



   



      
        public static List<ComplaintStatus> GetComplaintStatusListForReport()
        {
            List<ComplaintStatus> city = new List<ComplaintStatus>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.ComplaintStatus.Where(x => x.IsDeleted == false)
                            .Select
                            (
                                u => new ComplaintStatus
                                {
                                    ID = u.Id,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            city.Insert(0, new ComplaintStatus
            {
                ID = 0,
                Name = "All"
            });


            return city;
        }


        public static List<Workdone> GetWorkDoneList()
        {
            List<Workdone> WD = new List<Workdone>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    WD = dbContext.WorkDones.Where(x => x.IsActive == true)
                            .Select
                            (
                                u => new Workdone
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
            WD.Insert(0, new Workdone
            {
                ID = 0,
                Name = "All"
            });


            return WD;
        }

        public static List<SaleOfficerData> GetSOList()
        {
            List<SaleOfficerData> SO = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SO = dbContext.SaleOfficers
                            .Select
                            (
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
            SO.Insert(0, new SaleOfficerData
            {
                ID = 0,
                Name = "All"
            });


            return SO;
        }

        public static List<SaleOfficerData> GetFieldOfficerList()
        {
            List<SaleOfficerData> SO = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SO = dbContext.SaleOfficers.Where(u=>u.RoleID==3)
                            .Select
                            (
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
            SO.Insert(0, new SaleOfficerData
            {
                ID = 0,
                Name = "All"
            });


            return SO;
        }






        public static List<ComplaintStatus> GetProgressStatusList()
        {
            List<ComplaintStatus> city = new List<ComplaintStatus>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.ProgressStatus.Where(x => x.Isdeleted == false)
                            .Select
                            (
                                u => new ComplaintStatus
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
            city.Insert(0, new ComplaintStatus
            {
                ID = 0,
                Name = "--Select Progress Status--"
            });

            return city;
        }


      


        public static List<ComplaintStatus> GetProjectsList()
        {
            List<ComplaintStatus> city = new List<ComplaintStatus>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Zones
                            .Select
                            (
                                u => new ComplaintStatus
                                {
                                    ID = u.ID,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
                city.Insert(0, new ComplaintStatus
                {
                    ID = 0,
                    Name = "All"
                });
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }

       

   
        public static List<ComplaintStatus> GetProjectsListForReport()
        {
            List<ComplaintStatus> city = new List<ComplaintStatus>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Zones
                            .Select
                            (
                                u => new ComplaintStatus
                                {
                                    ID = u.ID,
                                    Name = u.Name,

                                }).OrderBy(x => x.Name).ToList();
                }
                city.Insert(0, new ComplaintStatus
                {
                    ID = 0,
                    Name = "All"
                });
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }


        public static List<ComplaintStatus> GetProjectsListForUsers(List<int?> list)
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
                        comlist.Name = dbContext.Zones.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                        city.Add(comlist);
                    }
                  
                }
                city.Insert(0, new ComplaintStatus
                {
                    ID = 0,
                    Name = "All"
                });

            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }


        public static List<ComplaintStatus> GetProjectsListForSDOS(List<int?> list)
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
                        comlist.Name = dbContext.Zones.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                        city.Add(comlist);
                    }

                }
                
               

            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }
        public static List<CityData> GetCityListCombo()
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            city.Insert(0, new CityData
            {
                ID = 0,
                Name = "Select"
            });
            return city;
        }

        // Get Cities List By RegionalHeadID ...
        public static List<CityData> GetCityListByRegionHeadID()
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = from Cities in dbContext.Cities
                               where
                                   (from RegionalHeadRegions in dbContext.RegionalHeadRegions

                                    select new
                                    {
                                        RegionalHeadRegions.RegionID
                                    }).Contains(new { RegionID = Cities.RegionID })
                               select new CityData()
                               {
                                   ID = Cities.ID,
                                   Name = Cities.Name,
                               };

                    return cityList.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetCityListBySOID(int soId)
        {
            IEnumerable<CityData> qryCity;
            List<CityData> cityList = new List<CityData>();

            cityList.Add(new CityData
            {
                ID = 0,
                Name = "All"
            });


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<int> soRetailersCities = dbContext.Retailers.Where(p => p.SaleOfficerID == soId && p.Status).Select(pp => pp.CityID.Value).ToList<int>();
                    qryCity = dbContext.Cities.Where(p => soRetailersCities.Contains(p.ID)).Select(c => new CityData()
                    {
                        ID = c.ID,
                        Name = c.Name,
                    }).OrderBy(x => x.Name).ToList();

                    foreach (var cty in qryCity)
                    {
                        cityList.Add(cty);
                    }

                    return cityList;
                }
            }
            catch (Exception)
            {
                return new List<CityData>();
            }
        }


        // Get Cities List By RegionalHeadID ...
        public static List<CityData> GetCityListByRegionHeadID(int RegionID, string selectText = "Select")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.Cities.Where(c => c.RegionID == RegionID && c.IsDeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }








        public static List<CityData> GetZoneListByProjectID(int RegionID, string selectText = "--Select Zone--")
        {
            List<CityData> cityList = null;

            try
            {
                if (RegionID == 9)
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        cityList = dbContext.Cities.Where(c => c.IsDeleted == false).OrderBy(c => c.Name)
                                    .Select(
                                        u => new CityData
                                        {
                                            ID = u.ID,
                                            Name = u.Name,

                                        }).ToList();
                    }
                    cityList = cityList.OrderBy(x => x.Name).ToList();
                    cityList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = selectText
                    });
                }
                else
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        cityList = dbContext.Cities.Where(c => c.IsDeleted == false && c.ZoneId == RegionID).OrderBy(c => c.Name)
                                    .Select(
                                        u => new CityData
                                        {
                                            ID = u.ID,
                                            Name = u.Name,

                                        }).ToList();
                    }
                    cityList = cityList.OrderBy(x => x.Name).ToList();
                }
                return cityList;





            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetSiteList(int ClientID, int ProjectID, int CityID, int AreaID, int SubDivisionID, string selectText = "--Select Site--")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.Retailers.Where(c => c.RegionID == ClientID && c.ZoneID == ProjectID && c.CityID == CityID && c.AreaID == AreaID && c.SubDivisionID == SubDivisionID && c.IsDeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetFaulttypedetaileList(int ClientID, string selectText = "Select")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.FaultTypeDetails.Where(c => c.FaulttypeID == ClientID && c.IsDeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<CityData> GetFaultTypeDetailListForFaulttypeID(int ClientID)
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.FaultTypeDetails.Where(c => c.FaulttypeID == ClientID && c.IsDeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = "--Select Fault Type Detail--"
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetUpdateProgressStatusListForFaulttypeID(int ClientID)
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.ProgressStatus.Where(c => c.FaulttypeId == ClientID && c.Isdeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = "--Select Progress Status--"
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<CityData> GetWorkDoneStatusListForFaulttypeID(int ClientID)
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.WorkDones.Where(c => c.FaulttypeID == ClientID && c.IsActive == true).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = "--Select Work Done Status--"
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }



        public static List<CityData> WorkDoneDetailList(int ClientID, string selectText = "Select")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.WorkDones.Where(c => c.FaulttypeID == ClientID && c.IsActive == true).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetEquipmenttypedetaileList(int ClientID, string selectText = "Select")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.EquipmentTypes.Where(c => c.EquipmentCategoryID == ClientID && c.IsDeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetWorkDoneDropdown(int ClientID, string selectText = "Select")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.WorkDones.Where(c => c.FaulttypeID == ClientID && c.IsActive == true).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetProgressDetailList(int ClientID, string selectText = "Select")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.ProgressStatus.Where(c => c.FaulttypeId == ClientID && c.Isdeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetProgressDetailListForReport(int ClientID, string selectText = "--Select Progress Status--")
        {
            List<CityData> cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.ProgressStatus.Where(c => c.FaulttypeId == ClientID && c.Isdeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,

                                    }).ToList();
                }
                var ctyList = cityList.OrderBy(x => x.Name).ToList();
                ctyList.Insert(0, new CityData
                {
                    ID = 0,
                    Name = selectText
                });
                return ctyList;

            }
            catch (Exception)
            {
                throw;
            }
        }



        public static string GetSiteIDList(int ClientID)
        {
            string cityList;

            try
            {


                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = dbContext.Retailers.Where(c => c.ID == ClientID && c.IsDeleted == false).Select(c => c.RetailerCode).FirstOrDefault();


                }


                return cityList;

            }
            catch (Exception)
            {
                throw;
            }
        }



        public static List<CityData> GetCityListByRegionID(int RegionID, string selectText = "Select")
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = from Cities in dbContext.Cities
                               where Cities.RegionID == RegionID



                               select new CityData()
                               {
                                   ID = Cities.ID,
                                   Name = Cities.Name,
                                   IsActive = Cities.IsActive
                               };
                    var ctyList = cityList.OrderBy(x => x.Name).ToList();
                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = selectText
                    });
                    return ctyList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }






        public static List<CityData> GetCityListByDealerId(int dealerId)
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = from x in dbContext.Dealers
                               where x.ID == dealerId
                               select new CityData()
                               {
                                   ID = x.CityID ?? 0,
                                   Name = x.City.Name,
                                   IsActive = x.City.IsActive
                               };
                    var ctyList = cityList.ToList();
                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });
                    return ctyList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetCityListByDealerRetailerCityIds(List<int> retailerCityIds)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var ctyList = dbContext.Cities.Where(x => retailerCityIds.Contains(x.ID) && x.IsActive && !x.IsDeleted)
                         .Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    IsActive = u.IsActive
                                }).ToList();

                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });
                    return ctyList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get Cities List By RegionID ...
        public static List<CityData> GetCityListByRegionID(int intRegionID)
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.RegionID == intRegionID && c.IsDeleted == false).OrderBy(c => c.Name)
                            .Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    RegionID = u.RegionID,
                                    RegionName = u.Region.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }


        public static List<SaleOfficerData> GetSOListByRegionID(int intRegionID)
        {
            List<SaleOfficerData> city = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    var regionalheadid = dbContext.RegionalHeadRegions.Where(x => x.RegionID == intRegionID).Select(x => x.RegionHeadID).ToList();

                    city = dbContext.SaleOfficers.Where(c => regionalheadid.Contains(Convert.ToInt32(c.RegionalHeadID)))
                           .Select(
                               u => new SaleOfficerData
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

            return city;
        }



        public static List<SubCategories> GetSubCatList(int intRegionID)
        {
            List<SubCategories> city = new List<SubCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SubCategories.Where(c => c.MainCategID == intRegionID && c.IsDeleted == false).OrderBy(c => c.SubCategDesc)
                            .Select(
                                u => new SubCategories
                                {
                                    ID = u.SubCategID,
                                    SubName = u.SubCategDesc

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }


        public static List<SubCategoryA> GetSubCatAList(int intRegionID, int RegionID1)
        {
            List<SubCategoryA> city = new List<SubCategoryA>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SubCtegoryAs.Where(c => c.MainCategID == intRegionID && c.SubCategID == RegionID1 && c.IsDeleted == false).OrderBy(c => c.SubCategADesc)
                            .Select(
                                u => new SubCategoryA
                                {
                                    SubCategoryAID = u.SubCategIDA,
                                    SubCategoryAName = u.SubCategADesc

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }



        public static List<SubCategoryA> GetSubCatAList(int intRegionID)
        {
            List<SubCategoryA> city = new List<SubCategoryA>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SubCtegoryAs.Where(c => c.SubCategID == intRegionID && c.IsDeleted == false).OrderBy(c => c.SubCategADesc)
                            .Select(
                                u => new SubCategoryA
                                {
                                    ID = u.SubCategIDA,
                                    SubCategoryAName = u.SubCategADesc

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }




        // Get Cities List By RegionID ...
        public static List<CityData> LoadCitiesListRelatedToRegionalHead(int CityID)
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var SO = dbContext.SaleOfficers.Where(s => s.ID == SalesOfficerID && s.IsDeleted == false).Select(r => r.RegionalHeadID).FirstOrDefault();
                    // List<int> Regions = dbContext.RegionalHeadRegions.Where(r => r.RegionHeadID == SO).Select(s => s.RegionID).ToList();
                    city = dbContext.Cities.Where(c => c.RegionID == CityID).Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList(); ;

                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }


        // Insert OR Update City ...
        public static int AddUpdateCity(CityData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        City CityObj = new City();

                        if (obj.ID == 0)
                        {
                            CityObj.ID = dbContext.Cities.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            CityObj.Name = obj.Name;
                            CityObj.RegionID = obj.RegionID;
                            CityObj.ShortCode = obj.ShortCode;
                            CityObj.IsActive = true;
                            CityObj.CreatedDate = DateTime.Now;
                            CityObj.LastUpdate = DateTime.Now;
                            CityObj.CreatedBy = 1;

                            dbContext.Cities.Add(CityObj);
                        }

                        Area AreaObj = new Area();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Areas.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = CityObj.ID;
                            AreaObj.IsActive = true;
                            AreaObj.CreatedDate = DateTime.Now;
                            AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.CreatedBy = 1;

                            dbContext.Areas.Add(AreaObj);
                        }


                        else
                        {
                            CityObj = dbContext.Cities.Where(u => u.ID == obj.ID).FirstOrDefault();
                            CityObj.Name = obj.Name;
                            CityObj.RegionID = obj.RegionID;
                            CityObj.ShortCode = obj.ShortCode;
                            CityObj.LastUpdate = DateTime.Now;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add City Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code City"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        // Delete City ...
        public static int DeleteCity(int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    City obj = dbContext.Cities.Where(u => u.ID == CityID).FirstOrDefault();
                    dbContext.Cities.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete City Failed");
                Resp = 1;
            }
            return Resp;
        }

        //GET Edit One City
        public static CityData GetEditCity(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Cities.Where(i => i.ID == CityID && i.IsActive == true && i.IsDeleted == false).Select(i => new CityData
                    {
                        ID = i.ID,
                        Name = i.Name,
                        RegionID = i.RegionID,
                        RegionName = i.Region.Name,
                        ShortCode = i.ShortCode,
                        LastUpdate = i.LastUpdate,
                        IsDeleted = i.IsDeleted,
                        IsActive = i.IsActive,
                        CreatedBy = i.CreatedBy
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




        public static ZoneData GetEditProjects(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Zones.Where(i => i.ID == CityID).Select(i => new ZoneData
                    {
                        ID = i.ID,
                        Name = i.Name,
                        ClientId = i.ClientId,
                        SapNo = i.SapNo,
                        CreatedOn=i.CreatedOn.ToString(),
                        AONO = i.AONO,
                        ContractNo = i.ContractNo,
                        ContractTitle = i.ContractTitle,
                        Duration = i.Duration,
                        AwardDate = i.AwardDate,
                        DateTo = i.DateTo,
                        DateFrom = i.DateFrom
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public static int DeleteProjects(int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Zone obj = dbContext.Zones.Where(u => u.ID == CityID).FirstOrDefault();
                    dbContext.Zones.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete City Failed");
                Resp = 1;
            }
            return Resp;
        }



        // Get All Cities For Grid ...
        public static List<CityData> GetCityForGrid(int intRegionID)
        {
            List<CityData> cityData = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityData = dbContext.Cities.Where(u => u.RegionID == intRegionID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    RegionID = u.RegionID,
                                    Name = u.Name,
                                    RegionName = u.Region.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load City Grid Failed");
                throw;
            }

            return cityData;
        }


        public static List<MainCategories> GetMainCategoryForGrid()
        {
            List<MainCategories> cityData = new List<MainCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityData = dbContext.MainCategories.Where(u => u.IsDeleted == false)
                            .ToList().Select(
                                u => new MainCategories
                                {
                                    ID = u.MainCategID,
                                    MainCategoryName = u.MainCategDesc

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load MainCategory Grid Failed");
                throw;
            }

            return cityData;
        }

        public static int AddUpdateMainCategory(MainCategories obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        MainCategory CityObj = new MainCategory();

                        if (obj.ID == 0)
                        {
                            CityObj.MainCategID = dbContext.MainCategories.OrderByDescending(u => u.MainCategID).Select(u => u.MainCategID).FirstOrDefault() + 1;
                            CityObj.MainCategDesc = obj.MainCategoryName;
                            CityObj.IsActive = true;
                            CityObj.IsDeleted = false;
                            CityObj.IsActive = true;
                            CityObj.CreatedOn = DateTime.Now;
                            CityObj.UpdatedOn = DateTime.Now;
                            CityObj.CreatedBy = 1;
                            CityObj.UpdatedBy = 1;

                            dbContext.MainCategories.Add(CityObj);
                        }
                        else
                        {
                            CityObj = dbContext.MainCategories.Where(u => u.MainCategID == obj.ID).FirstOrDefault();
                            CityObj.MainCategDesc = obj.MainCategoryName;
                            CityObj.IsActive = true;
                            CityObj.IsDeleted = false;
                            CityObj.IsActive = true;
                            CityObj.CreatedOn = DateTime.Now;
                            CityObj.UpdatedOn = DateTime.Now;
                            CityObj.CreatedBy = 1;
                            CityObj.UpdatedBy = 1;

                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add MainCategory Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code City"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static MainCategories GetEditMAinCategory(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.MainCategories.Where(i => i.MainCategID == CityID && i.IsActive == true && i.IsDeleted == false).Select(i => new MainCategories
                    {
                        ID = i.MainCategID,
                        MainCategoryName = i.MainCategDesc,

                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int DeleteMAinCategory(int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    MainCategory obj = dbContext.MainCategories.Where(u => u.MainCategID == CityID).FirstOrDefault();
                    dbContext.MainCategories.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete MainCategory Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static List<CityData> GetResult(string search, string sortOrder, int start, int length, List<CityData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<CityData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        public static List<MainCategories> GetResult1(string search, string sortOrder, int start, int length, List<MainCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count1(string search, List<MainCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<CityData> FilterResult(string search, List<CityData> dtResult, List<string> columnFilters)
        {
            IQueryable<CityData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[5].ToLower())))
                );

            return results;
        }

        private static IQueryable<MainCategories> FilterResult1(string search, List<MainCategories> dtResult, List<string> columnFilters)
        {
            IQueryable<MainCategories> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.MainCategoryName != null && p.MainCategoryName.ToLower().Contains(search.ToLower()))
                //&& (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[5].ToLower())))
                ));

            return results;
        }


        public static List<MainCategories> GetMainCatList()
        {
            List<MainCategories> city = new List<MainCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.MainCategories.Where(c => c.IsDeleted == false).OrderBy(c => c.MainCategDesc)
                            .Select(
                                u => new MainCategories
                                {
                                    ID = u.MainCategID,
                                    MainCategoryName = u.MainCategDesc

                                }).ToList();

                    return city;
                }
            }
            catch (Exception)
            {
                throw;
            }


        }




        // Insert OR Update Project ...
        public static int AddUpdateProject(ZoneData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Zone ZoneObj = new Zone();

                        if (obj.ID == 0)
                        {
                            ZoneObj.ID = dbContext.Zones.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            ZoneObj.Name = obj.Name;
                            ZoneObj.SapNo = obj.SapNo;
                            ZoneObj.AONO = obj.AONO;
                            ZoneObj.ContractNo = obj.ContractNo;
                            ZoneObj.ContractTitle = obj.ContractTitle;
                            ZoneObj.AwardDate = obj.AwardDate;
                            ZoneObj.Duration = obj.Duration;
                            ZoneObj.DateFrom = obj.DateFrom;
                            ZoneObj.DateTo =obj.DateTo;
                            ZoneObj.ClientId = obj.ClientId;
                            ZoneObj.CreatedOn = DateTime.Now;
                            ZoneObj.CreatedBy = 1;
                            dbContext.Zones.Add(ZoneObj);
                        }


                        else
                        {
                            ZoneObj = dbContext.Zones.Where(u => u.ID == obj.ID).FirstOrDefault();
                            ZoneObj.Name = obj.Name;
                            ZoneObj.SapNo = obj.SapNo;
                            ZoneObj.AONO = obj.AONO;
                            ZoneObj.ContractNo = obj.ContractNo;
                            ZoneObj.ContractTitle = obj.ContractTitle;
                            ZoneObj.AwardDate = obj.AwardDate;
                            ZoneObj.Duration = obj.Duration;
                            ZoneObj.DateFrom = obj.DateFrom;
                            ZoneObj.DateTo = obj.DateTo;
                            ZoneObj.ClientId = obj.ClientId;
                            ZoneObj.CreatedOn = DateTime.Now;
                            ZoneObj.CreatedBy = 1;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Projects Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code City"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        // Delete City ...
        public static int DeleteProject(int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Zone obj = dbContext.Zones.Where(u => u.ID == CityID).FirstOrDefault();
                    dbContext.Zones.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete City Failed");
                Resp = 1;
            }
            return Resp;
        }

        //GET Edit One City
        public static ZoneData GetEditProject(int ProjectID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Zones.Where(i => i.ID == ProjectID).Select(i => new ZoneData
                    {
                        ID = i.ID,
                        Name = i.Name,
                        SapNo = i.SapNo,

                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Get All Cities For Grid ...
        public static List<ZoneData> GetProjectsForGrid(int intRegionID)
        {
            List<ZoneData> cityData = new List<ZoneData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityData = dbContext.Zones.Where(u => u.ClientId == intRegionID)
                            .ToList().Select(
                                u => new ZoneData
                                {
                                    ID = u.ID,
                                    ClientName = dbContext.Regions.Where(x => x.ID == u.ClientId).Select(x => x.Name).FirstOrDefault(),
                                    Name = u.Name,
                                    SapNo = u.SapNo,
                                    CreatedOn= u.CreatedOn.ToString(),
                                    ClientId = u.ClientId,
                                    AONO = u.AONO,
                                    AwardDate = u.AwardDate,
                                    ContractNo = u.ContractNo,
                                    ContractTitle = u.ContractTitle,
                                    Duration = u.Duration,
                                    DateFrom = u.DateFrom,
                                    DateTo = u.DateTo,
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Project Grid Failed");
                throw;
            }

            return cityData;
        }


        public static List<ZoneData> GetResultProject(string search, string sortOrder, int start, int length, List<ZoneData> dtResult, List<string> columnFilters)
        {
            return FilterResultProject(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int CountProject(string search, List<ZoneData> dtResult, List<string> columnFilters)
        {
            return FilterResultProject(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<ZoneData> FilterResultProject(string search, List<ZoneData> dtResult, List<string> columnFilters)
        {
            IQueryable<ZoneData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower())))


                );

            return results;
        }

        public static List<AreaData> GetAreaList(int intRegionID)
        {
            List<AreaData> city = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Areas.Where(c => c.CityID == intRegionID && c.IsDeleted == false).OrderBy(c => c.Name)
                            .Select(
                                u => new AreaData
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

            return city;
        }

    }
}