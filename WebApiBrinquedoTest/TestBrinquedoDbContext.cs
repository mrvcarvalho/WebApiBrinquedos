using Microsoft.EntityFrameworkCore;
using WebApiBrinquedos.Context;

namespace WebApiBrinquedoTest
{
    public class TestBrinquedoDbContext : BrinquedoDbContext
    {
        public TestBrinquedoDbContext(DbContextOptions<BrinquedoDbContext> options) : base(options)
        {
        }

        public static TestBrinquedoDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BrinquedoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TestBrinquedoDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
