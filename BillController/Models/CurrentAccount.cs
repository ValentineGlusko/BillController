using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BillController.Models
{
    public class CurrentAccount
    {
        public Guid AccountId { get; init; }
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public byte[] Version { get;   set; }
        public ICollection<Bill?> Bills { get; set; } = [];
        private decimal _amount;
        public decimal Amount
        {
            get => Bills?.Sum(b => b.Amount) ?? 0;
            set
            {
                  
                
                _amount = value;
                
            }
        }
    }
}
