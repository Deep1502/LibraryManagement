using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        [Column(TypeName = "date")]
        public DateTime? IssueDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ReturnDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DueDate { get; set; }
    }
}
