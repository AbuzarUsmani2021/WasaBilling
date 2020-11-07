using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class CommonController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult GetCitiesList(int regionalHeadId) // now getting DEALERID 
        {
            try
            {
                var dealerId = regionalHeadId; // now r getting cities on basis of DealerID
                if (dealerId > 0)
                {
                    var retailerCityIds = ManageRetailer.GetDealerRetailerCityIdsList(regionalHeadId);
                    var cities = ManageCity.GetCityListByDealerRetailerCityIds(retailerCityIds);
                    if (cities != null && cities.Count > 0)
                    {
                        return Ok(new
                        {
                            Cities = cities.Where(s => s.IsActive).Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Cities List API Failed");
            }

            return Ok(new
            {
                Cities = new { }
            });
        }

        public IHttpActionResult GetAreasList(int cityId)
        {
            try
            {
                if (cityId > 0)
                {
                    var areas = ManageArea.GetAreaListByCityID(cityId);
                    if (areas != null && areas.Count > 0)
                    {
                        return Ok(new
                        {
                            Areas = areas.Where(s => s.IsActive).Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Areas List API Failed");
            }

            return Ok(new
            {
                Areas = new { }
            });
        }


        public IHttpActionResult GetZonesList(int cityId)
        {
            try
            {
                if (cityId > 0)
                {
                    var zones = ManageZone.GetZonesByCityID(cityId);
                    if (zones != null && zones.Count > 0)
                    {
                        return Ok(new
                        {
                            Zones = zones.Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
                else
                {
                    var zones = ManageZone.GetZoneList();
                    if (zones != null && zones.Count > 0)
                    {
                        return Ok(new
                        {
                            Zones = zones.Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Zones by CITYID List API Failed");
            }

            return Ok(new
            {
                Zones = new { }
            });
        }


        [Route("api/FeesStruct")]
        [HttpGet]
        public List<FeeStructure> FeesStruct()
        {
            List<FeeStructure> cities = new List<FeeStructure>();
            FeeStructure fee;

            var dbFees = db.FeeStructures.Where(c => c.IsActive).ToList();

            foreach (var dbCty in dbFees)
            {
                fee = new FeeStructure();
                fee.FeeStructID = dbCty.FeeStructID;
                fee.FeeStructName = dbCty.FeeStructName;

                cities.Add(fee);
            }

            return cities;
        }


        [Route("api/GetCities")]
        [HttpGet]
        public List<Regions> GetCities(int? ID)
        {
            List<Regions> cities = new List<Regions>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = (from i in db.RegionalHeadRegions
                             join re in db.Regions on i.RegionID equals re.ID
                             where (i.RegionHeadID == ID)
                             select new Regions
                             {
                                 ID = re.ID,
                                 Name = re.Name
                             }).ToList();

            
           // var dbRegions=db.Regions.Where(x=>dbCities.Contains())

            //foreach (var dbCty in dbCities)
            //{
            //    cty = new Regions();
            //    cty.ID = dbCty.ID;
            //    cty.Name = dbCty.Name;

            //    cities.Add(cty);
            //}

            return dbregions;
        }


        [Route("api/SyllabusOffered")]
        [HttpGet]
        public List<Customers> SyllabusOffered()
        {
            List<Customers> Syllabus = new List<Customers>();
            Customers cty;

            var dbCities = db.Retailers.Where(c => c.IsActive).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new Customers();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                Syllabus.Add(cty);
            }

            return Syllabus;
        }


        [Route("api/CustomersRrelatedToSoForCheckin/{Id}")]
        [HttpGet]
        public List<CustomersForCheckin> CustomersRrelatedToSoForCheckin(int Id)
        {
            List<CustomersForCheckin> CustomerValidate = new List<CustomersForCheckin>();
            CustomersForCheckin cty;

            var dbCities = db.Retailers.Where(c => c.SaleOfficerID == Id && c.IsActive == true).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new CustomersForCheckin();
                cty.ID = dbCty.ID;
                cty.ShopName = dbCty.ShopName;
                cty.ISActive = dbCty.IsActive;

                CustomerValidate.Add(cty);
            }

            return CustomerValidate;
        }


        [Route("api/CustomersRrelatedToSoForCheckin/{Id}")]
        [HttpGet]
        public List<CustomersForCheckin> CustomersRrelatedRegionIDForCheckin(int Id)
        {
            List<CustomersForCheckin> CustomerValidate = new List<CustomersForCheckin>();
            CustomersForCheckin cty;

            var dbCities = db.Retailers.Where(c => c.RegionID == Id && c.IsActive == true).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new CustomersForCheckin();
                cty.ID = dbCty.ID;
                cty.ShopName = dbCty.ShopName;
                cty.ISActive = dbCty.IsActive;

                CustomerValidate.Add(cty);
            }

            return CustomerValidate;
        }


        public List<CustomersForCheckin> DistributorRrelatedToSoForCheckin(int Id)
        {
            List<CustomersForCheckin> CustomerValidate = new List<CustomersForCheckin>();
            CustomersForCheckin cty;

            var dbCities = db.Dealers.Where(c => c.SaleOfficerID == Id && c.IsActive == true).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new CustomersForCheckin();
                cty.ID = dbCty.ID;
                cty.ShopName = dbCty.ShopName;
                cty.ISActive = dbCty.IsActive;

                CustomerValidate.Add(cty);
            }

            return CustomerValidate;
        }

        public string GetProgressStatusForWasa(int SOID)
        {
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            string remarks = "";
            

            var result1 = db.Sp_MyComplaintListRemarks1_2(1, dtFromToday, dtToToday).ToList();

            foreach (var item in result1)
            {
                if (item.ComplaintID == SOID)
                {
                    remarks = item.ProgressStatusName;

                }

            }


            return remarks;
        }


        public int? GetResolvedHour(int SOID)
        {
            int? Id = db.Jobs.Where(x => x.ID == SOID).Select(x => x.ResolvedHours).FirstOrDefault();


            return Id;
        }


        public string GetAssignedSaleOfficerNAme(int? SOID)
        {
            var name = "";
            if (SOID != 0)
            {
                name = db.SaleOfficers.Where(x => x.ID == SOID).Select(x => x.Name).FirstOrDefault();
            }
            else
            {
                name = null;
            }
            return name;
           
        }

        public List<ClientRemarks> GetClientRemarks(int SOID)
        {
            List<Projects> cities = new List<Projects>();
            //Regions cty;
            Projects cty;


            var dbregions = db.ClientRemarks.Where(x=>x.ComplaintID==SOID).Select(x => new ClientRemarks
            {
                ID = x.ID,
                
                Remarks = x.ClientRemarks+"/"+x.RemarksByName,
                ComplaintID=x.ComplaintID,
                LaunchedAt=x.RemarksDate,

            }).ToList();


            return dbregions;
        }
        public List<Projects> GetProjects(int SOID)
        {
            List<Projects> cities = new List<Projects>();
            //Regions cty;
            Projects cty;


            var Ids = db.SOProjects.Where(x => x.SaleOfficerID == SOID).Select(x => x.ProjectID).ToList();


            foreach (var item in Ids)
            {
                cty = new Projects();
                cty.ID = item;
                cty.Name = db.Zones.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();


                cities.Add(cty);
            }

            cities.Insert(0, new Projects
            {
                ID = 0,
                Name = "Select"
            });

            // var dbRegions=db.Regions.Where(x=>dbCities.Contains())

            //foreach (var dbCty in dbCities)
            //{
            //    cty = new Regions();
            //    cty.ID = dbCty.ID;
            //    cty.Name = dbCty.Name;

            //    cities.Add(cty);
            //}

            return cities;
        }
        public List<Projects> GetRegionalHeadTypeID(int SOID)
        {
            List<Projects> cities = new List<Projects>();
            //Regions cty;



            var Ids = db.RegionalHeads.Where(x => x.ID == SOID).Select(x => x.Type).ToList();


            foreach (var item in Ids)
            {
                cities = db.RegionalHeadsTypes.Where(x => x.ID == item).Select(x => new Projects
                {
                    ID = x.ID,
                    Name = x.Type
                }).ToList();

            }

            cities.Insert(0, new Projects
            {
                ID = 0,
                Name = "Select"
            });

            // var dbRegions=db.Regions.Where(x=>dbCities.Contains())

            //foreach (var dbCty in dbCities)
            //{
            //    cty = new Regions();
            //    cty.ID = dbCty.ID;
            //    cty.Name = dbCty.Name;

            //    cities.Add(cty);
            //}

            return cities;
        }


        public List<FaultType> GetFaultTypes()
        {
            List<FaultType> cities = new List<FaultType>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = db.FaultTypes.Select(x => new FaultType
            {
                ID = x.Id,
                Name = x.Name
            }).ToList();
          

            // var dbRegions=db.Regions.Where(x=>dbCities.Contains())

            //foreach (var dbCty in dbCities)
            //{
            //    cty = new Regions();
            //    cty.ID = dbCty.ID;
            //    cty.Name = dbCty.Name;

            //    cities.Add(cty);
            //}

            return dbregions;
        }


        public List<Priority> GetPriorities()
        {
            List<Priority> cities = new List<Priority>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = db.ComplaintPriorities.Select(x => new Priority
            {
                ID = x.Id,
                Name = x.Name
            }).ToList();
            dbregions.Insert(0, new Priority
            {
                ID = 0,
                Name = "Select"
            });

            // var dbRegions=db.Regions.Where(x=>dbCities.Contains())

            //foreach (var dbCty in dbCities)
            //{
            //    cty = new Regions();
            //    cty.ID = dbCty.ID;
            //    cty.Name = dbCty.Name;

            //    cities.Add(cty);
            //}

            return dbregions;
        }
        public List<ComplaintType> GetComplaintTypes()
        {
            List<ComplaintType> cities = new List<ComplaintType>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = db.ComplaintTypes.Select(x => new ComplaintType
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();


            dbregions.Insert(0, new ComplaintType
            {
                ID = 0,
                Name = "Select"
            });

            return dbregions;
        }

        public List<ComplaintType> GetLaunchedBy()
        {
            List<ComplaintType> cities = new List<ComplaintType>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = db.ComplaintlaunchedBies.Select(x => new ComplaintType
            {
                ID = x.Id,
                Name = x.Name
            }).ToList();


            dbregions.Insert(0, new ComplaintType
            {
                ID = 0,
                Name = "Select"
            });

            return dbregions;
        }

        public List<ComplaintType> GetRole()
        {
            List<ComplaintType> cities = new List<ComplaintType>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = db.SORoles.Select(x => new ComplaintType
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();


            dbregions.Insert(0, new ComplaintType
            {
                ID = 0,
                Name = "Select"
            });

            return dbregions;
        }
        public List<WorkDone> GetWorkDoneStatuses()
        {
            List<ComplaintType> cities = new List<ComplaintType>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = db.WorkDones.Select(x => new WorkDone
            {
                ID = x.ID,
                Name = x.Name,
                FaultTypeID=x.FaulttypeID
            }).ToList();


            dbregions.Insert(0, new WorkDone
            {
                ID = 0,
                Name = "Select"
            });

            return dbregions;
        }


        public List<ComplaintStatus> GetComplaintStatus()
        {
            List<ComplaintStatus> cities = new List<ComplaintStatus>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = db.ComplaintStatus.Select(x => new ComplaintStatus
            {
                ID = x.Id,
                Name = x.Name
            }).ToList();

            dbregions.Insert(0, new ComplaintStatus
            {
                ID = 0,
                Name = "Select"
            });
            // var dbRegions=db.Regions.Where(x=>dbCities.Contains())

            //foreach (var dbCty in dbCities)
            //{
            //    cty = new Regions();
            //    cty.ID = dbCty.ID;
            //    cty.Name = dbCty.Name;

            //    cities.Add(cty);
            //}

            return dbregions;
        }

        [Route("api/GetMainCategory")]
        [HttpGet]
        public List<MainCategories> MainCat()
        {
            List<MainCategories> MAinCat = new List<MainCategories>();
            MainCategories cty;

            var dbMainCat = db.MainCategories.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new MainCategories();
                cty.MainCategID = dbCty.MainCategID;
                cty.MainCategDesc = dbCty.MainCategDesc;

                MAinCat.Add(cty);
            }

            return MAinCat;
        }



        public List<RetailerType> RetailerType()
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.Tbl_RetailerClass.Where(c => c.Status == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new RetailerType();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.RetailerClass;

                MAinCat.Add(cty);
            }

            return MAinCat;
        }




        public List<RetailerType> RetailerType1()
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.Tbl_RetailerType.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new RetailerType();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.RetailerType;

                MAinCat.Add(cty);
            }

            return MAinCat;
        }


        public List<AllSaleOfficers> SalesOfficersNames(int saleofficerID)
        {  
            List<AllSaleOfficers> MAinCat = new List<AllSaleOfficers>();
            AllSaleOfficers cty;
            List<AllSaleOfficers> list;


            if (saleofficerID > 0)
            {
                //string SOName = "";
                var dbMainCat = db.Tbl_Access.Where(c => c.RepotedUP == saleofficerID && c.Status == true).Select(c => new
                {
                    id = c.ReportedDown
                });

                foreach (var dbCty in dbMainCat)
                {
                    cty = new AllSaleOfficers();
                    cty.ID = dbCty.id;
                    cty.Name = db.SaleOfficers.Where(x => x.ID == dbCty.id && x.IsActive==true).Select(x => x.Name).FirstOrDefault();

                    MAinCat.Add(cty);
                }
             
               
            }
            else
            {

            }


            return MAinCat.OrderBy(x=>x.Name).ToList();
        }



        public List<AllSaleOfficers> AssignedSalesOfficersNames(int HeadID)
        {
            List<AllSaleOfficers> MAinCat = new List<AllSaleOfficers>();
           


           
                //string SOName = "";
                var dbMainCat = db.SaleOfficers.Where(c => c.RegionalHeadID==HeadID && c.RoleID==3  && c.IsActive == true).Select(c => new AllSaleOfficers
                {
                    ID = c.ID,
                    Name=c.Name
                    
                }).ToList();


            dbMainCat.Insert(0, new AllSaleOfficers
            {
                ID = 0,
                Name = "Select"
            });





            return dbMainCat.OrderBy(x => x.ID).ToList();
        }

        public List<City> FollowUp()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_FollowupReasons.Where(c => c.Status == true).ToList();

                foreach (var dbCty in dbMainCat)
                {
                    cty = new City();
                    cty.ID = dbCty.ID;
                   cty.Name = dbCty.Name;

                    MAinCat.Add(cty);
                }


           


            return MAinCat.OrderBy(x => x.Name).ToList();
        }




        public List<AllSaleOfficers> SalesOfficers(int? RegionalHeadID, int saleofficerID)
        {
            List<AllSaleOfficers> MAinCat = new List<AllSaleOfficers>();
            AllSaleOfficers cty;


            //if (RegionalHeadID == 5)
            //{
            //    if (saleofficerID == 23)
            //    {
            //        var dbMainCat = db.SaleOfficers.Where(c => c.IsActive == true).ToList();

            //        foreach (var dbCty in dbMainCat)
            //        {
            //            cty = new AllSaleOfficers();
            //            cty.ID = dbCty.ID;
            //            cty.Name = dbCty.Name;

            //            MAinCat.Add(cty);
            //        }
            //    }
            //    else
            //    {

            //    }

            //}

            if (RegionalHeadID == 4)
            {

                if (saleofficerID == 1)
                {
                    var dbMainCat2 = db.SaleOfficers.Where(c => c.RegionalHeadID == RegionalHeadID).ToList();

                    foreach (var dbCty in dbMainCat2)
                    {
                        cty = new AllSaleOfficers();
                        cty.ID = dbCty.ID;
                        cty.Name = dbCty.Name;

                        MAinCat.Add(cty);
                    }
                }
                else
                {
                    var dbMainCat2 = db.SaleOfficers.Where(c => c.ID == saleofficerID).ToList();

                    foreach (var dbCty in dbMainCat2)
                    {
                        cty = new AllSaleOfficers();
                        cty.ID = dbCty.ID;
                        cty.Name = dbCty.Name;

                        MAinCat.Add(cty);
                    }

                }
            }
            //else if (RegionalHeadID == 10)
            //{

            //    if (saleofficerID == 18)
            //    {
            //        var dbMainCat2 = db.SaleOfficers.Where(c => c.RegionalHeadID == RegionalHeadID).ToList();

            //        foreach (var dbCty in dbMainCat2)
            //        {
            //            cty = new AllSaleOfficers();
            //            cty.ID = dbCty.ID;
            //            cty.Name = dbCty.Name;

            //            MAinCat.Add(cty);
            //        }
            //    }
            //    else
            //    {
            //        var dbMainCat2 = db.SaleOfficers.Where(c => c.ID == saleofficerID).ToList();

            //        foreach (var dbCty in dbMainCat2)
            //        {
            //            cty = new AllSaleOfficers();
            //            cty.ID = dbCty.ID;
            //            cty.Name = dbCty.Name;

            //            MAinCat.Add(cty);
            //        }

            //    }
            //}
            //else
            //{
            //    var dbMainCat2 = db.SaleOfficers.Where(c => c.RegionalHeadID == RegionalHeadID).ToList();

            //    foreach (var dbCty in dbMainCat2)
            //    {
            //        cty = new AllSaleOfficers();
            //        cty.ID = dbCty.ID;
            //        cty.Name = dbCty.Name;

            //        MAinCat.Add(cty);
            //    }
            //}



            return MAinCat;
        }



    }
}