using System;
using System.Collections.Generic;
using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentRepository commentRepository,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public IEnumerable<CommentDto> GetByBlog(long blogId)
        {
            var comments = _commentRepository.GetByBlog(blogId);
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public CommentDto Create(long blogId, int userId, CreateUpdateCommentDto dto)
        {
            var blog = _blogRepository.Get(blogId);

            if (blog.Status != BlogStatus.Published)
            {
                throw new InvalidOperationException("Comments can be added only to published blogs.");
            }

            var comment = new Comment(blogId, userId, dto.Text);
            var created = _commentRepository.Create(comment);
            return _mapper.Map<CommentDto>(created);
        }

        public CommentDto Update(long id, int userId, CreateUpdateCommentDto dto)
        {
            var comment = _commentRepository.Get(id);

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You can only edit your own comments.");

            comment.UpdateText(dto.Text, DateTime.UtcNow);
            _commentRepository.Update(comment);

            return _mapper.Map<CommentDto>(comment);
        }

        public void Delete(long id, int userId)
        {
            var comment = _commentRepository.Get(id);

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own comments.");

            comment.EnsureCanBeDeleted(DateTime.UtcNow);
            _commentRepository.Delete(id);
        }
    }
}
