using jcamacho_journal.Models;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data;
using System.Linq;
using jcamacho_journal.Helper;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace jcamacho_journal.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index(int? page, int? Id, string searchStr)
        {

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            return View(blogList.ToPagedList(pageNumber, pageSize));
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(Email model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    var body = "<p>Email From: <b>{0}</b>({1})</p><p>Message:</p><p>{2}</p>";
                    model.Body = "This is a message from your personal site. The name and the email of the contacting person is above";
                    var email = new MailMessage(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["emailfrom"])
                    {
                        Subject = "Contact the author.",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    //return RedirectToAction("EmailConfirmation");
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }

            }

            return View(model);


        }
    }
}