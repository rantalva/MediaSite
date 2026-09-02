using MediaSite_backend.MockData;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace MediaSite_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController(IArticleRepository articleRepository) : ControllerBase
    {
        [HttpGet(Name = "GetArticles")]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            var articles = await articleRepository.GetAllArticlesAsync();

            return Ok(articles);
        }

        [HttpGet("{id:guid}", Name = "GetArticleById")] // use id:guid as constrait!
        public async Task<ActionResult<Article>> GetArticleByGuid(Guid id)
        {
            if (id != null)
            {
                var article = await articleRepository.GetByIdAsync(id);

                if (article == null)
                    return NotFound();

                return Ok(article);

            }
            return BadRequest();
        }

        [HttpGet("{slug}", Name = "GetArticleBySlug")] // using string as constrait is not allowed! Remember that
        public async Task<ActionResult<Article>> GetArticleBySlug(string slug)
        {
            if (slug != null)
            {
                var article = await articleRepository.GetBySlugAsync(slug);

                if (article == null)
                    return NotFound();

                return Ok(article);
            }
            return BadRequest();
        }

        // POST api/<ArticlesController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateArticleDto>> CreateArticle([FromBody] CreateArticleDto articleDto)
        {

            var newArticle = await articleRepository.CreateAsync(articleDto);

            if (newArticle == null)
            {
                return BadRequest();
            }

            return CreatedAtRoute("GetArticles", newArticle);
        }

        // PUT api/<ArticlesController>/5
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EditArticleDto>> EditArticle(Guid id, [FromBody] EditArticleDto editArticleDto)
        {
            var article = await articleRepository.UpdateAsync(id, editArticleDto);

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        // DELETE api/<ArticlesController>/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {

            var deleted = await articleRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
