using BillController.Models;
using BillController.Models.Dto.Accounts;
using BillController.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BillController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrentAccountController : ControllerBase
    {
        private IRepository<CurrentAccount> _repository;
        public CurrentAccountController(IRepository<CurrentAccount> repo)
        {
            _repository = repo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAccount(Guid id)
        {
            var ovj = await _repository.Get(id);
            if (ovj == null)
            {
                ModelState.AddModelError("Add", "Impossible to find Account");
                return ValidationProblem(ModelState);
            }

            return Ok(ovj);
        }
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAccount()
        {
            var ovj =  await _repository.EntitySet.Include(e=> e.Bills).ToListAsync();
            if (ovj.Count != 0) return  Ok(ovj);
            ModelState.AddModelError("Add", "Impossible to find Account");
            return ValidationProblem(ModelState);

        }

        [HttpPost("acc")]
        public async Task<IActionResult> Add(CurrentAccountDTO.AddAccount add)
        {
            var curacc = new CurrentAccount()
            {
                Name = add.Name
            };
         await   _repository.AddAsync(curacc);
         return CreatedAtAction(nameof(GetAccount), new { id = curacc.AccountId }, curacc);
         
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _repository.Delete(id);
            if (result) return NoContent();
            ModelState.AddModelError("Delete", "Impossible to delete Account");
            return ValidationProblem(ModelState);
        }
    }
}
