using Microsoft.EntityFrameworkCore;
using trackingAPI.Models;

namespace trackingAPI.Data.Contexts
{
    public class IssueDbContext : DbContext
    {
        public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Issue> Issues { get; set; }
    }
}
