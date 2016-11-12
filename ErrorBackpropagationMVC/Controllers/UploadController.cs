using System.Web;
using System.Web.Mvc;
using System.IO;

namespace ErrorBackpropagationMVC.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Data = "Upload your data here, please.";
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string name)
        {
            if (name != null && file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Server.MapPath("~/App_Data");

                string newFolder = Path.Combine(path, name);
                if (!Directory.Exists(newFolder))
                    Directory.CreateDirectory(newFolder);

                var newPath = Path.Combine(Server.MapPath("~/App_Data/" + name), fileName);

                file.SaveAs(newPath);
            }
            else
                ViewBag.Error = "File upload error";

            return View();
        }
    }
}