using System;
using System.Collections.Generic;
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
        private readonly IMapper _mapper;

        public BlogService(IBlogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public BlogDto CreateBlog(CreateUpdateBlogDto dto, int userId)
        {
            var blog = new BlogPost(dto.Title, dto.Description, userId, dto.Images);
            var created = _repository.Create(blog);
            return _mapper.Map<BlogDto>(created);
        }

        public BlogDto UpdateBlog(int id, CreateUpdateBlogDto dto, int userId)
        {
            var blog = _repository.Get(id);

            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.UserId != userId)
                throw new UnauthorizedAccessException("You cannot modify someone else's blog.");

            blog.Update(dto.Title, dto.Description, dto.Images);
            _repository.Update(blog);

            return _mapper.Map<BlogDto>(blog);
        }

        public BlogDto Get(int id)
        {
            var blog = _repository.Get(id);

            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            return _mapper.Map<BlogDto>(blog);
        }

        public IEnumerable<BlogDto> GetByUser(int userId)
        {
            var blogs = _repository.GetByUser(userId);
            return _mapper.Map<IEnumerable<BlogDto>>(blogs);
        }

        public void DeleteBlog(int id, int userId)
        {
            var blog = _repository.Get(id);

            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (blog.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete someone else's blog.");

            _repository.Delete(id);
        }
    }
}
