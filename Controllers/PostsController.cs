using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blogy.Data;
using Blogy.Models;
using Blogy.Data.FileManager;
using Blogy.Models.Comments;
using Blogy.ViewModels;
using Blogy.Data.Repository;

namespace Blogy.Controllers
{
    public class PostsController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;

        public PostsController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;

        }

        // GET: Posts

        public IActionResult Index(int pageNumber, string category, string search)
        {
            if (pageNumber < 1)
                return RedirectToAction("Index", new { pageNumber = 1, category });

            var vm = _repo.GetAllPosts(pageNumber, category, search);

            return View(vm);
        }

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image) => new FileStreamResult(
            _fileManager.ImageStream(image),
            $"image/{image.Substring(image.LastIndexOf('.') + 1)}");
        //{
        //    var mime =
        //    return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        //}

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
                //return Post(vm.PostId);
                return RedirectToAction("Post", new{id = vm.PostId });

            var post = _repo.GetPost(vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                });
                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _repo.AddSubComment(comment);
            }

            await _repo.SaveChangesAsync();
            //return View();
            return RedirectToAction("Post", new { id = vm.PostId });

        }
        // GET: Posts/Details/5
        public IActionResult Post(int id)
        {
            return View(_repo.GetPost(id));
        }


    }
}
