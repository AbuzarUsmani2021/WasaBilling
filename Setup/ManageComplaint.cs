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
    public class ManageComplaint
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

        // Get SalesOfficer Area ID With Comma Seperator ...
        public static string GetSaleOfficerAreaID(int intSaleOfficerID)
        {
            String strAreaName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == intSaleOfficerID).FirstOrDefault();
            strAreaName = string.Join(",", objSaleOfficer.Areas.Select(p => p.ID));

            return strAreaName;
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


        // Get All SalesOfficer Related To CityID ...
        public static List<SaleOfficerData> GetSaleOfficerByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> compData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false).ToList()
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

            return compData;
        }

        // Get All Complaints List For Grid ...
        public static List<SaleOfficerData> GetSaleOfficerList()
        {
            List<SaleOfficerData> compData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.SaleOfficers.Where(u => u.IsDeleted == false).ToList()
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
                                    AreaName =  GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return compData;
        }

        // Get All Complaints List For Grid ...
        public static List<ComplaintData> GetComplaintListForGrid()
        {
            List<ComplaintData> compData = new List<ComplaintData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.Complaints.ToList()
                            .Select(
                                u => new ComplaintData
                                {
                                    ComplaintID = u.ComplaintID,
                                    RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                    RetailerId = u.RetailerId,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                    SaleOfficerID = u.SaleOfficerId,
                                    Title = u.Title,
                                    Detail = u.Detail,
                                    RetailerName = u.Retailer.ShopName,
                                    Remarks = string.IsNullOrEmpty(u.Remarks) ? "" : u.Remarks,
                                    DueDateString = u.DueDate.ToString("dd-MMM-yyyy"),
                                    Priority = u.Priority.ToString(),
                                    PriorityName = u.Priority.ToString(),
                                    Status = u.Status.ToString(),
                                    StatusName = u.Status.ToString(),
                                    RemUpdatedOn = string.IsNullOrEmpty(u.Remarks) && !u.RemUpdatedOn.HasValue ? "" : "Added on " + u.RemUpdatedOn.Value.ToString("dd-MMM-yyyy HH:mm"),

                                }).ToList();

                    foreach (var item in compData)
                    {
                        if(int.Parse(item.PriorityName) == (int)PriorityEnum.Low)
                        {
                            item.PriorityName = PriorityEnum.Low.ToString();
                        }
                        else if (int.Parse(item.PriorityName) == (int)PriorityEnum.Medium)
                        {
                            item.PriorityName = PriorityEnum.Medium.ToString();
                        }
                        else if (int.Parse(item.PriorityName) == (int)PriorityEnum.High)
                        {
                            item.PriorityName = PriorityEnum.High.ToString();
                        }

                        if (int.Parse(item.StatusName) == (int)StatusEnum.Pending)
                        {
                            item.StatusName = StatusEnum.Pending.ToString();
                        }
                        else if (int.Parse(item.StatusName) == (int)StatusEnum.Completed)
                        {
                            item.StatusName = StatusEnum.Completed.ToString();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return compData;
        }

        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerList()
        {
            List<SaleOfficer> compData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                compData = dbContext.SaleOfficers.ToList();
            }
            return compData;
        }

        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerListRelatedtoregionalHeadID(int RHID)
        {
            List<SaleOfficer> compData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RHID == 0)
                {
                    compData = dbContext.SaleOfficers.ToList();
                }
                else
                {
                    compData = dbContext.SaleOfficers.Where(s => s.RegionalHeadID == RHID).ToList();
                }
                
            }
            return compData;
        }

        //Insert OR Update Complaints ...
        public static Boolean AddUpdateComplaints(ComplaintData obj)
        {
            Boolean boolFlag = false;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Complaint saleofficerObj = new Complaint();

                    if (obj.ComplaintID == 0)
                    {
                        //saleofficerObj.ComplaintID = dbContext.Complaints.OrderByDescending(u => u.ComplaintID).Select(u => u.ComplaintID).FirstOrDefault() + 1;
                        saleofficerObj.Title = obj.Title;
                        saleofficerObj.Detail = obj.Detail;
                        saleofficerObj.RetailerId = obj.RetailerId;
                        saleofficerObj.SaleOfficerId = obj.SaleOfficerID.Value;
                        //saleofficerObj.CityID = 1;
                        //saleofficerObj.IsActive = true;
                        //saleofficerObj.IsDeleted = false;
                        saleofficerObj.DueDate = DateTime.Parse(obj.DueDateString);
                        saleofficerObj.Priority = int.Parse(obj.Priority);
                        saleofficerObj.Status = int.Parse(obj.Status);

                        saleofficerObj.CreatedOn = DateTime.Now;
                        //saleofficerObj.UpdatedOn = DateTime.Now;
                        //Created By Work Pending...
                        saleofficerObj.CreatedBy = 1;

                        dbContext.Complaints.Add(saleofficerObj);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        saleofficerObj = dbContext.Complaints.Where(u => u.ComplaintID == obj.ComplaintID).FirstOrDefault();
                        if (saleofficerObj != null)
                        {
                            saleofficerObj.Title = obj.Title;
                            saleofficerObj.Detail = obj.Detail;
                            saleofficerObj.RetailerId = obj.RetailerId;
                            saleofficerObj.SaleOfficerId = obj.SaleOfficerID.Value;
                            saleofficerObj.DueDate = DateTime.Parse(obj.DueDateString);
                            saleofficerObj.Priority = int.Parse(obj.Priority);
                            saleofficerObj.Status = int.Parse(obj.Status);
                            saleofficerObj.UpdatedOn = DateTime.Now;
                            saleofficerObj.UpdatedBy = 1;
                        }
                        
                        dbContext.SaveChanges();
                    }

                    boolFlag = true;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add/update Complaint Failed");
                boolFlag = false;
            }
            return boolFlag;
        }

        // Delete DeleteComplaint ...
        public static int DeleteComplaint(int todoID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Complaint objSaleOfficer = dbContext.Complaints.Where(u => u.ComplaintID == todoID).FirstOrDefault();
                    dbContext.Complaints.Remove(objSaleOfficer);
                    //obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch(Exception exp)
            {
                Log.Instance.Error(exp, "Delete Complaint Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static List<ComplaintData> GetResult(string search, string sortOrder, int start, int length, List<ComplaintData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ComplaintData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<ComplaintData> FilterResult(string search, List<ComplaintData> dtResult, List<string> columnFilters)
        {
            IQueryable<ComplaintData> results = dtResult.AsQueryable();

            //results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.AreaName != null && p.AreaName.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower())
            //    || p.Phone2 != null && p.Phone2.ToLower().Contains(search.ToLower())))
            //    && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
            //    && (columnFilters[3] == null || (p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(columnFilters[3].ToLower())))
            //     && (columnFilters[4] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[3].ToLower())))
            //      && (columnFilters[5] == null || (p.AreaName != null && p.AreaName.ToLower().Contains(columnFilters[3].ToLower())))
            //    && (columnFilters[6] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[4].ToLower())))
            //    && (columnFilters[7] == null || (p.Phone2 != null && p.Phone2.ToLower().Contains(columnFilters[5].ToLower())))
            //    );

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



        public static List<SaleOfficerData> GetSaleOfficerListByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> compData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false).ToList()
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

            return compData;
        }
    }
}