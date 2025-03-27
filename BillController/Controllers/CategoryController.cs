using BillController.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection.Metadata.Ecma335;
using BillController.Repository;
using static BillController.Models.Dto.Cat.CategoryDto;
using BillController.Repository.Realisation;

namespace BillController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private   IRepository<Category> _repository;
        public CategoryController(IRepository<Category> repo)
        {
            _repository = repo;
        }

        [HttpPost]
        public async Task<ActionResult> AddCategory(AddCategory add)
        {
            var cat = new Category() { Name = add.CategoryName };
            var res = await _repository.AddAsync(cat);
            if (res) return CreatedAtAction(nameof(GetCategory), new {id = cat.Id}, cat);
            ModelState.AddModelError(nameof(AddCategory), "Impossible to add");
            return ValidationProblem(ModelState);

        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var entity = await _repository.Get(id);
            if (entity == null)
            {
                ModelState.AddModelError(nameof(GetCategory), "The Impossible to find Entity");
                return ValidationProblem(ModelState);
            }
            return Ok(entity);

        }

        [HttpPost("pagin")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int page,int maximumPerPage) 
        {
            var list = _repository.EntitySet 
                .Skip((page - 1) * maximumPerPage)
                .Take(maximumPerPage);
            return  Ok( await list.ToListAsync());   
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var list = await _repository.EntitySet.ToListAsync();
            return Ok(list);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteCategory(Guid id)
        {
            var res = await _repository.Delete(id);
            if (res) return NoContent();
            ModelState.AddModelError(nameof(DeleteCategory), "Impossible to delete");
            return ValidationProblem(ModelState);
        }
    }
}
