using System.Reflection.Metadata.Ecma335;
using BillController.Models;
using BillController.Repository;
using BillController.Repository.Realisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using static BillController.Models.Dto.Bill.BillDto;

namespace BillController.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        IRepository<Bill> _billRepository;
        public BillController(IRepository<Bill> billRepository  )
        {
            _billRepository = billRepository;
        }

        [HttpPost("{accountId:guid}")]
        public async Task<IActionResult> AddBill(Guid accountId, Guid CategoryId, AddBillDto add)
        {
            var acc = _billRepository.Context.CurrentAccounts.Find(accountId);
            if (acc == null)
            {
                ModelState.AddModelError("Not Found", "The Acc is not found");
                return BadRequest(ModelState);
            }

            Bill created = new Bill()
            {
                Amount = add.Amount,
                AccountGuid = accountId,
                CategoryId = CategoryId,
                DueDate = add.dueDate,
                Name = add.Name
            };
            var result = await _billRepository.AddAsync(created);
            if (!result)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetBill), new { id = created.BillId} , created);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBill(Guid id)
        {
            var entitty = await _billRepository.Get(id);
            if (entitty == null)
            {
                return NotFound();
            }
            return Ok(entitty);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _billRepository.Context.Bills.ToListAsync();
            return Ok(list);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var pos = await _billRepository.Delete(id);
            if (pos is true) return NoContent();
            ModelState.AddModelError("Not Found", "The Bill is not found");
            return ValidationProblem(ModelState);
        }

    }
}
