﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly WebDbContext _context;

        public PostsController(WebDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetPosts()
        {
            return _context.Posts.ToList();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public ActionResult<Post> GetPost(int id)
        {
            var post = _context.Posts.Find(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // GET: api/Posts/title
        [HttpGet("{title}")]
        public ActionResult<IEnumerable<Post>> GetPost(string title)
        {
            var post = _context.Posts.Where(c => c.Title.Contains(title)).ToList();
            if (post.Count == 0)
            {
                return NotFound();
            }

            return post;
        }

        // GET: api/Posts/Latest
        [HttpGet("Latest")]
        public ActionResult<IEnumerable<Post>> GetPost()
        {
            var lstpost = _context.Posts.OrderByDescending(c => c.CreatedDate).Take(5).ToList();
            if (lstpost.Count == 0)
            {
                return NotFound();
            }

            return lstpost;
        }

        // GET: api/Posts/page/1
        [HttpGet("page/{page}")]
        public ActionResult<IEnumerable<Post>> GetPosts(int page)
        {
            var lstpost = _context.Posts.Skip((page - 1) * 10).Take(10).ToList();
            if (lstpost.Count == 0)
            {
                return NotFound();
            }

            return lstpost;
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        public ActionResult PutPost(int id, Post post)
        {
            var oldpost = _context.Posts.Find(id);

            if (oldpost == null)
            {
                return NotFound();
            }
            else
            {
                oldpost.Title = post.Title;
                oldpost.Content = post.Content;
                oldpost.UpdatedDate = DateTime.Now;
                _context.Entry(oldpost).State = EntityState.Modified;
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetPost", new { id = oldpost.ID }, oldpost);
            }

        }

        // POST: api/Posts
        [HttpPost]
        public ActionResult<Post> PostPost(Post post)
        {
            post.CreatedDate = DateTime.Now;

            _context.Posts.Add(post);
            _context.SaveChanges();

            return CreatedAtAction("GetPost", new { id = post.ID }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public ActionResult<Post> DeletePost(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return post;
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.ID == id);
        }
    }
}
