using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
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

        public QuizController(IQuizSubmissionService quizSubmissionService)
        {
            _quizSubmissionService = quizSubmissionService;
        }

        [HttpPost("submit/{quizId:int}")]
        public ActionResult<QuizResultDto> SubmitAnswers(int quizId, [FromBody] QuizSubmissionDto submission)
        {
            submission.TouristId = User.PersonId();
            var result = _quizSubmissionService.SubmitAnswers(quizId, submission);
            return Ok(result);
        }
    }
}