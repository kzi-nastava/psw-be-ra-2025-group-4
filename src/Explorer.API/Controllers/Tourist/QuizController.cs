using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizSubmissionService _quizSubmissionService;
        private readonly IQuizService _quizService;

        public QuizController(IQuizSubmissionService quizSubmissionService, IQuizService quizService)
        {
            _quizSubmissionService = quizSubmissionService;
            _quizService = quizService;
        }

        [HttpPost("submit/{quizId:int}")]
        public ActionResult<QuizResultDto> SubmitAnswers(int quizId, [FromBody] QuizSubmissionDto submission)
        {
            submission.TouristId = User.PersonId();
            var result = _quizSubmissionService.SubmitAnswers(quizId, submission);
            return Ok(result);
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
    }
}