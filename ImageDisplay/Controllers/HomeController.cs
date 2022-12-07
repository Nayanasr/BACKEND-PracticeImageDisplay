using ImageDisplay.Data;
using ImageDisplay.Models;
using ImageDisplay.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDisplay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

       private readonly ApplicationDbContext Context;

        private readonly IWebHostEnvironment webHostEnvironment1;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment ) {
            _logger = logger;
            Context = context;
        }

        public IActionResult Index() {
            var items = Context.students.ToList();
            return View(items);
        }
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentViewModel vm) {

            string stringFileName = UploadFile(vm);
            var student = new Student {
                Name = vm.Name,
                ProfileImage = stringFileName
            };
            Context.students.Add(student);
            Context.SaveChanges();

            return RedirectToAction("Index");
        }

        private string UploadFile(StudentViewModel vm) {
            string fileName = null;
            if (vm.ProfileImage != null) {
                string uploadDir = Path.Combine(webHostEnvironment1.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "_" + vm.ProfileImage.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    vm.ProfileImage.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
