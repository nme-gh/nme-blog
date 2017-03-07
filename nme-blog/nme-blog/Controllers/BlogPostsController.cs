using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using nme_blog.Models;
using nme_blog.Helpers;
using System.IO;
using System.Text;
using nme_blog.ExtensionMethods;
using PagedList;
using System.Linq.Expressions;
//using PagedList.Mvc;

namespace nme_blog.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts Style Demo without the Layout
        public ActionResult StyleDemo()
        {
            return View();
        }

        // GET: BlogPosts without any style and without any action restrictions as the required role is Admin. 
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult BlogAdmin()
        {
            // NME: Reverse the blog posts in the order of descending inside the Controller
            return View(Enumerable.Reverse( db.Posts.ToList() ) );
            // Reverse the blog posts in the order of descending inside the View instead of the Controller as this: @Model.Reverse()
        }

        // MODIFY
        public ActionResult Index(int? page, string searchStr)
        {
            int pageSize = 3;               // max number of blog posts to contain on each page
            int pageNumber = (page ?? 1);   // if no page was specified in the querystring, default to the first page (1)

            if (searchStr == null)
            {
                var posts = new List<BlogPost>(); // representing an unknown number of blog posts

                foreach (var postItem in db.Posts.ToList())
                {
                    // Let's set a bodyAbstract instead of Body for long posts to show in the View
                    if (!String.IsNullOrEmpty(postItem.Body) && postItem.Body.Length > 74)
                    {
                        StringBuilder sb = new StringBuilder(postItem.Body.Substring(0, 75));
                        sb = sb.GetPartialString(' ', false, true);
                        sb.Append(" ...");
                        postItem.Body = sb.ToString();
                    }
                    posts.Add(postItem);
                }

                //return View(posts);
                //return View(posts.AsQueryable().OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize));
                var currentPagePosts = posts.AsQueryable().OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize); // will only contain 3 posts max because of the pageSize
                ViewBag.currentPageData = currentPagePosts;
                return View(currentPagePosts);
            }
            else
            {
                ViewBag.UserSearch = searchStr; // Keep the seach string that was used by the user, so the paging doesn't lose the data set when the search string is used.

                var listPosts = db.Posts.AsQueryable();
                listPosts = listPosts.Where(p => p.Body.Contains(searchStr) ||
                                            p.Title.Contains(searchStr) ||
                                            p.Slug.Contains(searchStr) ||
                                            p.Comments.Any(c => c.Body.Contains(searchStr) ||
                                                            c.Author.DisplayName.Contains(searchStr) ||
                                                            c.Author.FirstName.Contains(searchStr) ||
                                                            c.Author.LastName.Contains(searchStr)
                                                            )
                                            );

                return View(listPosts.OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize));
            }
        }


        //// GET: BlogPosts
        //public ActionResult Index(int? page)
        //{
        //    var posts = new List<BlogPost>(); // representing an unknown number of blog posts

        //    foreach (var postItem in db.Posts.ToList())
        //    {
        //        // Let's set a bodyAbstract instead of Body for long posts to show in the View
        //        if (!String.IsNullOrEmpty(postItem.Body) && postItem.Body.Length > 74)
        //        {
        //            StringBuilder sb = new StringBuilder(postItem.Body.Substring(0, 75));
        //            sb = sb.GetPartialString(' ', false, true);
        //            sb.Append(" ...");
        //            postItem.Body = sb.ToString();
        //        }
        //        posts.Add(postItem);
        //    }

        //    int pageSize = 3;               // max number of blog posts to contain on each page
        //    int pageNumber = (page ?? 1);   // if no page was specified in the querystring, default to the first page (1)

        //    ////return View(db.Posts.ToList());
        //    ////return View(db.Posts.AsQueryable().ToPagedList(pageNumber, pageSize));
        //    //return View(posts);
        //    //return View(posts.AsQueryable().OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize));

        //    var currentPagePosts = posts.AsQueryable().OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize); // will only contain 3 posts max because of the pageSize
        //    ViewBag.currentPageData = currentPagePosts;
        //    return View(currentPagePosts);

        //}

        //NME: Change the Index() from POST to GET and change it from ActionResult to IQueryable<BlogPost>
        //NME: public IQueryable <BlogPost> IndexSearch(string searchStr)

        //[HttpPost]
        //public ActionResult Index(int? page, string searchStr)
        //{
        //    //var listPosts = db.Posts.Where(p => p.Body.Contains(searchStr))
        //    //    .Union(db.Posts.Where(p => p.Title.Contains(searchStr)))
        //    //    .Union(db.Posts.Where(p => p.Slug.Contains(searchStr)))
        //    //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Body.Contains(searchStr))))
        //    //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.DisplayName.Contains(searchStr))))
        //    //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.FirstName.Contains(searchStr))))
        //    //    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.LastName.Contains(searchStr))))
        //    //    .AsQueryable(); // Eager Loading with AsQueryable()

        //    ViewBag.searchStr = searchStr;

        //    var listPosts = db.Posts.AsQueryable();
        //    listPosts = listPosts.Where(p => p.Body.Contains(searchStr) ||
        //                                p.Title.Contains(searchStr) ||
        //                                p.Slug.Contains(searchStr) ||
        //                                p.Comments.Any(c => c.Body.Contains(searchStr) || 
        //                                                c.Author.DisplayName.Contains(searchStr) ||
        //                                                c.Author.FirstName.Contains(searchStr) ||
        //                                                c.Author.LastName.Contains(searchStr)
        //                                                )
        //                                );

        //    //Expression<Func<BlogPost, bool>> myPredicate = null;
        //    //myPredicate = p => p.Body.Contains(searchStr) ||
        //    //                            p.Title.Contains(searchStr) ||
        //    //                            p.Slug.Contains(searchStr) ||
        //    //                            p.Comments.Any(c => c.Body.Contains(searchStr) ||
        //    //                                            c.Author.DisplayName.Contains(searchStr) ||
        //    //                                            c.Author.FirstName.Contains(searchStr) ||
        //    //                                            c.Author.LastName.Contains(searchStr)
        //    //                                            );
        //    //listPosts = listPosts.Where(myPredicate);

        //    int pageSize = 3; // display three blog posts at a time on this page
        //    int pageNumber = (page ?? 1);

        //    ////return View(db.Posts.ToList());
        //    ////return View(db.Posts.AsQueryable().ToPagedList(pageNumber, pageSize));
        //    //return View(posts);
        //    return View(listPosts.OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize));
        //}


        // GET: BlogPosts/Details/5
        // GET: Blog/{slug}
        public ActionResult Details(string Slug)
        {
            // Url.LocalPath: Get the relative/partial url for internal redirection. 
            // This way, there should be no issues when running your website either locally (https://localhost:<port>/Blog/{slug}) or published (http://<>.azurewebsites.net/Blog/{slug}).
            //
            // Url.AbsoluteUri: Don't use the fully-qualified url since it would be vulnerable for Open Redirection Attacks.
            ViewBag.ReturnUrl = Request.Url.LocalPath;

            if (string.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Body")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                string fileName = Validators.GetPathToImage(image);
                if (!String.IsNullOrEmpty(fileName))
                {
                    blogPost.mediaURL = "~/Uploads/" + fileName;
                }

                // Adding slug now
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }

                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.Now;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        // GET: BlogEdit/{slug}
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string Slug)
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
            return View(blogPost);
        }

        //TODO:ADD HIDDEN FIELDS


        // POST: BlogPosts/Edit/5
        // PSOT: BlogEdit/{slug}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Slug,mediaURL,  Title,Body,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string fileName = Validators.GetPathToImage(image);
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        blogPost.mediaURL = "~/Uploads/" + fileName;
                    }
                }

                var currentTitle = db.Posts.AsNoTracking().FirstOrDefault(x => x.Id == blogPost.Id).Title;
                if (currentTitle != blogPost.Title)
                {
                    // Title is being modified, update the Slug accordingly
                    var Slug = StringUtilities.URLFriendly(blogPost.Title);
                    if (String.IsNullOrWhiteSpace(Slug))
                    {
                        ModelState.AddModelError("Title", "Invalid title");
                        return View(blogPost);
                    }
                    if (db.Posts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);
                    }

                    blogPost.Slug = Slug;
                }

                blogPost.Updated = DateTimeOffset.Now;

                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        // GET: BlogDel/{slug}
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string Slug)
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
            return View(blogPost);
        }

        //[Authorize Roles="Admin, Moderator"]
        // POST: BlogPosts/Delete/5
        // POST: BlogDel/{slug}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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
