using Abp.Domain.Entities;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class TenureForm:Entity<long>
    {
        public string Name { get; set; }
    }
}
