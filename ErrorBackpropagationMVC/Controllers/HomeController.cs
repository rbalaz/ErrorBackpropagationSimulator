using System.Web.Mvc;

namespace ErrorBackpropagationMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session.Abandon();
            return View();
        }
    }
}