using Microsoft.AspNetCore.Mvc;
using MvcCoreStorage.Services;

namespace MvcCoreStorage.Controllers
{
    public class AzureFilesController : Controller
    {
        private ServiceStorageFiles service;

        public AzureFilesController(ServiceStorageFiles service)
        {
            this.service = service;
        }

        public async Task< IActionResult> Index()
        {
            List<string> files = await this.service.GetFilesAsync();
            return View(files);
        }

        public async Task<IActionResult> ReadFile(string filename)
        {
            string data = await this.service.ReadFileAsync(filename);
            ViewData["DATA"] = data;
            return View();
        }

        public async Task<IActionResult> DeleteFile(string filename)
        {
            await this.service.DeleteFilesAsync(filename);
            return RedirectToAction("Index");
        }

        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            string fileName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadFileAsync(fileName, stream);
            }
            ViewData["MENSAJE"] = "Fichero subido correctamente";
            return View();
        }
    }
}
