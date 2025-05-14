using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MvcBlog.Models;
using System.Data.Entity;
using System.Web.UI.WebControls;

namespace MvcBlog.Controllers
{
    public class BlogController : Controller
    {
        private BlogDbContext db = new BlogDbContext();

        public ActionResult Index(string search)
        {
            var blogs = db.BlogPosts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                blogs = blogs.Where(b => b.Title.Contains(search));
                ViewBag.Search = search;
            }

            return View(blogs.ToList());
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlogPost post, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    imageFile.SaveAs(path);
                    post.ImagePath = "/Images/" + fileName;
                }


                db.BlogPosts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }
        // GET: Blog/Edit/5
        public ActionResult Edit(int id)
        {
            var blog = db.BlogPosts.Find(id);
            if (blog == null)
                return HttpNotFound();

            return View(blog);
        }

        // POST: Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BlogPost blog, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingBlog = db.BlogPosts.Find(blog.Id);
                if (existingBlog == null)
                {
                    return HttpNotFound();
                }

                existingBlog.Title = blog.Title;
                existingBlog.Content = blog.Content;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string folderPath = Server.MapPath("~/Images");

                    // Klasör varsa, değilse oluştur
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);
                    imageFile.SaveAs(filePath);

                    // Yeni resim yolunu güncelle
                    existingBlog.ImagePath = "/Images/" + fileName;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }



        // GET: Blog/Delete/5
        public ActionResult Delete(int id)
        {
            var blog = db.BlogPosts.Find(id);
            if (blog == null)
                return HttpNotFound();

            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var blog = db.BlogPosts.Find(id);

            // Resmi de silelim (isteğe bağlı)
            if (!string.IsNullOrEmpty(blog.ImagePath))
            {
                string path = Server.MapPath(blog.ImagePath);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            db.BlogPosts.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int id)
        {
            var blog = db.BlogPosts.Find(id);
            if (blog == null)
                return HttpNotFound();

            return View(blog);
        }

    }

}

