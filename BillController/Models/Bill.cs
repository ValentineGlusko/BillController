using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text.Json.Serialization;

namespace BillController.Models
{
    public class Bill
    {
        public Guid BillId { get; init; }
        [MaxLength(500)]
        public Guid CategoryId { get; init; }
        public Guid AccountGuid { get; init; }

        public string Name {get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PayedDate { get; init; }
        public bool IsPayed { get; set; } = false;
        public Category Category { get; init; }

        [JsonIgnore]
        public byte[] Version { get;   set; }
        public CurrentAccount CurrentAccount { get; set; }
        
    }
     
}
