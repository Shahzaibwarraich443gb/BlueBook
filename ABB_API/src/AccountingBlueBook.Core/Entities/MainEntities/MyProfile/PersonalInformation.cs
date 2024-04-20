using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public enum Title
    {
        Mr=1,
        Mrs=2,
        Miss=3,
        Ms=4,
        Mx=5,
        Sir=6,
        Dr=7,
        Lady=8,
        Lord=9,
        Manager=10
    }
    public enum Gender
    {
        Male=1,
        Female=2,
        Others=3
    }
//
    [Table("Employees")]
    public class Employee: Entity<long> ,IMustHaveTenant,IFullAudited
    {

        
        public Title Title { get; set; }
        public string FirstName { get; set; }     
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string Suffix { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public int MobileNumber { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }


        public DateTime? DateofBirth { get; set; }
        public    string EmployeeCode { get; set; }
        public DateTime? HireDate { get; set; }
        public int DefaultSessionTimeout  { get; set; }
        public int TenantId { get; set ; }
        public long? CreatorUserId { get ; set ; }
        public DateTime CreationTime { get  ; set  ; }
        public long? LastModifierUserId { get  ; set  ; }
        public DateTime? LastModificationTime { get  ; set  ; }
        public long? DeleterUserId { get  ; set  ; }
        public DateTime? DeletionTime { get  ; set  ; }
        public bool IsDeleted { get  ; set  ; }
    }
    ///////
}
