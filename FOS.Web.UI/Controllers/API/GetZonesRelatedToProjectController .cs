using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetZonesRelatedToProjectController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ProjectID,int SOID)
        {
            try
            {
                if (ProjectID > 0)
                {
                    var SubCat = ManageArea.GetTownsForAPI(ProjectID,SOID);
                    List<CityData> cityList = new List<CityData>();
                    CityData cty;

                   

                        if (ProjectID != 9)
                         {
                        foreach (var item in SubCat)
                        {
                                if (item.ClientId == ProjectID)
                                {
                                    cty = new CityData();

                                    cty.ID = (int)item.ID;
                                    cty.Name = db.Cities.Where(x => x.ID == item.ID).Select(x => x.Name).FirstOrDefault();

                                    cityList.Add(cty);
                                }
                         }
                        cityList.Insert(0, new CityData
                        {
                            ID = 0,
                            Name = "Select"
                        });
                        return Ok(new
                            {
                                Zones = cityList.Select(d => new
                                {
                                    d.ID,
                                    d.Name
                                }).OrderBy(d => d.ID)

                            });

                        }
                        else
                        {
                            return Ok(new
                            {
                                Zones = SubCat.Select(d => new
                                {
                                    d.ID,
                                    d.Name
                                }).OrderBy(d => d.ID)
                            });


                        }

                    //if (SubCat != null && SubCat.Count > 0)
                    //{
                    //    return Ok(new
                    //    {
                    //        Zones = SubCat.Select(d => new
                    //        {
                    //            d.ID,
                    //            d.Name
                    //        }).OrderBy(d => d.ID)
                    //    });
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ZonesController GET API Failed");
            }

            return Ok(new
            {
                Zones = new { }
            });
        }


    }
}