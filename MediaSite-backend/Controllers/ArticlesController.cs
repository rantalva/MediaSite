using MediaSite_backend.MockData;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MediaSite_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        [HttpGet(Name = "GetArticles")]
        public ActionResult<IEnumerable<Article>> GetArticles()
        {
            return Ok(ArticlesStore.ArticlesList);
        }

        [HttpGet("{id:guid}", Name = "GetArticleById")] // use id:guid as constrait!
        public ActionResult<Article> GetArticleByGuid(Guid id)
        {
            if (id != null)
            {
                var article = ArticlesStore.ArticlesList
                    .FirstOrDefault(a => a.Id == id);

                if (article == null)
                    return NotFound();

                return Ok(article);

            }
            return BadRequest();
        }

        [HttpGet("{slug}", Name = "GetArticleBySlug")] // using string as constrait is not allowed! Remember that
        public ActionResult<Article> GetArticleBySlug(string slug)
        {
            if (slug != null)
            {
                var article = ArticlesStore.ArticlesList
                    .FirstOrDefault(a => a.Slug == slug);

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
        public ActionResult<ArticleDto> CreateArticle([FromBody] ArticleDto articleDto)
        {
            if (ArticlesStore.ArticlesList.FirstOrDefault(a => a.Title.ToLower() == articleDto.Title.ToLower()) != null)
            {
                ModelState.AddModelError("Error", "Article already exists");
                return BadRequest();
            }
            if (articleDto == null)
            {
                return BadRequest();
            }

            Article newArticle = new() 
            { 
                Title = articleDto.Title,
                Slug = articleDto.GenerateSlugFromTitle(articleDto.Title), // feels stupid needs refactoring
                Content = articleDto.Content,
                HeroImage = articleDto.HeroImage
            };

            ArticlesStore.ArticlesList.Add(newArticle);

            return CreatedAtRoute("GetArticles", newArticle);
        }

        // PUT api/<ArticlesController>/5
        [HttpPut("{id:guid}")]
        public ActionResult<ArticleDto> EditArticle(Guid id, [FromBody] ArticleDto articleDto)
        {
            var article = ArticlesStore.ArticlesList.FirstOrDefault(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            // this is good for now. But if we want to just change one, we should use /patch. that needs to be implemented

            article.Title = articleDto.Title;
            article.Content = articleDto.Content;
            article.HeroImage = articleDto.HeroImage;
            article.LastEditDate = DateTime.UtcNow;

            return Ok(article);
        }

        // DELETE api/<ArticlesController>/5
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteArticle(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var article = ArticlesStore.ArticlesList.FirstOrDefault(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            ArticlesStore.ArticlesList.Remove(article);

            return NoContent();
        }
    }
}
