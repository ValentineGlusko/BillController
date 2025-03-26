namespace BillController.Models.CategoryDto
{
    public class BillDto
    {

        public record AddBillDto(string Name, decimal Amount, DateTime dueDate);
    }
}
