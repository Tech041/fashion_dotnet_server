using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceServer.Entities
{
    
    
        [Index(nameof(VisitorId), nameof(Date), IsUnique = true)] // compound unique index
        [Index(nameof(CreatedAt))] // index for pagination
        public class Visitor
        {
            [Key]
            public Guid Id { get; set; }  // EF needs a primary key


            public string VisitorId { get; set; } = String.Empty;

          
            public DateTime Date { get; set; } // normalized to midnight

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    
}














