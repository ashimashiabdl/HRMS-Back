using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public partial class Search_Result
    {
        public long EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string IdentityNo { get; set; }
        public string NationalNo { get; set; }
        public Nullable<long> PayLocationId { get; set; }
        public string PayLocation { get; set; }
        public Nullable<long> EmployeeTypeId { get; set; }
        public string EmployeeType { get; set; }
        public Nullable<long> EmployeeStatusId { get; set; }
        public string EmployeeStatus { get; set; }
        public Nullable<long> CostCenterId { get; set; }
        public string CostCenter { get; set; }
        public Nullable<long> WorkPlaceId { get; set; }
        public string WorkPlace { get; set; }
        public Nullable<long> OrganizationUnitId { get; set; }
        public string OrganizationUnit { get; set; }
        public Nullable<bool> IsEmployed { get; set; }
        public Nullable<long> EducationGradeId { get; set; }
        public string EducationGrade { get; set; }
        public Nullable<long> JobId { get; set; }
        public string Job { get; set; }
        public Nullable<long> JobNatureId { get; set; }
        public string JobNature { get; set; }
        public string JobCode { get; set; }
        public Nullable<int> JobDegree { get; set; }
        public Nullable<long> GenderId { get; set; }
        public string Gender { get; set; }   
        
        public Nullable<long> MaritalStatusId { get; set; }
        public string MaritalStatus { get; set; }

        /// <summary>
        /// True when the row is surfaced via a StatusId=14 (LastInPayLocation) order
        /// while the employee's active order (StatusId=9) is in another pay location.
        /// </summary>
        public bool IsEmployedInOtherPayLocation { get; set; }

        public Nullable<long> CurrentPayLocationId { get; set; }

        public string CurrentPayLocation { get; set; }
    }
}
