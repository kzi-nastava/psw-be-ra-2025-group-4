using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public BlogService(
            IBlogRepository repository,
            ICommentRepository commentRepository,
            IMapper mapper)
        {
            _repository = repository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        // =========================
        // Popularity (po komentarima)
        // =========================
        private static BlogPopularityDTO CalculatePopularityDto(int commentsCount)
        {
            if (commentsCount > 30) return BlogPopularityDTO.Famous;
            if (commentsCount > 10) return BlogPopularityDTO.Active;
            return BlogPopularityDTO.None;
        }

        private BlogDto MapWithPopularity(BlogPost blog)
        {
            var dto = _mapper.Map<BlogDto>(blog);
            var commentsCount = _commentRepository.CountByBlog(blog.Id);
            dto.Popularity = CalculatePopularityDto(commentsCount);
            return dto;
        }

        private IEnumerable<BlogDto> MapListWithPopularity(IEnumerable<BlogPost> blogs)
        {
            return blogs.Select(MapWithPopularity).ToList();
        }

        // =========================
        // CRUD
        // =========================
        public BlogDto CreateBlog(CreateUpdateBlogDto dto, int userId)
        {
            var blog = new BlogPost(dto.Title, dto.Description, userId, dto.Images);
            var created = _repository.Create(blog);
            return _mapper.Map<BlogDto>(created);
        }

        public BlogDto UpdateBlog(long id, CreateUpdateBlogDto dto, int userId)
        {
            var blog = _repository.Get(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.UserId != userId)
                throw new UnauthorizedAccessException("You cannot modify someone else's blog.");

            blog.Update(dto.Title, dto.Description, dto.Images);
            _repository.Update(blog);

            return MapWithPopularity(blog);
        }

        public BlogDto Get(long id)
        {
            var blog = _repository.Get(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            return MapWithPopularity(blog);
        }

        public BlogDto GetForUser(long id, int userId)
        {
            var blog = _repository.Get(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.Status == BlogStatus.Preparation && blog.UserId != userId)
                throw new UnauthorizedAccessException("You cannot see someone else's blog in draft state.");

            return MapWithPopularity(blog);
        }

        public IEnumerable<BlogDto> GetByUser(int userId)
        {
            var blogs = _repository.GetByUser(userId);
            return MapListWithPopularity(blogs);
        }

        public IEnumerable<BlogDto> GetVisible(int userId)
        {
            var blogs = _repository.GetVisible(userId);
            return MapListWithPopularity(blogs);
        }

        public void DeleteBlog(long id, int userId)
        {
            var blog = _repository.Get(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete someone else's blog.");

            _repository.Delete(id);
        }

        public void Publish(long id, int userId)
        {
            var blog = _repository.Get(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.UserId != userId)
                throw new UnauthorizedAccessException("You cannot publish someone else's blog.");

            blog.Publish();
            _repository.Update(blog);
        }

        public void Archive(long id, int userId)
        {
            var blog = _repository.Get(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.UserId != userId)
                throw new UnauthorizedAccessException("You cannot archive someone else's blog.");

            blog.Archive();
            _repository.Update(blog);
        }

        // =========================
        // Filters
        // =========================
        public IEnumerable<BlogDto> GetActive()
        {
            var blogs = _repository.GetAll();
            var dtos = MapListWithPopularity(blogs);
            return dtos.Where(b => b.Popularity == BlogPopularityDTO.Active);
        }

        public IEnumerable<BlogDto> GetFamous()
        {
            var blogs = _repository.GetAll();
            var dtos = MapListWithPopularity(blogs);
            return dtos.Where(b => b.Popularity == BlogPopularityDTO.Famous);
        }
    }
}
