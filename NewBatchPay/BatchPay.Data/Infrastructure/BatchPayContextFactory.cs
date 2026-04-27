// Data/BatchPayContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BatchPay.Data.Infrastructure
{
    public class BatchPayContextFactory : IDesignTimeDbContextFactory<BatchPayContext>
    {
        public BatchPayContext CreateDbContext( string[] args )
        {
            var opt = new DbContextOptionsBuilder<BatchPayContext>()
                .UseSqlServer( "Server=DESKTOP-HNI6DDI\\SQLEXPRESS;Database=BatchPay;Trusted_Connection=True;TrustServerCertificate=True" )
                .Options;
            return new BatchPayContext( opt );
        }
    }
}

