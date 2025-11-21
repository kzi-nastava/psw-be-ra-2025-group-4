using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class QuizSubmissionService : IQuizSubmissionService
    {
        private readonly IQuizAnswerRepository _answerRepo;
        private readonly IQuizRepository _quizRepo;
        private readonly IMapper _mapper;

        public QuizSubmissionService(IQuizAnswerRepository answerRepo, IQuizRepository quizRepo, IMapper mapper)
        {
            _answerRepo = answerRepo;
            _quizRepo = quizRepo;
            _mapper = mapper;
        }

        public QuizResultDto SubmitAnswers(int quizId, QuizSubmissionDto dto)
        {
            var quiz = ValidateSubmission(quizId, dto);

            var answers = dto.Answers.Select(a => new QuizAnswer(dto.TouristId, a.QuestionId, a.SelectedOptionId)).ToList();
            _answerRepo.Create(answers);

            var questionResults = quiz.Questions.Select(question => BuildQuestionResult(question, dto.Answers)).ToList();

            return new QuizResultDto
            {
                QuizId = quizId,
                QuestionResults = questionResults
            };
        }

        private Quiz ValidateSubmission(int quizId, QuizSubmissionDto dto)
        {
            if (dto.QuizId != quizId)
                throw new ArgumentException($"Quiz ID in route ({quizId}) does not match Quiz ID in request body ({dto.QuizId})");

            var quiz = _quizRepo.GetById(quizId);
            if (quiz == null)
                throw new NotFoundException($"Quiz not found: {quizId}");

            var questionIds = quiz.Questions.Select(q => q.Id).ToHashSet();
            foreach (var answer in dto.Answers)
            {
                if (!questionIds.Contains(answer.QuestionId))
                    throw new ArgumentException($"Question {answer.QuestionId} does not belong to quiz {quizId}");
            }

            return quiz;
        }

        private QuestionResultDto BuildQuestionResult(Question question, List<QuizAnswerDto> submittedAnswers)
        {
            var submittedAnswer = submittedAnswers.FirstOrDefault(a => a.QuestionId == question.Id);
            var selectedOption = submittedAnswer != null
                ? question.Options.FirstOrDefault(o => o.Id == submittedAnswer.SelectedOptionId)
                : null;

            var isCorrect = selectedOption?.IsCorrect ?? false;
            var feedback = selectedOption?.Feedback ?? "No answer submitted";

            if (selectedOption != null && !isCorrect)
            {
                var correctOptions = question.Options.Where(o => o.IsCorrect).ToList();
                if (correctOptions.Count > 1)
                {
                    var correctTexts = string.Join(", ", correctOptions.Select(o => o.Text));
                    feedback += $". Correct answers: {correctTexts}";
                }
            }

            return new QuestionResultDto
            {
                QuestionId = question.Id,
                SelectedOptionId = submittedAnswer?.SelectedOptionId ?? 0,
                IsCorrect = isCorrect,
                Feedback = feedback
            };
        }
    }
}