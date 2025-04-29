using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Developer_Toolbox.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _context.Categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new {
                    c.Id,
                    c.CategoryName,
                    c.Logo,
                    c.UserId
                });

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            return Ok(new
            {
                category.Id,
                category.CategoryName,
                category.Logo,
                category.UserId
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category cat)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Categories.Add(cat);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = cat.Id }, cat);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Category updatedCat)
        {
            var existing = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            existing.CategoryName = updatedCat.CategoryName;
            existing.Logo = updatedCat.Logo;
            existing.UserId = updatedCat.UserId;

            _context.SaveChanges();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (cat == null)
                return NotFound();

            _context.Categories.Remove(cat);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
