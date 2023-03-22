using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options)
        {
        }

        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Tag> tags { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<Address> addresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
        }
    }
}