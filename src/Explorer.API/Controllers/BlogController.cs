using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize(Roles = "tourist,author")]
    [Route("api/blogs")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        private int GetUserId()
        {
            var id = User.FindFirst("id")?.Value;

            if (id != null) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;

            return int.Parse(pid ?? throw new Exception("No user id found"));
        }

        [HttpGet("mine")]
        public ActionResult<IEnumerable<BlogDto>> GetMine()
        {
            var result = _blogService.GetByUser(GetUserId());
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<BlogDto> Get(int id)
        {
            var result = _blogService.Get(id);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<BlogDto> Create([FromBody] CreateUpdateBlogDto dto)
        {
            var created = _blogService.CreateBlog(dto, GetUserId());
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public ActionResult<BlogDto> Update(int id, [FromBody] CreateUpdateBlogDto dto)
        {
            var updated = _blogService.UpdateBlog(id, dto, GetUserId());
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _blogService.DeleteBlog(id, GetUserId());
            return NoContent();
        }

        [HttpPost("{id:int}/publish")]
        public IActionResult Publish(int id)
        {
            _blogService.Publish(id, GetUserId());
            return Ok();
        }

        [HttpPost("{id:int}/archive")]
        public IActionResult Archive(int id)
        {
            _blogService.Archive(id, GetUserId());
            return Ok();
        }

    }
}
