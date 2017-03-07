using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using nme_blog.Models;
using Microsoft.AspNet.Identity;

namespace nme_blog.Controllers
{
    [Authorize, RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            // Eager Loading is done here!
            // All other properties of the Comments class/object will be used as Lazy Loading later on in the code whenever they are used or needed. 
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Post);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        //[Authorize]
        public ActionResult Create(string Slug)
        {
            if (string.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            TempData["PostData"] = blogPost;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,Body")] Comment comment)
        {
            /*
             * Slug in the URL parameter
             * 
             * PostId, AuthorId will be used for creating a new comment on a specific post
             * */

            var user = db.Users.Find(User.Identity.GetUserId());
            //BlogPost tmpPost = (BlogPost) TempData["PostData"];
            
            if (ModelState.IsValid)
            {
                var Slug = db.Posts.FirstOrDefault(x => x.Id == comment.PostId).Slug;
                comment.PostId = comment.PostId;
                comment.AuthorId = user.Id;
                comment.Created = DateTimeOffset.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                //return RedirectToAction("Details", "BlogPosts", routeValues: new { slug = Slug });
                return Redirect(Request.UrlReferrer.ToString());
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        [Authorize(Roles = "Admin, Moderator")]
        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            //ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            ViewBag.Slug = db.Posts.FirstOrDefault(x => x.Id == comment.PostId).Slug;
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,AuthorId,Created,   Body,UpdateReason")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var Slug = db.Posts.FirstOrDefault(x => x.Id == comment.PostId).Slug;

                /*
                //Keeping the original AuthorId of the comment.
                //AuthorId won't be updated with the User editing the comment. 
                
                var user = db.Users.Find(User.Identity.GetUserId());
                comment.AuthorId = user.Id;
                */
                comment.Updated = DateTimeOffset.Now;

                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "BlogPosts", routeValues: new { slug = Slug });
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        [Authorize(Roles = "Admin, Moderator")]
        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Slug = db.Posts.FirstOrDefault(x => x.Id == comment.PostId).Slug;
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
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
