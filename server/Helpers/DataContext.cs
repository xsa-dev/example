using Microsoft.EntityFrameworkCore;
using WebApi.Core.Domain.Entities;

namespace WebApi.Helpers {
    public class DataContext : DbContext {

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DataContext (DbContextOptions<DataContext> options) : base (options) {
            
        }
    }
}