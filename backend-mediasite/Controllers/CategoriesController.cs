using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories.CategoryRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICategoryRepository _categoryRepository;
    public CategoriesController(ApplicationDbContext context, ICategoryRepository categoryRepository)
    {
        _context = context;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCategoryWithPostsDto>>> GetCategory()
    {
        var categories = await _categoryRepository.GetCategoriesWithArticlesAsync();

        if (categories == null) 
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpGet("{id:guid}", Name = "GetCategoriesById")]
    public async Task<ActionResult<Category>> GetCategory(Guid id)
    {
        var category = await _categoryRepository.GetCategoryWithArticlesByIdAsync(id);

        if (category == null) 
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet("{categoryName}", Name = "GetCategoriesByName")]
    public async Task<ActionResult<Category>> GetCategoryByName(string categoryName)
    {
        var category = await _categoryRepository.GetCategoryWithArticlesByNameAsync(categoryName);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> EditCategory(Guid id, [FromBody] CategoryDto editArticleDto)
    {
        var category = await _categoryRepository.EditCategoryAsync(id, editArticleDto);

        if (category == null)
        {
            return BadRequest();
        }

        return Ok(category);

    }

    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[Authorize]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> PostCategory([FromBody] CategoryDto editArticleDto)
    {
        var category = await _categoryRepository.CreateCategoryAsync(editArticleDto);

        if (category == null)
        {
            return BadRequest();
        }

        return CreatedAtAction("GetCategory", new { id = category.Id }, category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var deleted = await _categoryRepository.DeleteCategoryAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
