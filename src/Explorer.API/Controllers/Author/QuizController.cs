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

        [HttpGet("{id:int}")]
        public ActionResult<QuizDto> GetById(int id)
        {
            return Ok(_quizService.GetById(id));
        }

        [HttpPost]
        public ActionResult<QuizDto> Create([FromBody] QuizDto dto)
        {
            dto.AuthorId = User.PersonId().ToString();
            return Ok(_quizService.Create(dto));
        }

        [HttpPut("{id:int}")]
        public ActionResult<QuizDto> Update(int id, [FromBody] QuizDto dto)
        {
            dto.Id = id;
            dto.AuthorId = User.PersonId().ToString();
            return Ok(_quizService.Update(dto));
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            _quizService.Delete(id);
            return Ok();
        }

        [HttpPost("submit/{quizId:int}")]
        public ActionResult<QuizDto> SubmitAnswers(int quizId, [FromBody] QuizDto submitted)
        {
            return Ok(_quizService.SubmitAnswers(quizId, submitted));
        }
    }
}
