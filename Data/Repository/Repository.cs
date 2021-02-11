using Blogy.Helpers;
using Blogy.Models;
using Blogy.Models.Comments;
using Blogy.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogy.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly BlogyContext _context;


        public Repository(BlogyContext context )
        {
            _context = context;

        }
        public void AddPost(Post post)
        {
            _context.Add(post);

        }

        public List<Post> GetAllPosts()
        {
            return _context.Posts.ToList();
        }

        public IndexViewModel GetAllPosts(
            int pageNumber,
            string category,
            string search )
        {
            //Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

            //return _context.Posts
            //    .Where(post => InCategory(post))
            //    .ToList();

            //return _context.Posts
            //.Where(post => post.Category.ToLower().Equals(category.ToLower()))
            //.ToList();

            int pageSize = 5;
            int skipAmount = pageSize * (pageNumber - 1);


            var query = _context.Posts.AsNoTracking().AsQueryable();


            if (!String.IsNullOrEmpty(category))
                query = query.Where(post => post.Category.ToLower().Equals(category.ToLower()));

            //query = query.Where(x => InCategory(x));

            if (!String.IsNullOrEmpty(search))
                query = query.Where(post => EF.Functions.Like(post.Title, $"%{search}%")
                                    || EF.Functions.Like(post.Body, $"%{search}%")
                                    || EF.Functions.Like(post.Description, $"%{search}%"));

            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
                Category = category,
                Search = search,
                Posts = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()

            };
        }


        public Post GetPost(int id)
        {
            return _context.Posts
                .Include(p => p.MainComments)
                .ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _context.Posts.Remove(GetPost(id));
        }

        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _context.SubComments.Add(comment);
        }


    }
}
