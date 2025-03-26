using System.Reflection.Metadata.Ecma335;
using BillController.Models;
using BillController.Repository;
using BillController.Repository.Realisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BillController.Models.CategoryDto.BillDto;

namespace BillController.Controllers
{
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
        [Authorize]
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
            var list = _billRepository.Context.Bills;
            return Ok(list);
        }

    }
}
