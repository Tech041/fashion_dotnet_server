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
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public string Id { get; set; } = Guid.NewGuid().ToString(); // EF needs a primary key


            public string VisitorId { get; set; } = String.Empty;

          
            public DateTime Date { get; set; } // normalized to midnight

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    
}














