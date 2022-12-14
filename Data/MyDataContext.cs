using Microsoft.EntityFrameworkCore;
using SampleMinimalAPI.Model;

namespace SampleMinimalAPI.Data
{
    public class MyDataContext:DbContext
    {
        public MyDataContext(DbContextOptions<MyDataContext> options)
       : base(options) { }

        public DbSet<Student> Students => Set<Student>();
    }
}
