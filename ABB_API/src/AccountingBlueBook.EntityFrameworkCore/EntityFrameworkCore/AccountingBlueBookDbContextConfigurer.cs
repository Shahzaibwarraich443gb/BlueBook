using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AccountingBlueBook.EntityFrameworkCore
{
    public static class AccountingBlueBookDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AccountingBlueBookDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AccountingBlueBookDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
