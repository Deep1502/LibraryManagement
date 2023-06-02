using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Issue> Issues { get; set; } = null!;
    }
}
