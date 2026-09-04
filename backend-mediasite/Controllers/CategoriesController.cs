using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Category
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
    {
        return await _context.Categories.ToListAsync();
    }

    // GET: api/Category/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(System.Guid id)
    {
        var category = await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new GetCategoryWithPostsDto
            {
                Name = c.Name,

                Articles = c.Articles
                    .Select(a => new CategoryArticleDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Slug = a.Slug,
                        HeroImage = a.HeroImage
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
        return Ok(category);
    }

    // PUT: api/Category/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> EditCategory(Guid id, [FromBody] CategoryDto editArticleDto)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category != null) 
        {
            category.Name = editArticleDto.Name;
            await _context.SaveChangesAsync();
            return Ok();
        }

        return BadRequest();
    }

    // POST: api/Category
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory([FromBody] CategoryDto editArticleDto)
    {
        var category = new Category();

        category.Name = editArticleDto.Name;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCategory", new { id = category.Id }, category);
    }

    // DELETE: api/Category/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(System.Guid? id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(System.Guid? id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
