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
   public class ManageDropdowns
    {
        // Fauly Type Dropdown Start
        public static List<DropdownData> GetAllFaulttype()
        {
            List<DropdownData> FaultTypeAAllData = new List<DropdownData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    FaultTypeAAllData = dbContext.FaultTypes
                            .Select(
                                u => new DropdownData
                                {
                                    FaulttypeID = u.Id,
                                    FaulttypeName = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            FaultTypeAAllData.Insert(0, new DropdownData
            {
                FaulttypeID = 0,
                FaulttypeName = "--Select Fault Type--"
            });
            return FaultTypeAAllData;
        }
        // Fauly Type Dropdown END


        // Fauly Type Detail Start

        // Table Start
        public static List<DropdownData> FaulttypedetailFilteredData(int FaultType)
        {
            List<DropdownData> FaultTypeDetailData = new List<DropdownData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if(FaultType==0)
                    {
                        FaultTypeDetailData = dbContext.FaultTypeDetails.Where(x=>x.IsDeleted==false)
                              .Select(
                                  u => new DropdownData
                                  {
                                      FaultyTypeDetailID =u.ID,
                                      FaulttypeName=dbContext.FaultTypes.Where(x=>x.Id== u.FaulttypeID).Select(x=>x.Name).FirstOrDefault(),
                                      FaultyTypeDetailName = u.Name
                                  }).ToList();
                    }
                    else         
                    {
                        FaultTypeDetailData = dbContext.FaultTypeDetails.Where(x => x.IsDeleted == false && x.FaulttypeID== FaultType)
                                                     .Select(
                                                         u => new DropdownData
                                                         {
                                                             FaultyTypeDetailID = u.ID,
                                                             FaulttypeName = dbContext.FaultTypes.Where(x => x.Id == u.FaulttypeID).Select(x => x.Name).FirstOrDefault(),
                                                             FaultyTypeDetailName = u.Name
                                                         }).ToList();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Fault Type Detail List Failed");
                throw;
            }
            return FaultTypeDetailData;
        }
        public static List<DropdownData> GetResult12(string search, string sortOrder, int start, int length, List<DropdownData> dtResult, List<string> columnFilters)
        {
            return FilterResult12(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }
        private static IQueryable<DropdownData> FilterResult12(string search, List<DropdownData> dtResult, List<string> columnFilters)
        {
            IQueryable<DropdownData> results = dtResult.AsQueryable();
            results = results.Where(p => (search == null || (p.FaulttypeName != null && p.FaulttypeName.ToLower().Contains(search.ToLower()) || p.FaultyTypeDetailName != null && p.FaultyTypeDetailName.ToLower().Contains(search.ToLower())))
                 && (columnFilters[1] == null || (p.FaulttypeName != null && p.FaulttypeName.ToLower().Contains(columnFilters[1].ToLower())))
                 && (columnFilters[2] == null || (p.FaultyTypeDetailName != null && p.FaultyTypeDetailName.ToLower().Contains(columnFilters[2].ToLower())))
                 );

            return results;

        }
        public static int Count(string search, List<DropdownData> dtResult, List<string> columnFilters)
        {
            return FilterResult12(search, dtResult, columnFilters).Count();
        }

        // Table END

        public static int AddUpdateFaulttypedetail(DropdownData obj)
        {
            int Res = 0;
            using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        FaultTypeDetail Ftd = new FaultTypeDetail();

                        if (obj.FaultyTypeDetailID == 0)
                        {
                            var AlreadyExist = dbContext.FaultTypeDetails.Where(x => x.FaulttypeID == obj.FId && x.Name == obj.FaultyTypeDetailName && x.IsDeleted==false).ToList();
                            if (AlreadyExist.Count == 0)
                            {
                                Ftd.ID = dbContext.FaultTypeDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                Ftd.Name = obj.FaultyTypeDetailName;
                                Ftd.FaulttypeID = obj.FId;
                                Ftd.IsDeleted = false;
                                dbContext.FaultTypeDetails.Add(Ftd);
                                dbContext.SaveChanges();
                                Res = 2;
                                scope.Complete();
                            }
                            else
                            {
                                Res = 3;
                            }

                        }
                        else
                        {
                            Ftd = dbContext.FaultTypeDetails.Where(u => u.ID == obj.FaultyTypeDetailID).FirstOrDefault();
                            Ftd.Name = obj.FaultyTypeDetailName;
                            Ftd.FaulttypeID = obj.FId;
                            Ftd.IsDeleted = false;
                            dbContext.SaveChanges();
                            Res = 4;
                            scope.Complete();

                    }


                    }
                }
            return Res;
        }

        public static DropdownData GetEditFaulttypedetail(int ftd)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.FaultTypeDetails.Where(i => i.ID == ftd).Select(i => new DropdownData
                    {
                        FaultyTypeDetailID = i.ID,
                        FaulttypeID = i.FaulttypeID,
                        FaultyTypeDetailName = i.Name,
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int DeleteFaulttypedetail(int ftd)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    FaultTypeDetail obj = dbContext.FaultTypeDetails.Where(u => u.ID == ftd).FirstOrDefault();
                    obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Fault Type Detail Failed");
                Resp = 1;
            }
            return Resp;
        }

        // Fauly Type Detail END


        // Progress Status Start


        //Table Start
        public static List<DropdownData> GetProgressStatusData(int FaultType)
        {
            List<DropdownData> ProgressStatusData = new List<DropdownData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (FaultType == 0)
                    {
                        ProgressStatusData = dbContext.ProgressStatus.Where(x => x.Isdeleted == false)
                              .Select(
                                  u => new DropdownData
                                  {
                                      ProgressStatusID = u.ID,
                                      ProgressStatusName = u.Name,
                                      FaulttypeName = dbContext.FaultTypes.Where(x => x.Id == u.FaulttypeId).Select(x => x.Name).FirstOrDefault()
                                  }).ToList();
                    }
                    else
                    {
                        ProgressStatusData = dbContext.ProgressStatus.Where(x => x.Isdeleted == false && x.FaulttypeId == FaultType)
                                                     .Select(
                                                         u => new DropdownData
                                                         {
                                                             ProgressStatusID = u.ID,
                                                             ProgressStatusName = u.Name,
                                                             FaulttypeName = dbContext.FaultTypes.Where(x => x.Id == u.FaulttypeId).Select(x => x.Name).FirstOrDefault()
                                                         }).ToList();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Progress Status List Failed");
                throw;
            }
            return ProgressStatusData;
        }

        public static List<DropdownData> GetProgressStatusResult(string search, string sortOrder, int start, int length, List<DropdownData> dtResult, List<string> columnFilters)
        {
            return FilterProgressStatusResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int GetProgressStatusCount(string search, List<DropdownData> dtResult, List<string> columnFilters)
        {
            return FilterProgressStatusResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<DropdownData> FilterProgressStatusResult(string search, List<DropdownData> dtResult, List<string> columnFilters)
        {
            IQueryable<DropdownData> results = dtResult.AsQueryable();
            results = results.Where(p => (search == null || (p.FaulttypeName != null && p.FaulttypeName.ToLower().Contains(search.ToLower()) || p.ProgressStatusName != null && p.ProgressStatusName.ToLower().Contains(search.ToLower())))
                 && (columnFilters[1] == null || (p.FaulttypeName != null && p.FaulttypeName.ToLower().Contains(columnFilters[1].ToLower())))
                 && (columnFilters[2] == null || (p.ProgressStatusName != null && p.ProgressStatusName.ToLower().Contains(columnFilters[2].ToLower())))
                 );

            return results;

        }

        //Table End

        public static int AddUpdateProgressStatus(DropdownData PSData)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        ProgressStatu PS = new ProgressStatu();

                        if (PSData.ProgressStatusID == 0)
                        {
                            var AlreadyExist = dbContext.ProgressStatus.Where(x => x.FaulttypeId == PSData.ProgressStatusFaultTypeID && x.Name == PSData.ProgressStatusName &&x.Isdeleted==false).ToList();
                            if (AlreadyExist.Count == 0)
                            {
                                PS.ID = dbContext.ProgressStatus.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                PS.Name = PSData.ProgressStatusName;
                                PS.FaulttypeId = PSData.ProgressStatusFaultTypeID;
                                PS.Isdeleted = false;
                                PS.CreatedOn = DateTime.Now;
                                dbContext.ProgressStatus.Add(PS);
                                dbContext.SaveChanges();
                                Res = 5;
                                scope.Complete();
                            }
                            else
                            {
                                Res = 6;
                            }

                        }
                        else
                        {
                            PS = dbContext.ProgressStatus.Where(u => u.ID == PSData.ProgressStatusID).FirstOrDefault();
                            PS.Name = PSData.ProgressStatusName;
                            PS.FaulttypeId = PSData.ProgressStatusFaultTypeID;
                            PS.Isdeleted = false;
                            dbContext.SaveChanges();
                            Res = 7;
                            scope.Complete();
                        }


                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Progress Status Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Add Progress Status Failed"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 4;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static DropdownData GetEditProgressStatus(int ProgressStatusID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.ProgressStatus.Where(i => i.ID == ProgressStatusID).Select(i => new DropdownData
                    {
                        ProgressStatusID = i.ID,
                        FaulttypeID = i.FaulttypeId,
                        ProgressStatusName = i.Name,
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int DeleteProgressStatus(int ProgressStatusID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    ProgressStatu obj = dbContext.ProgressStatus.Where(u => u.ID == ProgressStatusID).FirstOrDefault();
                    obj.Isdeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Progress Status  Failed");
                Resp = 1;
            }
            return Resp;
        }



        // Progress Status End




        // Work Done Start

        // Table Start
        public static List<DropdownData> GetWorkDoneTable(int WorkDoneFaulttypeID)
        {
            List<DropdownData> WorkDoneData = new List<DropdownData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (WorkDoneFaulttypeID == 0)
                    {
                        WorkDoneData = dbContext.WorkDones.Where(x => x.IsActive == true)
                              .Select(
                                  u => new DropdownData
                                  {
                                      WorkDoneID = u.ID,
                                      WorkDoneName = u.Name,
                                      WorkDoneFaulttypeName = dbContext.FaultTypes.Where(x => x.Id == u.FaulttypeID).Select(x => x.Name).FirstOrDefault()
                                  }).ToList();
                    }
                    else
                    {
                        WorkDoneData = dbContext.WorkDones.Where(x => x.IsActive == true && x.FaulttypeID == WorkDoneFaulttypeID)
                                                     .Select(
                                                        u => new DropdownData
                                                        {
                                                            WorkDoneID = u.ID,
                                                            WorkDoneName = u.Name,
                                                            WorkDoneFaulttypeName = dbContext.FaultTypes.Where(x => x.Id == WorkDoneFaulttypeID).Select(x => x.Name).FirstOrDefault()
                                                        }).ToList();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Work Done Status List Failed");
                throw;
            }
            return WorkDoneData;
        }
        public static List<DropdownData> GetWorkDoneResult(string search, string sortOrder, int start, int length, List<DropdownData> dtResult, List<string> columnFilters)
        {
            return GetWorkDoneFinalResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }
        public static int GetWorkDoneCount(string search, List<DropdownData> dtResult, List<string> columnFilters)
        {
            return GetWorkDoneFinalResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<DropdownData> GetWorkDoneFinalResult(string search, List<DropdownData> dtResult, List<string> columnFilters)
        {
            IQueryable<DropdownData> results = dtResult.AsQueryable();
            results = results.Where(p => (search == null || (p.WorkDoneFaulttypeName != null && p.WorkDoneFaulttypeName.ToLower().Contains(search.ToLower()) || p.WorkDoneName != null && p.WorkDoneName.ToLower().Contains(search.ToLower())))
                 && (columnFilters[1] == null || (p.WorkDoneFaulttypeName != null && p.WorkDoneFaulttypeName.ToLower().Contains(columnFilters[1].ToLower())))
                 && (columnFilters[2] == null || (p.WorkDoneName != null && p.WorkDoneName.ToLower().Contains(columnFilters[2].ToLower())))
                 );

            return results;

        }

        // Table END

        public static int AddUpdateWorkDone(DropdownData WorkDoneData)
        {
            int Res = 0;
            using (TransactionScope scope = new TransactionScope())
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    WorkDone Ftd = new WorkDone();

                    if (WorkDoneData.WorkDoneID == 0)
                    {
                        var AlreadyExist = dbContext.WorkDones.Where(x => x.FaulttypeID == WorkDoneData.WorkDoneFaulttypeID && x.Name == WorkDoneData.WorkDoneName && x.IsActive == true).ToList();
                        if (AlreadyExist.Count == 0)
                        {
                            Ftd.ID = dbContext.WorkDones.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            Ftd.Name = WorkDoneData.WorkDoneName;
                            Ftd.FaulttypeID = WorkDoneData.WorkDoneFaulttypeID;
                            Ftd.IsActive = true;
                            dbContext.WorkDones.Add(Ftd);
                            dbContext.SaveChanges();
                            Res = 8;
                            scope.Complete();
                        }
                        else
                        {
                            Res = 9;
                        }

                    }
                    else
                    {
                        Ftd = dbContext.WorkDones.Where(u => u.ID == WorkDoneData.WorkDoneID).FirstOrDefault();
                        Ftd.Name = WorkDoneData.WorkDoneName;
                        Ftd.FaulttypeID = WorkDoneData.WorkDoneFaulttypeID;
                        Ftd.IsActive = true;
                        dbContext.SaveChanges();
                        Res = 10;
                        scope.Complete();

                    }


                }
            }
            return Res;
        }

        public static DropdownData GetEditWorkDoneData(int WorkDoneID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.WorkDones.Where(i => i.ID == WorkDoneID).Select(i => new DropdownData
                    {
                        WorkDoneID = i.ID,
                        WorkDoneFaulttypeID = i.FaulttypeID,
                        WorkDoneName = i.Name,
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int DeleteWorkDoneData(int WorkDoneID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    WorkDone obj = dbContext.WorkDones.Where(u => u.ID == WorkDoneID).FirstOrDefault();
                    obj.IsActive = false;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Work Down Status Failed");
                Resp = 1;
            }
            return Resp;
        }

        // Fauly Type Detail END






    }
}
