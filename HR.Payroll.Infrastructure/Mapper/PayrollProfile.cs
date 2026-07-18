using AutoMapper;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using HR.BaseInfo.Core.DTOs;
using HR.Identity.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.Data.EmployeeRelated;
using HR.Payroll.Core.DTOs;
using HR.SharedKernel.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Infrastructure.Mapper;

public class PayrollProfile : Profile
{
    public PayrollProfile()
    {
        CreateMap<OrganisationEmployeeTypeFicheItem, OrganisationEmployeeTypeFicheItemDTO>()
          .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
          .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
          .ForMember(dest => dest.EnterType, opt => opt.MapFrom(src => src.EnterType == null ? "" : src.EnterType.title))
          .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType == null ? "" : src.PaymentType.title))
          .ForMember(dest => dest.OrganisationCheckFormula, opt => opt.MapFrom(src => src.OrganisationCheckFormula == null ? "" : src.OrganisationCheckFormula.title))
          .ForMember(dest => dest.OrganisationFormula, opt => opt.MapFrom(src => src.OrganisationFormula == null ? "" : src.OrganisationFormula.title))
          ;
        CreateMap<OrganisationEmployeeTypeFicheItemDTO, OrganisationEmployeeTypeFicheItem>();

        CreateMap<OrganisationEmployeeTypeFundTypeDefinition, OrganisationEmployeeTypeFundTypeDefinitionDTO>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
          .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
          .ForMember(dest => dest.EmployeeWageItem, opt => opt.MapFrom(src => src.EmployeeWageItem == null ? "" : src.EmployeeWageItem.title))
          .ForMember(dest => dest.EmployerWageItem, opt => opt.MapFrom(src => src.EmployerWageItem == null ? "" : src.EmployerWageItem.title))
          .ForMember(dest => dest.EmployeeFormula, opt => opt.MapFrom(src => src.EmployeeFormula == null ? "" : (src.EmployeeFormula.Formula == null ? "" : src.EmployeeFormula.Formula.title)))
          .ForMember(dest => dest.EmployerFormula, opt => opt.MapFrom(src => src.EmployerFormula == null ? "" : (src.EmployerFormula.Formula == null ? "" : src.EmployerFormula.Formula.title)))
          .ForMember(dest => dest.FundType, opt => opt.MapFrom(src => src.FundType == null ? "" : src.FundType.title))
          ;
        CreateMap<OrganisationEmployeeTypeFundTypeDefinitionDTO, OrganisationEmployeeTypeFundTypeDefinition>();
        CreateMap<PersonnelFunction, PersonnelFunctionDTO>()
          .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
        .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
        .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
            ;

        CreateMap<TempPersonnelFunction, TempPersonnelFunctionDTO>()
          .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
        .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
        .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
            ;

        CreateMap<PersonnelFunctionDTO, PersonnelFunction>()
          .ForMember(dest => dest.CostCenter, opt => opt.Ignore())
          .ForMember(dest => dest.OrganizationUnit, opt => opt.Ignore())
          .ForMember(dest => dest.WorkPlace, opt => opt.Ignore())
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.Employee, opt => opt.Ignore())
          .ForMember(dest => dest.ArearsStatus, opt => opt.Ignore())
          .ForMember(dest => dest.PersonnelFunctionExcelFile, opt => opt.Ignore());
        CreateMap<PaymentPeriod, PaymentPeriodDTO>().ReverseMap();
        // PersonnelFunctionVisible mappings
        CreateMap<PersonnelFunctionVisible, PersonnelFunctionVisibleDTO>().ReverseMap();





        CreateMap<PaymentType, PaymentTypeDTO>().ReverseMap();
        CreateMap<DeductionType, DeductionTypeDTO>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
          .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
          .ForMember(dest => dest.SettlementItem, opt => opt.MapFrom(src => src.SettlementItem == null ? "" : src.SettlementItem.title))
          ;
        CreateMap<DeductionTypeDTO, DeductionType>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore());
        CreateMap<TaxTable, TaxTableDTO>().ReverseMap();
        CreateMap<BankDisketteTemplate, BankDisketteTemplateDTO>().ReverseMap();

        CreateMap<BankDisketteTemplate, BankDisketteTemplateDTO>()
       .ForMember(dest => dest.Bank, opt => opt.MapFrom(src => src.Bank == null ? "" : src.Bank.title));
        CreateMap<BankDisketteTemplateDTO, BankDisketteTemplate>();



        CreateMap<PaymentPeriodEmployeeBonus, PaymentPeriodEmployeeBonusDTO>()
