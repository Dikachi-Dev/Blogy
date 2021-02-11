using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Blogy.Data;
using Blogy.Models;
using Blogy.ViewModels;
using Blogy.Data.FileManager;
using Blogy.Data.Repository;

namespace Blogy.Controllers
{
    //(Roles = "Admin")
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;

        public PanelController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }


        public  IActionResult Index()
        {
            var posts = _repo.GetAllPosts();
            return View(posts);
        }


        // GET: Posts/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Posts/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create( PostViewModel vm) //[Bind("Id,Title,Body,Created")]
        //{
        //    var post = new Post
        //    {
        //        Id = vm.Id,
        //        Title = vm.Title,
        //        Body = vm.Body,
        //        Image = await _fileManager.SaveImage(vm.Image),//Handle image
        //        Description = vm.Description,
        //        Category = vm.Category,
        //        Tag = vm.Tag
        //    };
        //    if (ModelState.IsValid)
        //    {

        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(new PostViewModel());
        //}

        // GET: Posts/Edit/5
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new PostViewModel());
            }
            else
            {
            var post = _repo.GetPost((int)id);
            //if (post == null)
            //{
            //    return NotFound();
            //}

            return View(new PostViewModel {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                CurrentImage = post.Image,
                Description = post.Description,
                Category = post.Category,
                Tag = post.Tag

            });
            }

        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( PostViewModel vm)  // [Bind("Id,Title,Body,Created")]
        {
            var post = new Post
            {
                Id = vm.Id,
                Title = vm.Title,
                Body = vm.Body,
                Description = vm.Description,
                Category = vm.Category,
                Tag = vm.Tag,
                //await _fileManager.SaveImage(vm.Image)Handle image
            };

            if (vm.Image == null)
                post.Image = vm.CurrentImage;
            else
            {
                if (!string.IsNullOrEmpty(vm.CurrentImage))
                    _fileManager.RemoveImage(vm.CurrentImage);

                post.Image = await _fileManager.SaveImage(vm.Image);
            }

            if (post.Id > 0)
                _repo.UpdatePost(post);
            else
                _repo.AddPost(post);

            if (await _repo.SaveChangesAsync())
                return RedirectToAction("Index");
            else
                return View(post);




        }




        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {

            _repo.RemovePost(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }



}
