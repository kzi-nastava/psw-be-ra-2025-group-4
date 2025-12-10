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

        [HttpGet("{id:long}")]
        public ActionResult<BlogDto> Get(long id)
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

        [HttpPut("{id:long}")]
        public ActionResult<BlogDto> Update(long id, [FromBody] CreateUpdateBlogDto dto)
        {
            var updated = _blogService.UpdateBlog(id, dto, GetUserId());
            return Ok(updated);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _blogService.DeleteBlog(id, GetUserId());
            return NoContent();
        }

        [HttpPost("{id:long}/publish")]
        public IActionResult Publish(long id)
        {
            _blogService.Publish(id, GetUserId());
            return Ok();
        }

        [HttpPost("{id:long}/archive")]
        public IActionResult Archive(long id)
        {
            _blogService.Archive(id, GetUserId());
            return Ok();
        }
    }
}
