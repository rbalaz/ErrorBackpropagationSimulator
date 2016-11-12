using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ErrorBackpropagationMVC.Controllers
{
    public class LearningFormController : Controller
    {
        // GET: API
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

            ErrorBackpropagation backprop = new ErrorBackpropagation(form.layers, filePath, form.learningParameter, form.errorTolerance, form.successRate, dirPath);

            double[] learningRate;
            double[] errorRate;
            double[][] learningTable;
            double[][] testingTable;

            backprop.execute(out learningRate, out errorRate, out learningTable, out testingTable);
            if (backprop.outputs == 2)
                string.Concat(form.layers, " 1");
            else
                string.Concat(form.layers, " 2");

            if (learningRate.Length < 100)
            {
                ViewBag.LearningRate = learningRate.Select(x => x.ToString()).ToArray();
                ViewBag.ErrorRate = errorRate.Select(x => x.ToString()).ToArray();
                ViewBag.XAxisData = Enumerable.Range(1, errorRate.Length).ToArray();
            }
            else
            {
                Array.Copy(learningRate.Select(x => x.ToString()).ToArray(), learningRate.Length - 101, ViewBag.LearningRate, 0, 100);
                Array.Copy(errorRate.Select(x => x.ToString()).ToArray(), errorRate.Length - 101, ViewBag.ErrorRate, 0, 100);
                ViewBag.XAxisData = Enumerable.Range(1, 100).ToArray();
            }

            string chartFileName = "chart.txt";
            string chartFilePath = Path.Combine(Server.MapPath("~/App_Data/"), chartFileName);
            FileStream stream = new FileStream(chartFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            for (int i = 0; i < ViewBag.XAxisData.Length; i++)
                writer.Write(ViewBag.XAxisData[i] + " ");
            writer.WriteLine();
            for (int i = 0; i < ViewBag.ErrorRate.Length; i++)
                writer.Write(ViewBag.ErrorRate[i] + " ");
            writer.WriteLine();
            for (int i = 0; i < ViewBag.LearningRate.Length; i++)
                writer.Write(ViewBag.LearningRate[i] + " ");
            writer.Close();
            stream.Close();

            ViewBag.LearningTable = learningTable;
            ViewBag.TestingTable = testingTable;

            return View(form);
        }

        public ActionResult RatesChart()
        {
            string chartFilePath = Path.Combine(Server.MapPath("~/App_Data/"), "chart.txt");
            FileStream stream = new FileStream(chartFilePath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            string[] line = reader.ReadLine().Trim(' ').Split(' ');
            int[] xAxisData = line.Select(s => int.Parse(s)).ToArray();

            string[] errorRate = reader.ReadLine().Trim(' ').Split(' ');
            string[] learningRate = reader.ReadLine().Trim(' ').Split(' ');
            reader.Close();
            stream.Close();

            System.IO.File.Delete(chartFilePath);

            Chart ratesChart = new Chart(width: 600, height: 400)
            .AddTitle("Progress of global error and learning success")
            .AddSeries(
                name: "Global Error",
                chartType: "Line",
                xField: "Iteration",
                xValue: xAxisData,
                yFields: "Global Error",
                yValues: errorRate)
            .AddSeries(
                name: "Learning",
                chartType: "Line",
                xField: "Iteration",
                xValue: xAxisData,
                yFields: "LearningSuccess",
                yValues: learningRate)
            .AddLegend()
            .Write();
            
            ratesChart.Save("~/Content/chart", "jpeg");
            return File("~/Content/chart", "jpeg");
        }
    }
}