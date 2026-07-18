using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

/// <summary>
/// فایل فهرست کارکنان
/// </summary>
[Table("Tax_DisketteWP", Schema = "Payroll")]
public class TaxDisketteWP : BaseEntity, IignoreDateRangeValidation
{
    public TaxDisketteWP()
    {
        NationalNo = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        FatherName = string.Empty;
        BirthPlace = string.Empty;
        EducationGrade = string.Empty;
        InsuranceNo = string.Empty;
        ExemptionType = string.Empty;
        InsuranceName = string.Empty;
        CountryOfCitizenship = string.Empty;
        CountryOfResidence = string.Empty;
        PostalCode = string.Empty;
        Address = string.Empty;
        Position = string.Empty;
    }

    [ForeignKey("TaxDiskette")]
    public long TaxDisketteId { get; set; }
    public virtual TaxDiskette? TaxDiskette { get; set; }
    [ForeignKey("Employee")]
    [Comment("شناسه کارمند")]
    public long EmployeeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }
    [ForeignKey("Fiche")]
    public long FicheId { get; set; }
    public virtual Fiche? Fiche { get; set; }
    /// <summary>
    /// ملیت
    /// </summary>
    public int Nationality { get; set; }
    /// <summary>
    /// کد ملی
    /// </summary>
    [StringLength(100)]
    public string NationalNo { get; set; }
    /// <summary>
    /// نام
    /// </summary>
    [StringLength(100)]
    public string FirstName { get; set; }
    /// <summary>
    /// نام خانوادگی
    /// </summary>
    [StringLength(100)]
    public string LastName { get; set; }
    /// <summary>
    /// نام پدر
    /// </summary>
    [StringLength(100)]
    public string FatherName { get; set; }
    /// <summary>
    /// تاریخ تولد
    /// </summary>
    public int BirthDate { get; set; }
    /// <summary>
    /// ش . ش
    /// </summary>
    public int IdentityNo { get; set; }
    /// <summary>
    /// محل تولد
    /// </summary>
    [StringLength(150)]
    public string BirthPlace { get; set; }
    /// <summary>
    /// مقطع تحصیلی
    /// </summary>
    [StringLength(150)]
    public string EducationGrade { get; set; }
    /// <summary>
    /// نوع بیمه  - جدول 3
    /// </summary>
    public int InsuranceTypeId { get; set; }
    /// <summary>
    /// رشته عددی  10رقمی و بدون خط تیره
    /// </summary>
    [StringLength(10)]
    public string InsuranceNo { get; set; }
    [StringLength(10)]
    public string ExemptionType { get; set; }
    /// <summary>
    /// نام بیمه
    /// </summary>
    [MaxLength(100)]
    public string InsuranceName { get; set; }
    /// <summary>
    /// کشور محل تابعیت - جدول 5
    /// </summary>
    [MaxLength(200)]
    public string CountryOfCitizenship { get; set; }
    /// <summary>
    /// کشور محل زندگی - جدول 5
    /// </summary>
    [MaxLength(200)]
    public string CountryOfResidence { get; set; }
    /// <summary>
    /// کد پستی
    /// </summary>
    [MaxLength(16)]
    public string PostalCode { get; set; }
    /// <summary>
    /// آدرس
    /// </summary>
    [MaxLength(1500)]
    public string Address { get; set; }
    /// <summary>
    /// رسته شغلی
    /// </summary>
    public int Occupation { get; set; }
    /// <summary>
    /// سمت
    /// </summary>
    [MaxLength(128)]
    public string Position { get; set; }

    /// <summary>
    /// نوع استخدام - جدول 7
    /// </summary>
    public int EmployeeType { get; set; }
    /// <summary>
    /// تاریخ شروع به کار
    /// </summary>
    public int StartWorkDate { get; set; }
    /// <summary>
    /// تاریخ پایان کار - درصورت پایان همکاری به دلایل
    /// بازخرید، اخراج و سایر موارد به استثنای بازنشستگی، پر شود
    /// </summary>
    public int EndWorkDate { get; set; }
    /// <summary>
    /// تاریخ بازنشستگی
    /// </summary>
    public int RetiredDate { get; set; }
}