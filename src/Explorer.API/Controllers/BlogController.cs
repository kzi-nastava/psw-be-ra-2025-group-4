using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Explorer.API.Controllers
{
    [Authorize(Roles = "tourist,author")]
    [Route("api/blogs")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ICommentService _commentService;
        private readonly IWebHostEnvironment _env;

        public BlogController(IBlogService blogService, ICommentService commentService, IWebHostEnvironment env)
        {
            _blogService = blogService;
            _commentService = commentService;
            _env = env;
        }


        private int GetUserId()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            return int.Parse(pid ?? throw new Exception("No user id found"));
        }

      
        [HttpGet]
        public ActionResult<IEnumerable<BlogDto>> GetVisible()
        {
            var result = _blogService.GetVisible(GetUserId());
            return Ok(result);
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
            var result = _blogService.GetForUser(id, GetUserId());
            return Ok(result);
        }

        // Create
        [HttpPost]
        public ActionResult<BlogDto> Create([FromBody] CreateUpdateBlogDto dto)
        {
            if (dto.ImagesBase64 != null && dto.ImagesBase64.Count > 0)
            {
                dto.Images ??= new List<string>();

                foreach (var img in dto.ImagesBase64.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    var fileName = SaveImageFromBase64(img);
                    dto.Images.Add($"/BlogImages/{fileName}"); // preporuka da u DB čuvaš putanju
                }

                dto.ImagesBase64 = null;
            }

            var created = _blogService.CreateBlog(dto, GetUserId());
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }


        // Update
        [HttpPut("{id:long}")]
        public ActionResult<BlogDto> Update(long id, [FromBody] CreateUpdateBlogDto dto)
        {
            if (dto.ImagesBase64 != null && dto.ImagesBase64.Count > 0)
            {
                dto.Images ??= new List<string>();

                foreach (var img in dto.ImagesBase64.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    var fileName = SaveImageFromBase64(img);
                    dto.Images.Add($"/BlogImages/{fileName}");
                }

                dto.ImagesBase64 = null;
            }

            var updated = _blogService.UpdateBlog(id, dto, GetUserId());
            return Ok(updated);
        }


        // Delete
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _blogService.DeleteBlog(id, GetUserId());
            return NoContent();
        }

        // Publish
        [HttpPost("{id:long}/publish")]
        public IActionResult Publish(long id)
        {
            _blogService.Publish(id, GetUserId());
            return Ok();
        }

        // Archive
        [HttpPost("{id:long}/archive")]
        public IActionResult Archive(long id)
        {
            _blogService.Archive(id, GetUserId());
            return Ok();
        }

        [HttpGet("active")]
        public ActionResult<IEnumerable<BlogDto>> GetActive()
    => Ok(_blogService.GetActive());

        [HttpGet("famous")]
        public ActionResult<IEnumerable<BlogDto>> GetFamous()
            => Ok(_blogService.GetFamous());

        // ======================
        //       COMMENTS
        // ======================

        // svi komentari za blog
        [HttpGet("{id:long}/comments")]
        public ActionResult<IEnumerable<CommentDto>> GetComments(long id)
        {
            var result = _commentService.GetByBlog(id);
            return Ok(result);
        }

        // kreiranje komentara (samo ako je blog objavljen)
        [HttpPost("{id:long}/comments")]
        public ActionResult<CommentDto> CreateComment(long id, [FromBody] CreateUpdateCommentDto dto)
        {
            var created = _commentService.Create(id, GetUserId(), dto);
            return CreatedAtAction(nameof(GetComments), new { id }, created);
        }

        // izmena komentara (autor + 15 minuta)
        // 3) Izmena komentara (autor + 15 minuta)
        [HttpPut("comments/{commentId:long}")]
        public ActionResult<CommentDto> UpdateComment(long commentId, [FromBody] CreateUpdateCommentDto dto)
        {
            var updated = _commentService.Update(commentId, GetUserId(), dto);
            return Ok(updated);
        }

        // 4) Brisanje komentara (autor + 15 minuta)
        [HttpDelete("comments/{commentId:long}")]
        public IActionResult DeleteComment(long commentId)
        {
            _commentService.Delete(commentId, GetUserId());
            return NoContent();
        }

        private string SaveImageFromBase64(string base64)
        {
            var commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0) base64 = base64[(commaIndex + 1)..];

            var bytes = Convert.FromBase64String(base64);

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var folder = Path.Combine(webRoot, "BlogImages");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}.jpg"; // ili detektuj ext ako želiš
            var fullPath = Path.Combine(folder, fileName);

            System.IO.File.WriteAllBytes(fullPath, bytes);

            return fileName; // ili vrati "/BlogImages/" + fileName
        }

    }
}
