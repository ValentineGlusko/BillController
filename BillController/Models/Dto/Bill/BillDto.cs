namespace BillController.Models.Dto.Bill
{
    public class BillDto
    {

        public record AddBillDto(string Name, decimal Amount, DateTime dueDate);
    }
}
