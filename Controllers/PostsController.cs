using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jcamacho_journal.Models;
using jcamacho_journal.Helper;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace jcamacho_journal.Controllers
{
    [RequireHttps]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public ActionResult Index()
        //{
        //    return View(db.Posts.ToList().OrderByDescending(p => p.Comments.Count()));
        //}


        // GET: Posts
        public ActionResult Index(int? page, int? Id, string searchStr)
        {

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            return View(blogList.ToPagedList(pageNumber,pageSize)); 
        }

        public IQueryable<Post> IndexSearch(string searchStr)
        {
            IQueryable<Post> result = null;
            if (searchStr != null)
            {
                result = db.Posts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) || 
                         p.Body.Contains(searchStr) || 
                         p.Comments.Any(c => c.Body.Contains(searchStr) ||
                         c.Author.FirstName.Contains(searchStr) || 
                         c.Author.LastName.Contains(searchStr) || 
                         c.Author.DisplayName.Contains(searchStr) || 
                         c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.Posts.AsQueryable();
            }

            return result.OrderByDescending(p => p.CreationDate);
        }


        public IQueryable<Post> IndexCount()
        {
            IQueryable<Post> result = null;

            result = db.Posts.AsQueryable(); // p represents post
            result = result.Where(p => p.Comments.Count > 0);

            return result.OrderByDescending(p => p.CreationDate);
        }




        // GET: Posts/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = db.Posts.Find(id);
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(post);
        //}

        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL,IsDeleted")] Post post, HttpPostedFileBase image)
        {
            if (image!=null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && 
                    ext != ".gif" && ext != ".bmp")
                {
                    ModelState.AddModelError("image", "Invalid Format");
                }
            }
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var filePath = "/Uploads/";
                    var absPath = Server.MapPath("~"+filePath);
                    post.MediaURL = filePath + image.FileName;

                }
                var Slug = StringUtilities.URLFriendly(post.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(post);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(post);
                }

                post.Slug = Slug;
                post.CreationDate = DateTimeOffset.Now;
                post.UpdatedDate = DateTimeOffset.Now;
                post.IsDeleted = false;
                
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UpdatedDate,Title,Body,IsDeleted")] Post post, HttpPostedFileBase image)
        {
            var sMediaURL = Request["MediaURL"].ToString();
            var sSlug = Request["Slug"].ToString();

            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" &&
                    ext != ".gif" && ext != ".bmp")
                {
                    ModelState.AddModelError("image", "Invalid Format");
                }
            }

            if (image != null)
            {
                var filePath = "/Uploads/";
                var absPath = Server.MapPath("~" + filePath);
                post.MediaURL = filePath + image.FileName;

            }
            post.UpdatedDate = DateTimeOffset.Now;
            post.Slug = sSlug;
            post.MediaURL = sMediaURL;
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult CommentCreate([Bind(Include = "Id,PostId,Body,CreationDate,AuthorId")] Comment comment)
        { // only pass in the bind the attributes that have forms
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(comment.Body))
                {
                    comment.CreationDate = DateTime.Now;
                    comment.AuthorId = User.Identity.GetUserId();
                    db.Comments.Add(comment);
                    db.SaveChanges();

                    var post = db.Posts.Find(comment.PostId);
                    return RedirectToAction("Details", new { Slug = post.Slug });
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