.ForMember(dest => dest.Coefficient, opt => opt.MapFrom(src => src.Coefficient == null ? "" : src.Coefficient.title))
.ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
;
        CreateMap<PaymentPeriodEmployeeBonusDTO, PaymentPeriodEmployeeBonus>();






        CreateMap<TaxNonCashPayment, TaxNonCashPaymentDTO>()
        .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType == null ? "" : src.ItemType.title))
        .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
        ;
        CreateMap<TaxNonCashPaymentDTO, TaxNonCashPayment>();

        CreateMap<FunctionExcelDefinition, FunctionExcelDefinitionDTO>()
        .ForMember(dest => dest.MappedExcelColumn, opt => opt.MapFrom(src => src.MappedExcelColumn == null ? "" : src.MappedExcelColumn.title))
        .ForMember(dest => dest.PersonnelFunctionColumn, opt => opt.MapFrom(src => src.PersonnelFunctionColumn == null ? "" : src.PersonnelFunctionColumn.title + " - ( " + src.PersonnelFunctionColumn.Value + " ) "))
        .ForMember(dest => dest.ExcelDefinitionType, opt => opt.MapFrom(src => src.ExcelDefinitionType == null ? "" : src.ExcelDefinitionType.title))
        .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType == null ? "" : src.LeaveType.title))
        .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
        ;
        CreateMap<FunctionExcelDefinitionDTO, FunctionExcelDefinition>();

        CreateMap<TaxDisketteWH, TaxDisketteWhDTO>()
        .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
        .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
        .ForMember(dest => dest.ActiveName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.ActiveName))
        .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType == null ? "" : src.PaymentType.title))
       ;
        CreateMap<TaxNonCashPaymentDTO, TaxDisketteWH>();

        // TaxDisketteWK mapping
        CreateMap<TaxDisketteWK, TaxDisketteWkDTO>()
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType == null ? "" : src.PaymentType.title));

        // TaxDisketteWP mapping
        CreateMap<TaxDisketteWP, TaxDisketteWpDTO>()
            .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? src.NationalNo : src.Employee.NationalNo))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? src.FirstName : src.Employee.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? src.LastName : src.Employee.LastName))
            .ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => src.Employee == null ? src.FatherName : src.Employee.FatherName))
            .ForMember(dest => dest.IdentityNo, opt => opt.MapFrom(src => src.IdentityNo))
            ;


        CreateMap<InsuranceDisketteItem, InsuranceDisketteItemDTO>()
        .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
        .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
        .ForMember(dest => dest.ActiveName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.ActiveName))
        .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : src.CostCenter.title))
        ;
        CreateMap<InsuranceDisketteItemDTO, InsuranceDisketteItem>();


        CreateMap<TaxDiskette, TaxDisketteDTO>()
        .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
        .ForMember(dest => dest.TaxDisketteStatus, opt => opt.MapFrom(src => src.TaxDisketteStatus == null ? "" : src.TaxDisketteStatus.title))
        ;
        CreateMap<TaxDisketteDTO, TaxDiskette>();

        CreateMap<OrganisationFicheItem, OrganisationFicheItemDTO>()
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
       .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
       .ForMember(dest => dest.EnterType, opt => opt.MapFrom(src => src.EnterType == null ? "" : src.EnterType.title))
       .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType == null ? "" : src.PaymentType.title))
        ;
        CreateMap<OrganisationFicheItemDTO, OrganisationFicheItem>();

        CreateMap<BankDisketteTemplateRow, BankDisketteTemplateRowDTO>()
        .ForMember(dest => dest.BankDisketteTemplate, opt => opt.MapFrom(src => src.BankDisketteTemplate.Bank == null ? "" : src.BankDisketteTemplate.Bank.title))
        .ForMember(dest => dest.DisketteItemType, opt => opt.MapFrom(src => src.DisketteItemType == null ? "" : src.DisketteItemType.title))
        ;
        CreateMap<BankDisketteTemplateRowDTO, BankDisketteTemplateRow>();

        CreateMap<BankBranch, BankBranchDTO>()
       .ForMember(dest => dest.Bank, opt => opt.MapFrom(src => src.Bank == null ? "" : src.Bank.title))
       ;
        CreateMap<BankBranchDTO, BankBranch>();



        CreateMap<TaxCoefficientItem, TaxCoefficientItemDTO>()
       .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
       .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax == null ? "" : (src.Tax.EmployeeType == null ? "" : src.Tax.EmployeeType.title)))
         ;
        CreateMap<TaxCoefficientItemDTO, TaxCoefficientItem>();


        CreateMap<BlackList, BlackListDTO>()
          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
            ;

        CreateMap<BlackListDTO, BlackList>();


        CreateMap<BlockedAccount, BlockedAccountDTO>()

          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
            ;

        CreateMap<BlockedAccountDTO, BlockedAccount>();




        CreateMap<InsuranceBranch, InsuranceBranchDTO>()

          .ForMember(dest => dest.InsuranceType, opt => opt.MapFrom(src => src.InsuranceType == null ? "" : src.InsuranceType.title))
            ;

        CreateMap<InsuranceBranchDTO, InsuranceBranch>();

        CreateMap<PersonnelLoan, PersonnelLoanDTO>()

          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
          .ForMember(dest => dest.LoanType, opt => opt.MapFrom(src => src.LoanType == null ? "" : src.LoanType.title))

            ;

        CreateMap<PersonnelLoanDTO, PersonnelLoan>();

        CreateMap<PersonnelLoanPayment, PersonnelLoanPaymentDTO>()
          .ForMember(dest => dest.PersonnelLoan, opt => opt.MapFrom(src => src.PersonnelLoan == null ? "" : (src.PersonnelLoan.LoanType == null ? "" : src.PersonnelLoan.LoanType.title)))
          .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType == null ? "" : src.PaymentType.title))
            ;

        CreateMap<PersonnelLoanPaymentDTO, PersonnelLoanPayment>();

        // PersonnelPayment mappings
        CreateMap<PersonnelPayment, PersonnelPaymentDTO>()
          .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType == null ? "" : src.PaymentType.title))
          .ForMember(dest => dest.BankBranch, opt => opt.MapFrom(src => src.BankBranch == null ? "" : src.BankBranch.title))
          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : (src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo)))
          .ForMember(dest => dest.PaymentPeriodId, opt => opt.MapFrom(src => src.Fiche == null ? (long?)null : src.Fiche.PaymentPeriodId))
          .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.Fiche == null ? "" : (src.Fiche.PaymentPeriod == null ? "" : src.Fiche.PaymentPeriod.title)))
          .ForMember(dest => dest.FicheStatus, opt => opt.MapFrom(src => src.Fiche == null ? "" : (src.Fiche.FicheStatus == null ? "" : src.Fiche.FicheStatus.title)))
          .ForMember(dest => dest.IsClosed, opt => opt.MapFrom(src => src.Fiche == null ? false : (src.Fiche.PaymentPeriod == null ? false : src.Fiche.PaymentPeriod.IsClosed)));

        CreateMap<Tax, TaxDTO>()
          .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
          .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
          ;

        CreateMap<TaxDTO, Tax>();

        CreateMap<PersonnelLoan, PersonnelLoanDTO>()
          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
          .ForMember(dest => dest.LoanType, opt => opt.MapFrom(src => src.LoanType == null ? "" : src.LoanType.title))
          .ForMember(dest => dest.BankBranch, opt => opt.MapFrom(src => src.BankBranch == null ? "" : src.BankBranch.title))
          .ForMember(dest => dest.StartDeductPaymentPeriod, opt => opt.MapFrom(src => src.StartDeductPaymentPeriod == null ? "" : src.StartDeductPaymentPeriod.title))
          ;

        CreateMap<PersonnelLoanDTO, PersonnelLoan>();


        CreateMap<LoanType, LoanTypeDTO>()
          
          .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
          .ForMember(dest => dest.SettlementItem, opt => opt.MapFrom(src => src.SettlementItem == null ? "" : src.SettlementItem.title))

            ;

        CreateMap<LoanTypeDTO, LoanType>();


        CreateMap<PersonnelFicheItem, PersonnelFicheItemDTO>()
          .ForMember(dest => dest.PaymentInterval, opt => opt.MapFrom(src => src.PaymentInterval == null ? "" : src.PaymentInterval.title))
          .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
          .ForMember(dest => dest.EnterType, opt => opt.MapFrom(src => src.EnterType == null ? "" : src.EnterType.title))

            ;

        CreateMap<PersonnelFicheItemDTO, PersonnelFicheItem>();

        CreateMap<BankBranchDTO, BankBranch>();

        CreateMap<BankBranch, BankBranchDTO>()

                        .ForMember(dest => dest.Bank, opt => opt.MapFrom(src => src.Bank == null ? "" : src.Bank.title))
            ;
        CreateMap<ArearsChangedFicheItem, ArearsChangedFicheItemDTO>()
                    .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
                    .ForMember(dest => dest.DifferenceAmount, opt => opt.MapFrom(src => src.CurrentAmount - src.LastAmount))
        ;

        CreateMap<Arear, ArearDTO>()
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
       .ForMember(dest => dest.ArearsStatus, opt => opt.MapFrom(src => src.ArearsStatus == null ? "" : src.ArearsStatus.title))
       .ForMember(dest => dest.ApproveTimePaymentPeriod, opt => opt.MapFrom(src => src.ApproveTimePaymentPeriod == null ? "" : src.ApproveTimePaymentPeriod.title))
       .ForMember(dest => dest.PaymentPeriodIntendToPay, opt => opt.MapFrom(src => src.PaymentPeriodIntendToPay == null ? "" : src.PaymentPeriodIntendToPay.title))
       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
       .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
       .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
       .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName))
       .ForMember(dest => dest.Serial, opt => opt.MapFrom(src => src.InterdictOrder == null ? (short?)null : src.InterdictOrder.Serial))
       .ForMember(dest => dest.InterdictOrder, opt => opt.MapFrom(src => src.InterdictOrder == null ? "" : src.InterdictOrder.Serial.ToString()))
       .ForMember(dest => dest.PersonnelFunction, opt => opt.MapFrom(src => src.PersonnelFunctionId == null ? "" : src.PersonnelFunctionId.ToString()))
        ;


        CreateMap<ArearFiche, ArearFicheDTO>()
       .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
       .ForMember(dest => dest.PaymentPeriodIntendToPay, opt => opt.MapFrom(src => src.PaymentPeriodIntendToPay == null ? "" : src.PaymentPeriodIntendToPay.title))
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
       .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : src.CostCenter.title))
       .ForMember(dest => dest.PeymanRow, opt => opt.MapFrom(src => src.PeymanRow == null ? "" : src.PeymanRow.title))
       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
       .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
       .ForMember(dest => dest.Serial, opt => opt.MapFrom(src => src.InterdictOrder == null ? 0 : src.InterdictOrder.Serial))
       .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.InterdictOrder == null ? DateTime.MinValue : src.InterdictOrder.StartDate))
       .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.InterdictOrder == null ? DateTime.MinValue : src.InterdictOrder.EndDate))
       .ForMember(dest => dest.PayRollApproveUser, opt => opt.MapFrom(src => src.InterdictOrder == null ? "" : src.InterdictOrder.PayRollApproveUser))
       .ForMember(dest => dest.PayRollRealExecuteDate, opt => opt.MapFrom(src => src.InterdictOrder == null ? DateTime.MinValue : src.InterdictOrder.PayRollRealExecuteDate))
       .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.InterdictOrder == null ? (src.Employee == null ? "" : src.Employee.PersonelCode) : src.InterdictOrder.PersonelCode))
       .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
       .ForMember(dest => dest.FicheStatus, opt => opt.MapFrom(src => src.FicheStatus == null ? "" : src.FicheStatus.title))
       .ForMember(dest => dest.FicheType, opt => opt.MapFrom(src => "معوقه"))
       .ForMember(dest => dest.ChangedItemsCount, opt => opt.MapFrom(src => src.ArearsChangedFicheItems == null ? 0 : src.ArearsChangedFicheItems.Count(c => c.IsDeleted != true)))
       .ForMember(dest => dest.ChangedItemsDifferenceSum, opt => opt.MapFrom(src => src.ArearsChangedFicheItems == null ? 0 : src.ArearsChangedFicheItems.Where(c => c.IsDeleted != true).Sum(c => c.CurrentAmount - c.LastAmount)))
       ;

        CreateMap<Fiche, FicheDTO>()
       .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
       .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : src.CostCenter.title))
       .ForMember(dest => dest.PeymanRow, opt => opt.MapFrom(src => src.PeymanRow == null ? "" : src.PeymanRow.title))
       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
       .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
       .ForMember(dest => dest.Serial, opt => opt.MapFrom(src => src.InterdictOrder == null ? 0 : src.InterdictOrder.Serial))
       .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.InterdictOrder == null ? (src.Employee == null ? "" : src.Employee.PersonelCode) : src.InterdictOrder.PersonelCode))
       .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
               .ForMember(dest => dest.FicheStatus, opt => opt.MapFrom(src => src.FicheStatus == null ? "" : src.FicheStatus.title))
       .ForMember(dest => dest.FicheType, opt => opt.MapFrom(src => "عادی"))
       ;

        CreateMap<FicheLeaveItem, FicheLeaveItemDTO>()
       .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType == null ? "" : src.LeaveType.title))
       .ForMember(dest => dest.PersonnelLeave, opt => opt.MapFrom(src => src.PersonnelLeave == null ? "" : src.PersonnelLeave.title))
       ;



        CreateMap<BatchLog, BatchLogDTO>()
       .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
       .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName))
       .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
       .ForMember(dest => dest.LogTypeTitle, opt => opt.MapFrom(src =>
           src.LogTypeId == (int)Enums.BatchLoggerRecordType.OrderArear ? "محاسبه معوقه حکمی" :
           src.LogTypeId == (int)Enums.BatchLoggerRecordType.BankDiskette ? "دیسکت بانک" :
           src.LogTypeId.ToString()))
       ;


        CreateMap<FicheItem, FicheItemDTO>()
       .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
       .ForMember(dest => dest.ArearPaymentPeriod, opt => opt.MapFrom(src => src.ArearPaymentPeriod == null ? "" : src.ArearPaymentPeriod.title))
       .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.Fiche == null ? "" : (src.Fiche.PaymentPeriod == null ? "" : src.Fiche.PaymentPeriod.title)))
       .ForMember(dest => dest.PersonnelLoan, opt => opt.MapFrom(src => src.PersonnelLoan == null ? "" : src.PersonnelLoan.title))
       .ForMember(dest => dest.RemainDeductionAmount, opt => opt.MapFrom(src => src.RemainDeductionAmount))
       .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentTypeId == 11232 ? "پرداختی" : (src.PaymentTypeId == 11233 ? "کسور" : "")))
        ;

        CreateMap<ArearFicheItem, ArearFicheItemDTO>()
       .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
       .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentTypeId == 11232 ? "پرداختی" : (src.PaymentTypeId == 11233 ? "کسور" : "")))
       .ForMember(dest => dest.ArearPaymentPeriod, opt => opt.MapFrom(src => src.ArearPaymentPeriod == null ? "" : src.ArearPaymentPeriod.title))
       .ForMember(dest => dest.PersonnelLoan, opt => opt.MapFrom(src => src.PersonnelLoan == null ? "" : src.PersonnelLoan.title))
        ;

        CreateMap<CalclulationSetting, CalclulationSettingDTO>()
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
       .ForMember(dest => dest.RewardFormula, opt => opt.MapFrom(src => src.RewardFormula == null ? "" : src.RewardFormula.title))
       .ForMember(dest => dest.SanavatFormula, opt => opt.MapFrom(src => src.SanavatFormula == null ? "" : src.SanavatFormula.title))
       .ForMember(dest => dest.RewardAndSanavatStoreType, opt => opt.MapFrom(src => src.RewardAndSanavatStoreType == null ? "" : src.RewardAndSanavatStoreType.title))
        ;

        CreateMap<CalclulationSettingDTO, CalclulationSetting>();

        CreateMap<CostCenterFicheItem, CostCenterFicheItemDTO>()
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
       .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
       .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : src.CostCenter.title))
        ;
        CreateMap<CostCenterFicheItemDTO, CostCenterFicheItem>();


        CreateMap<BatchPayRollRequest, BatchPayRollRequestDTO>()
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
    
       .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
        ;
        CreateMap<BatchPayRollRequestDTO, BatchPayRollRequest>();

        CreateMap<BatchPayRollRequestDetail, BatchPayRollRequestDetailDTO>()
       .ForMember(dest => dest.ActiveName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.ActiveName))
       .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
       .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
       .ForMember(dest => dest.IdentityNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.IdentityNo))
       .ForMember(dest => dest.BatchPayRollRequest, opt => opt.MapFrom(src => src.BatchPayRollRequest == null ? "" : src.BatchPayRollRequest.Id.ToString()))
        ;
        CreateMap<BatchPayRollRequestDetailDTO, BatchPayRollRequestDetail>();

        CreateMap<BatchSettlementRequest, BatchSettlementRequestDTO>()
       .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
       .ForMember(dest => dest.SettlementCause, opt => opt.MapFrom(src => src.SettlementCause == null ? "" : src.SettlementCause.title))
       .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
        ;
        CreateMap<BatchSettlementRequestDTO, BatchSettlementRequest>();

        CreateMap<BatchSettlementRequestDetail, BatchSettlementRequestDetailDTO>()
       .ForMember(dest => dest.ActiveName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.ActiveName))
       .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
       .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
       .ForMember(dest => dest.BatchSettlementRequest, opt => opt.MapFrom(src => src.BatchSettlementRequest == null ? "" : src.BatchSettlementRequest.Id.ToString()))
        ;
        CreateMap<BatchSettlementRequestDetailDTO, BatchSettlementRequestDetail>();



        CreateMap<BankDisketteGroupAndFile, BankDisketteGroupAndFileDTO>()
    .ForMember(dest => dest.BankDisketteTemplate, opt => opt.MapFrom(src => src.BankDisketteTemplate == null ? "" : (src.BankDisketteTemplate.Bank == null ? "" : src.BankDisketteTemplate.Bank.title)))
    ;


        CreateMap<BankDisketteCostCenter, BankDisketteCostCenterDTO>()
    .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : (src.CostCenter.title == null ? "" : src.CostCenter.title)))
    ;

        CreateMap<InsuranceDisketteCostCenter, InsuranceDisketteCostCenterDTO>()
  .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : (src.CostCenter.title == null ? "" : src.CostCenter.title)))
  ;
        CreateMap<TaxDisketteCostCenter, TaxDisketteCostCenterDTO>()
.ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : (src.CostCenter.title == null ? "" : src.CostCenter.title)))
;



        CreateMap<BankDiskette, BankDisketteDTO>()
        .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
        .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
        .ForMember(dest => dest.BankDisketteStatus, opt => opt.MapFrom(src => src.BankDisketteStatus == null ? "" : src.BankDisketteStatus.title))
        //.ForMember(dest => dest.BankDisketteTemplate, opt => opt.MapFrom(src => src.BankDisketteTemplate == null ? "" : (src.BankDisketteTemplate.Bank == null ? "" : src.BankDisketteTemplate.Bank.title)))
        ;
        CreateMap<BankDisketteDTO, BankDiskette>();

        // EmployeeDeduction
        CreateMap<EmployeeDeduction, EmployeeDeductionDTO>()
          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
          .ForMember(dest => dest.DeductionType, opt => opt.MapFrom(src => src.DeductionType == null ? "" : src.DeductionType.title))
          .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
          .ForMember(dest => dest.StartDeductPaymentPeriod, opt => opt.MapFrom(src => src.StartDeductPaymentPeriod == null ? "" : src.StartDeductPaymentPeriod.title))
          ;
        CreateMap<EmployeeDeductionDTO, EmployeeDeduction>()
          .ForMember(dest => dest.Employee, opt => opt.Ignore())
          .ForMember(dest => dest.DeductionType, opt => opt.Ignore())
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.StartDeductPaymentPeriod, opt => opt.Ignore());

        // EmployeeFund
        CreateMap<EmployeeFund, EmployeeFundDTO>()
          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
          .ForMember(dest => dest.FundType, opt => opt.MapFrom(src => src.FundType == null ? "" : src.FundType.title))
          .ForMember(dest => dest.StartDeductPaymentPeriod, opt => opt.MapFrom(src => src.StartDeductPaymentPeriod == null ? "" : src.StartDeductPaymentPeriod.title))
          ;
        CreateMap<EmployeeFundDTO, EmployeeFund>()
          .ForMember(dest => dest.Employee, opt => opt.Ignore())
          .ForMember(dest => dest.FundType, opt => opt.Ignore())
          .ForMember(dest => dest.StartDeductPaymentPeriod, opt => opt.Ignore());

        // EmployeeSettlement
        CreateMap<EmployeeSettlement, EmployeeSettlementDTO>()
          .ForMember(dest => dest.OrganisationChartTitle, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
          .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
          .ForMember(dest => dest.EmployeeTitle, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
          .ForMember(dest => dest.SettlementCauseTitle, opt => opt.MapFrom(src => src.SettlementCause == null ? "" : src.SettlementCause.title))
          .ForMember(dest => dest.SettlementStatusTitle, opt => opt.MapFrom(src => src.SettlementStatus == null ? "" : src.SettlementStatus.title))
          .ForMember(dest => dest.InterdictOrderTitle, opt => opt.MapFrom(src => src.InterdictOrder == null ? "" : src.InterdictOrder.Id.ToString()))
          .ForMember(dest => dest.LastInterdictOrderTitle, opt => opt.MapFrom(src => src.LastInterdictOrder == null ? "" : src.LastInterdictOrder.Id.ToString()))
          .ForMember(dest => dest.FicheTitle, opt => opt.MapFrom(src => src.Fiche == null ? "" : src.Fiche.Id.ToString()))
          .ForMember(dest => dest.SettlementItemIds, opt => opt.Ignore())
          .ForMember(dest => dest.SettlementItems, opt => opt.Ignore());
        CreateMap<EmployeeSettlementDTO, EmployeeSettlement>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.EmployeeType, opt => opt.Ignore())
          .ForMember(dest => dest.Employee, opt => opt.Ignore())
          .ForMember(dest => dest.SettlementCause, opt => opt.Ignore())
          .ForMember(dest => dest.SettlementStatus, opt => opt.Ignore())
          .ForMember(dest => dest.InterdictOrder, opt => opt.Ignore())
          .ForMember(dest => dest.LastInterdictOrder, opt => opt.Ignore())
          .ForMember(dest => dest.Fiche, opt => opt.Ignore());

        CreateMap<EmployeeSettlementItem, EmployeeSettlementItemDTO>()
          .ForMember(dest => dest.SettlementItemTitle, opt => opt.MapFrom(src => src.SettlementItem == null ? "" : src.SettlementItem.title))
          .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount > 0 ? (long?)src.Amount : null))
          .ForMember(dest => dest.SystemCalculatedAmount, opt => opt.MapFrom(src => src.SystemCalculatedAmount > 0 ? (long?)src.SystemCalculatedAmount : null));
        CreateMap<EmployeeSettlementItemDTO, EmployeeSettlementItem>()
          .ForMember(dest => dest.EmployeeSettlement, opt => opt.Ignore())
          .ForMember(dest => dest.SettlementItem, opt => opt.Ignore())
          .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount ?? 0))
          .ForMember(dest => dest.SystemCalculatedAmount, opt => opt.MapFrom(src => src.SystemCalculatedAmount ?? 0));

        CreateMap<EmployeeSettlementAttachment, EmployeeSettlementAttachmentDTO>()
          .ForMember(dest => dest.SettlementDocumentAttachmentTypeTitle, opt => opt.Ignore())
          .ForMember(dest => dest.FileTitle, opt => opt.Ignore())
          .ForMember(dest => dest.FileExtension, opt => opt.Ignore())
          .ForMember(dest => dest.FileSize, opt => opt.Ignore())
          .ForMember(dest => dest.MimeType, opt => opt.Ignore());
        CreateMap<EmployeeSettlementAttachmentDTO, EmployeeSettlementAttachment>()
          .ForMember(dest => dest.EmployeeSettlement, opt => opt.Ignore())
          .ForMember(dest => dest.SettlementDocumentAttachmentType, opt => opt.Ignore())
          .ForMember(dest => dest.File, opt => opt.Ignore());

        CreateMap<HR.Identity.Core.Entities.AspNetUsers, AspNetUsersDTO>()

        .ForMember(dest => dest.Disabled, opt => opt.MapFrom(src => !src.LockoutEnabled))


            ;



        CreateMap<BankDisketteItem, BankDisketteItemDTO>()
        .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName))
        .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
        ;
        CreateMap<BankDisketteItemDTO, BankDisketteItem>();

        CreateMap<InsuranceDiskette, InsuranceDisketteDTO>()
        .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
        .ForMember(dest => dest.InsuranceBranch, opt => opt.MapFrom(src => src.InsuranceBranch == null ? "" : src.InsuranceBranch.WorkshopName + " ( " + src.InsuranceBranch.WorkshopCode + " ) "))
        .ForMember(dest => dest.InsuranceDisketteStatus, opt => opt.MapFrom(src => src.InsuranceDisketteStatus == null ? "" : src.InsuranceDisketteStatus.title))
        .ForMember(dest => dest.ReportType, opt => opt.MapFrom(src => src.ReportType == null ? "" : src.ReportType.title))
        .ForMember(dest => dest.PeymanRow, opt => opt.MapFrom(src => src.PeymanRow == null ? "" : src.PeymanRow.title + " - " + (src.PeymanRow.Code)))
        .ForMember(dest => dest.BatchPayRollRequest, opt => opt.MapFrom(src => src.BatchPayRollRequest == null ? "" : src.BatchPayRollRequest.title))
        .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))

        ;
        CreateMap<InsuranceDisketteDTO, InsuranceDiskette>();




        CreateMap<InsuranceType, InsuranceTypeDTO>().ReverseMap();
        CreateMap<OrganProperty, OrganPropertyDTO>().ReverseMap();
        CreateMap<Bank, BankDTO>().ReverseMap();
        CreateMap<MinimumMonthlyWage, MinimumMonthlyWageDTO>().ReverseMap();



        CreateMap<OrganisationLeave, OrganisationLeaveDTO>()
          .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType == null ? "" : src.LeaveType.title))
          ;
        CreateMap<OrganisationLeaveDTO, OrganisationLeave>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.LeaveType, opt => opt.Ignore());

        CreateMap<OrganisationFundType, OrganisationFundTypeDefinitionDTO>()
          .ForMember(dest => dest.FundType, opt => opt.MapFrom(src => src.FundType == null ? "" : src.FundType.title))
          ;
        CreateMap<OrganisationFundTypeDefinitionDTO, OrganisationFundType>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.FundType, opt => opt.Ignore());


        CreateMap<OrganisationEmployeeTypeLeave, OrganisationEmployeeTypeLeaveDTO>()
          .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType == null ? "" : src.LeaveType.title))
          .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
          ;
        CreateMap<OrganisationEmployeeTypeLeaveDTO, OrganisationEmployeeTypeLeave>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.LeaveType, opt => opt.Ignore())
          .ForMember(dest => dest.EmployeeType, opt => opt.Ignore());


        CreateMap<OrganisationEmployeeTypeCoefficientBonusWageItem, OrganisationEmployeeTypeCoefficientBonusWageItemDTO>()
          .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
          .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
          .ForMember(dest => dest.Coefficient, opt => opt.MapFrom(src => src.Coefficient == null ? "" : src.Coefficient.title))
            ;
        CreateMap<OrganisationEmployeeTypeCoefficientBonusWageItemDTO, OrganisationEmployeeTypeCoefficientBonusWageItem>();


        CreateMap<EmployeeLeaveEntitlement, EmployeeLeaveEntitlementDTO>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
          .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType == null ? "" : src.LeaveType.title))
          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : (src.Employee.FirstName + " " + src.Employee.LastName)))
          .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
          .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
          .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
          .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
          ;
        CreateMap<EmployeeLeaveEntitlementDTO, EmployeeLeaveEntitlement>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.LeaveType, opt => opt.Ignore())
          .ForMember(dest => dest.Employee, opt => opt.Ignore());

        CreateMap<PersonnelLeave, PersonnelLeaveDTO>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
          .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType == null ? "" : src.LeaveType.title))
          .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
          .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : (src.Employee.FirstName + " " + src.Employee.LastName)))
          .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
          .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
          .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
          .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
          ;
        CreateMap<PersonnelLeaveDTO, PersonnelLeave>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.LeaveType, opt => opt.Ignore())
          .ForMember(dest => dest.PaymentPeriod, opt => opt.Ignore())
          .ForMember(dest => dest.Employee, opt => opt.Ignore());

        // PersonnelFunctionExcelFile mappings
        CreateMap<PersonnelFunctionExcelFile, PersonnelFunctionExcelFileDTO>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
          .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.title))
          .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => src.CreateDate))
          .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src => src.AspNetUsers == null ? "" : (src.AspNetUsers.FirstName + " " + src.AspNetUsers.LastName)))
          .ForMember(dest => dest.PaymentPeriod, opt => opt.MapFrom(src => src.PaymentPeriod == null ? "" : src.PaymentPeriod.title))
          .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title));
        
        CreateMap<PersonnelFunctionExcelFileDTO, PersonnelFunctionExcelFile>()
          .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
          .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.FileName))
          .ForMember(dest => dest.PaymentPeriod, opt => opt.Ignore())
          .ForMember(dest => dest.EmployeeType, opt => opt.Ignore());

    }
}
