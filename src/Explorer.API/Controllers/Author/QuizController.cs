using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public ActionResult<List<QuizDto>> GetAll()
        {
            return Ok(_quizService.GetAll());
        }

        [HttpGet("{id:long}")]
        public ActionResult<QuizDto> GetById(long id)
        {
            return Ok(_quizService.GetById(id));
        }

        [HttpPost]
        public ActionResult<QuizDto> Create([FromBody] QuizDto dto)
        {
            dto.AuthorId = User.PersonId().ToString();
            return Ok(_quizService.Create(dto));
        }

        [HttpPut("{id:long}")]
        public ActionResult<QuizDto> Update(long id, [FromBody] QuizDto dto)
        {
            dto.Id = id;
            dto.AuthorId = User.PersonId().ToString();
            return Ok(_quizService.Update(dto));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _quizService.Delete(id);
            return Ok();
        }

    }
}
