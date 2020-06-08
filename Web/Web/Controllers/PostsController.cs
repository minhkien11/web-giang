using System;
using System.Collections.Generic;
using System.IO;
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
        [HttpPost("title")]
        public ActionResult<IEnumerable<Post>> GetPost([FromBody] string title)
        {
            var unsignedtitle = Util.ChuyenTiengVietKhongDau(title).ToLower();
            var post = _context.Posts.Where(c => c.Title.Contains(title) || c.UnsignedTitle.Contains(unsignedtitle)).ToList();
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
            var lstpost = _context.Posts.Where(c => c.Type == 1).OrderByDescending(c => c.CreatedDate).Take(5).ToList();
            if (lstpost.Count == 0)
            {
                return NotFound();
            }

            return lstpost;
        }

        // GET: api/Posts/page/1
        [HttpGet("page/{type}/{page}")]
        public ActionResult<PaginationSet<Post>> GetPosts(int page, int  type)
        {
            var lstPost = type == 0 ? _context.Posts : _context.Posts.Where(c => c.Type == type);
            var result = new PaginationSet<Post>()
            {
                Page = page,
                TotalPage = (int)Math.Ceiling((decimal)lstPost.Count() / 6),
                Items = lstPost.Skip((page - 1) * 6).Take(6).ToList(),
            };

            if (result.Items.Count == 0)
            {
                return NotFound();
            }

            return result;
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        public ActionResult PutPost(Post post)
        {
            var oldpost = _context.Posts.Find(post.ID);

            if (oldpost == null)
            {
                return NotFound();
            }
            else
            {
                oldpost.Title = post.Title;
                oldpost.UnsignedTitle = Util.ChuyenTiengVietKhongDau(post.Title).ToLower();
                oldpost.Content = post.Content;
                oldpost.UpdatedDate = DateTime.Now;
                _context.Entry(oldpost).State = EntityState.Modified;
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.ID))
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
            post.UnsignedTitle = Util.ChuyenTiengVietKhongDau(post.Title).ToLower();
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

        [HttpPost("image")]
        public ActionResult UploadImage()
        {
            string[] fileType = { "bmp", "jpg", "jpeg", "png" };

            if (Request.Form.Files.Count == 0)
                return NotFound();

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            List<string> lstName = new List<string>();

            foreach (var fileUpload in Request.Form.Files)
            {
                if (!fileType.Any(c => fileUpload.ContentType.Contains(c)))
                    return Problem(
                        title: "Không thành công",
                        detail: "Định dạng file " + fileUpload.FileName + " không hợp lệ");

                string filePath = Path.Combine(folderPath, fileUpload.FileName);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    fileUpload.CopyTo(fs);
                }

                Image img = new Image();
                img.Name = fileUpload.FileName;
                img.UploadedDate = DateTime.Now;
                _context.Images.Add(img);
                lstName.Add(filePath);
            }
            _context.SaveChanges();

            return Content(string.Join('\n',lstName));
        }
    }
}
