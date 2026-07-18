using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.BaseInfo.Core.Entities;
using OfficeOpenXml;
using HR.Employee.Core.Entities;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hr.Employee.infrastructure.Services;

public class GroupPunishmentEncourageService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService
) : BaseService<GroupPunishmentEncourage, EmployeeContext, GroupPunishmentEncourageDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetExcelPreview(long FileId)
    {
        IbaseFile File;
        File = _unitOfWork.Context.Files.Find(FileId);
        var SuccessCount = ExcelDataToDataTableFunction(new MemoryStream(File.Content), FileId);
        return OperationResult.Succeeded(payload: SuccessCount);
    }
    public long GetEmployeeIdFromNationalNo(string NationalNO)
    {


        using (var connection = new SqlConnection(_connectionString))
        using (var command = connection.CreateCommand())
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[emp].[GetEmployeeIdFromNationalNo]";
            command.Parameters.AddWithValue("@NationalNO", NationalNO);
            SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.BigInt);
            returnValue.Direction = ParameterDirection.ReturnValue;

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return Convert.ToInt64(returnValue.Value);
        }
    }
    public int ExcelDataToDataTableFunction(Stream strm, long FileId)
    {
        try
        {
            ExcelPackage.License.SetNonCommercialOrganization("HRMS");
            var xlPackage = new ExcelPackage(strm);

            List<TempPunishmentEncourage> retList = new List<TempPunishmentEncourage>();
            foreach (var item in xlPackage.Workbook.Worksheets.Take(1))
            {
                int pureDataStartIndex = 1;
                var maxSizeSetting = GetSettingById(10018);

                var propertyList = typeof(TempPunishmentEncourage).GetProperties();

                for (int i = pureDataStartIndex; i <= item.Dimension.End.Row; i++)
                {
                    try
                    {
                        TempPunishmentEncourage row = new()
                        {
                            OrganisationChartId = _currentUserDefaultOrganId,
                            CreateDate = DateTime.Now,
                            IPAddress = "",
                            TempFileId = FileId,
                            //  var data = item.Cells[Convert.ToInt32(i), singleSetting.MappedExcelColumn.Order, Convert.ToInt32(i), singleSetting.MappedExcelColumn.Order];
                            NationalNo = item.Cells[i, 1].Text,
                            UnitValue = Convert.ToInt32(item.Cells[i, 2].Text)
                        };

                        if (!string.IsNullOrEmpty(row.NationalNo))
                        {
                            if (row.NationalNo.Length == 10)
                            {
                                var employeeId = GetEmployeeIdFromNationalNo(row.NationalNo);
                                if (employeeId > 0 && row.UnitValue > 0)
                                {
                                    row.EmployeeId = employeeId;
                                    retList.Add(row);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            _unitOfWork.Context.TempPunishmentEncourages.AddRange(retList);
            _unitOfWork.Context.SaveChanges();
            return retList.Count();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public new async Task<OperationResult> CreateForAsync(GroupPunishmentEncourageDTO entityToCreate)
    {
        try
        {
            var mappedTodo = _mapper.Map<GroupPunishmentEncourage>(entityToCreate);
            if (typeof(GroupPunishmentEncourage).GetInterfaces().Contains(typeof(IOrganisationChartId)))
            {
                if (_currentUserDefaultOrganId > 0)
                {
                    PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                    propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                }
                else
                {
                    throw new Exception("سازمان پیش فرض مشخض نشده است");
                }
            }
            if (entityToCreate.TempFileId > 0)
            {

            }
            else
            {
                return OperationResult.Failed("فایل موقت یافت نشد");
            }
            _unitOfWork.CreateTransaction();

            try
            {
                mappedTodo.title = "تنبیه و تشویق گروهی";
                //mappedTodo.LastModifiedUser = _UserName;

                var templist = _unitOfWork.Context.TempPunishmentEncourages.Where(x => x.TempFileId == entityToCreate.TempFileId).ToList();
                mappedTodo.EmPloyeeCount = templist.Count();    
                Add(mappedTodo);
                if (await _unitOfWork.Save() > 0)
                {
                  //  var templist = _unitOfWork.Context.TempPunishmentEncourages.Where(x => x.TempFileId == entityToCreate.TempFileId);
                    if (templist == null)
                    {
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("هیچ رکوردی جهت تنبیه یا تشویق یافت نشد");
                    }
                    else
                    {
                        if (templist.Any())
                        {
                            List<PunishmentEncourage> MainList = [];
                            foreach (var item in templist)
                            {
                                if (item.EmployeeId.HasValue)
                                {
                                    PunishmentEncourage toAdd = new()
                                    {
                                        EmployeeId = item.EmployeeId.Value,
                                        GroupPunishmentEncourageId = mappedTodo.Id,
                                        OrganisationAgentOfPunishmentEncourageScoreIntervalId = entityToCreate.OrganisationAgentOfPunishmentEncourageScoreIntervalId,
                                        UnitValue = item.UnitValue,
                                        AgentOfPunishmentEncourageId = entityToCreate.AgentOfPunishmentEncourageId,
                                        IsGroup = true,
                                        CreateDate = DateTime.Now,
                                        Description = entityToCreate.Description,
                                        IPAddress = "",
                                        OrganisationChartId = _currentUserDefaultOrganId
                                    };
                                    MainList.Add(toAdd);
                                }
                            }
                            _unitOfWork.Context.PunishmentEncourages.AddRange(MainList);
                            await _unitOfWork.Save();

                            var tempFile = await _unitOfWork.Context.Files.FindAsync(entityToCreate.TempFileId);
                            GroupPunishmentEncourageFile file = new GroupPunishmentEncourageFile()
                            {
                                CreateDate = DateTime.Now,
                                Content = tempFile.Content,
                                Extension = tempFile.Extension,
                                IPAddress = "",
                                GroupPunishmentEncourageId = mappedTodo.Id,
                                MimeType = tempFile.MimeType,
                                Size = tempFile.Size,
                                TempFileId = tempFile.Id,
                                UniqueId = tempFile.UniqueId,
                                title = "استخراج شده از فایل گروهی",

                            };
                            await _unitOfWork.Context.GroupPunishmentEncourageFiles.AddAsync(file);
                            await _unitOfWork.Save();
                            _unitOfWork.Commit();
                            return OperationResult.Succeeded(payload: mappedTodo.Id);
                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("هیچ رکوردی جهت تنبیه یا تشویق یافت نشد");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
            }
            return OperationResult.Failed();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed();
        }

    }
    public bool Validate(HR.Employee.Core.Entities.GroupPunishmentEncourage entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
