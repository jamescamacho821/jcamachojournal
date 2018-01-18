using jcamacho_journal.Models;
using System.Linq;
using System.Web.Mvc;

namespace jcamacho_journal.Controllers
{
    [RequireHttps]
    public class TopFiveController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TopFive
        //public ActionResult Index()
        //{
        //    return View();
        //}


        public ActionResult CommentCount()
        {

            IQueryable<Post> result = db.Posts.AsQueryable();
            result = result.Where(p => p.Comments.Count > 0);

            return View(result.OrderByDescending(p => p.CreationDate));
        }

        public ActionResult Index( int? Id)
        {

            var blogList = IndexCount();
            return View(blogList);
        }

        public IQueryable<Post> IndexCount()
        {
            IQueryable<Post> result = db.Posts.AsQueryable();
            result = result.Where(p => p.Comments.Count > 0);
                      
            return result.OrderByDescending(p => p.Comments.Count);
        }
    }
}