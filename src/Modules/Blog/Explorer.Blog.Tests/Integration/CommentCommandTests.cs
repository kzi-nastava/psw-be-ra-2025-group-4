using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests.Integration
{
    [Collection("Sequential")]
    public class CommentCommandTests : BaseBlogIntegrationTest
    {
        public CommentCommandTests(BlogTestFactory factory) : base(factory) { }

        private static BlogController CreateController(IServiceScope scope, string userId = "1")
        {
            return new BlogController(
                scope.ServiceProvider.GetRequiredService<IBlogService>(),
                scope.ServiceProvider.GetRequiredService<ICommentService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }

        // ================================================================
        // CREATE COMMENT - ONLY ON PUBLISHED BLOG
        // ================================================================

        [Fact]
        public void Creates_comment_on_published_blog()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var dto = new CreateUpdateCommentDto { Text = "Prvi komentar" };

            var result = ((ObjectResult)controller.CreateComment(-2, dto).Result)?.Value as CommentDto;

            result.ShouldNotBeNull();
            result.BlogId.ShouldBe(-2);
            result.UserId.ShouldBe(1);
            result.Text.ShouldBe("Prvi komentar");

            var stored = db.Comments.First(c => c.Id == result.Id);
            stored.BlogId.ShouldBe(-2);
            stored.UserId.ShouldBe(1);
            stored.Text.ShouldBe("Prvi komentar");
        }

        [Fact]
        public void Fails_to_create_comment_on_non_published_blog()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");

            var dto = new CreateUpdateCommentDto { Text = "Ne bi trebalo da uspe" };

            Should.Throw<InvalidOperationException>(() =>
            {
                controller.CreateComment(-1, dto); // Preparation
            });

            Should.Throw<InvalidOperationException>(() =>
            {
                controller.CreateComment(-3, dto); // Archived
            });
        }

        // ================================================================
        // UPDATE COMMENT (AUTHOR + WITHIN 15 MINUTES)
        // ================================================================

        [Fact]
        public void Author_can_edit_comment_within_15_minutes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

            // create comment now (fresh)
            var freshComment = new Comment(-2, 1, "Stari tekst", DateTime.UtcNow.AddMinutes(-10));
            db.Comments.Add(freshComment);
            db.SaveChanges();

            var updateDto = new CreateUpdateCommentDto { Text = "Novi tekst" };
            var updated = ((ObjectResult)controller.UpdateComment(freshComment.Id, updateDto).Result)?.Value as CommentDto;

            updated.ShouldNotBeNull();
            updated.Text.ShouldBe("Novi tekst");
            updated.LastModifiedAt.ShouldNotBeNull();

            var stored = db.Comments.First(c => c.Id == freshComment.Id);
            stored.Text.ShouldBe("Novi tekst");
        }

        [Fact]
        public void Author_cannot_edit_comment_after_15_minutes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

            // create expired comment
            var oldComment = new Comment(-2, 1, "Tekst", DateTime.UtcNow.AddMinutes(-16));
            db.Comments.Add(oldComment);
            db.SaveChanges();

            var updateDto = new CreateUpdateCommentDto { Text = "Novi tekst posle isteka" };

            Should.Throw<InvalidOperationException>(() =>
            {
                controller.UpdateComment(oldComment.Id, updateDto);
            });
        }

        // ================================================================
        // DELETE COMMENT (AUTHOR + WITHIN 15 MINUTES)
        // ================================================================

        [Fact]
        public void Author_cannot_delete_comment_after_15_minutes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var oldComment = new Comment(-2, 1, "Tekst za brisanje", DateTime.UtcNow.AddMinutes(-16));
            db.Comments.Add(oldComment);
            db.SaveChanges();

            Should.Throw<InvalidOperationException>(() =>
            {
                controller.DeleteComment(oldComment.Id);
            });
        }

        // ================================================================
        // OTHER USER CANNOT MODIFY OR DELETE COMMENT
        // ================================================================

        [Fact]
        public void Other_user_cannot_edit_or_delete_comment()
        {
            using var scope = Factory.Services.CreateScope();
            var controllerAuthor = CreateController(scope, "1");
            var controllerOther = CreateController(scope, "2");
            var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var ownComment = new Comment(-2, 1, "Tekst", DateTime.UtcNow);
            db.Comments.Add(ownComment);
            db.SaveChanges();

            var updateDto = new CreateUpdateCommentDto { Text = "Drugi pokušaj" };

            Should.Throw<UnauthorizedAccessException>(() =>
            {
                controllerOther.UpdateComment(ownComment.Id, updateDto);
            });

            Should.Throw<UnauthorizedAccessException>(() =>
            {
                controllerOther.DeleteComment(ownComment.Id);
            });
        }
    }
}
