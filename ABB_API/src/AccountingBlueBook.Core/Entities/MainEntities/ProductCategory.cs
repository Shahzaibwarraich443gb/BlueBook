using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{



    [Table("ProductCategories")]

    public class ProductCategory  : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Nature { get; set; }
        public bool IsActive { get; set; }

        public ProductCategoryEnum ProductCategoryEnum { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }
}
