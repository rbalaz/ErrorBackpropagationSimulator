using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;

namespace ErrorBackpropagationMVC.Controllers
{
    public class TestingController : Controller
    {
        // GET: Testing
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Models.LearningFormModel form)
        {
            if (form != null)
            {
                TempData["form"] = form;
                return RedirectToAction("ViewForm");
            }
            return View();
        }

        public ActionResult ViewForm()
        {
            Models.LearningFormModel form = TempData["form"] as Models.LearningFormModel;
            var dirPath = Path.Combine(Server.MapPath("~/App_Data/"), form.folderName);
            List<string> files = Directory.GetFiles(dirPath).ToList();
            
            string filePath = Path.Combine(dirPath, files.Find(s => s.Contains(form.fileName)));

            ErrorBackpropagation backprop = new ErrorBackpropagation(filePath);
            double[][] learningTable;
            double[][] testingTable;
            Network network;
            backprop.testNetworkFromFile(out learningTable, out testingTable, out network);

            string layers = "";
            foreach (Neuron[] layer in network.layers)
            {
                layers = string.Concat(layers, layer.Length + " ");
            }

            ViewBag.LearningTable = learningTable;
            ViewBag.TestingTable = testingTable;
            ViewBag.Layers = layers;
            ViewBag.LearningParameter = network.learningParameter;
            ViewBag.ErrorTolerance = network.outputTolerance;

            return View(form);
        }
    }
}