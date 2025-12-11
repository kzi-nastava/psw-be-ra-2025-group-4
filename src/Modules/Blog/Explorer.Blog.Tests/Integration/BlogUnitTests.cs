using Explorer.Blog.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Explorer.Blog.Tests.Integration
{
    [Collection("Sequential")]
    public class BlogUnitTests : BaseBlogIntegrationTest
    {
        public BlogUnitTests(BlogTestFactory factory) : base(factory) { }

        [Theory]
        [InlineData(-1, true, BlogStatus.Published)]
        [InlineData(-2, false, BlogStatus.Published)]
        [InlineData(-3, false, BlogStatus.Archived)]
        public void Publishes(long blogId, bool shouldPublish, BlogStatus expectedStatus)
        {
            var blog = GetTestBlog(blogId);

            if (shouldPublish)
            {
                Should.NotThrow(() => blog.Publish());
                blog.Status.ShouldBe(expectedStatus);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() => blog.Publish());
                blog.Status.ShouldBe(expectedStatus);
            }
        }

        [Theory]
        [InlineData(-1, "New Title", "New Desc")]
        [InlineData(-4, "A", "B")]
        public void Updates_preparation_blog(long id, string newTitle, string newDesc)
        {
            var blog = GetTestBlog(id);

            Should.NotThrow(() => blog.Update(newTitle, newDesc, new List<string> { "img.png" }));

            blog.Title.ShouldBe(newTitle);
            blog.Description.ShouldBe(newDesc);
            blog.LastUpdatedAt.ShouldBeNull();
        }

        [Theory]
        [InlineData(-2, "Ignored", "Updated desc")]
        public void Updates_published_blog_description_only(long id, string newTitle, string newDesc)
        {
            var blog = GetTestBlog(id);

            var oldTitle = blog.Title;
            var oldImages = blog.Images;

            Should.NotThrow(() => blog.Update(newTitle, newDesc, new List<string> { "ignored.png" }));

            blog.Title.ShouldBe(oldTitle);
            blog.Images.ShouldBe(oldImages);
            blog.Description.ShouldBe(newDesc);
            blog.LastUpdatedAt.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(-3)]
        public void Fails_updating_archived_blog(long id)
        {
            var blog = GetTestBlog(id);

            Should.Throw<InvalidOperationException>(() =>
                blog.Update("X", "Y", new List<string>())
            );
        }

        [Theory]
        [InlineData(-2, true)]
        [InlineData(-1, false)]
        [InlineData(-3, false)]
        public void Archives(long id, bool shouldArchive)
        {
            var blog = GetTestBlog(id);

            if (shouldArchive)
            {
                Should.NotThrow(() => blog.Archive());
                blog.Status.ShouldBe(BlogStatus.Archived);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() => blog.Archive());
            }
        }

        private static BlogPost GetTestBlog(long id)
        {
            BlogPost blog = id switch
            {
                -1 => new BlogPost("Test1", "Desc1", 1, new List<string>()),
                -2 => new BlogPost("Test2", "Desc2", 1, new List<string>()),
                -3 => new BlogPost("Test3", "Desc3", 1, new List<string>()),
                -4 => new BlogPost("Test4", "Desc4", 1, new List<string>()),
                -5 => new BlogPost("Test5", "Desc5", 2, new List<string>()),
                _ => throw new ArgumentException("Unknown test blog ID")
            };

            if (id == -2)
            {
                blog.Publish();
            }
            else if (id == -3)
            {
                blog.Publish();
                blog.Archive();
            }

            return blog;
        }
    }
}
