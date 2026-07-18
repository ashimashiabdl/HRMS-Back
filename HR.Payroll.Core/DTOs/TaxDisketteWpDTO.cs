using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs
{
    public class TaxDisketteWpDTO : BaseDTO
    {
        public long TaxDisketteId { get; set; }
        public string? TaxDiskette { get; set; }
        public long EmployeeId { get; set; }
        public long FicheId { get; set; }

        public int Nationality { get; set; }
        public string? NationalNo { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }
        public int BirthDate { get; set; }
        public int IdentityNo { get; set; }
        public string? BirthPlace { get; set; }
        public string? EducationGrade { get; set; }

        public int InsuranceTypeId { get; set; }
        [StringLength(10)]
        public string? InsuranceNo { get; set; }
        [StringLength(10)]
        public string? ExemptionType { get; set; }
        public string? InsuranceName { get; set; }
        public string? CountryOfCitizenship { get; set; }
        public string? CountryOfResidence { get; set; }
        public string? PostalCode { get; set; }
        public string? Address { get; set; }

        public int Occupation { get; set; }
        public string? Position { get; set; }

        public int EmployeeType { get; set; }
        public int StartWorkDate { get; set; }
        public int EndWorkDate { get; set; }
        public int RetiredDate { get; set; }
    }
}


