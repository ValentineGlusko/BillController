using System.ComponentModel.DataAnnotations;

namespace BillController.Models
{
    public class Category
    {
        [MaxLength(200)]
        public string Name { get; set; } = String.Empty;
        public Guid Id { get; init; }
        public ICollection<Bill> Bills { get; set; } = [];
    }
}
