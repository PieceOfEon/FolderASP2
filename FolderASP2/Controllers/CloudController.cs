using FolderASP2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using File = FolderASP2.Models.File;
using static System.Net.WebRequestMethods;
using System.Text.Json;

namespace FolderASP2.Controllers
{
    public class CloudController : Controller
    {
        private static List<Folder> _folders = new List<Folder>();
        private static List<File> _files = new List<File>();
        private readonly IWebHostEnvironment _hostEnvironment;

        public CloudController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            InitializeFiles();
        }

        public IActionResult Index(int? folderId)
        {
            var viewModel = new CloudViewModel
            {
                Folders = LoadFolders(),
                Files = LoadFiles(),
                SelectedFolder = _folders.FirstOrDefault(f => f.Id == folderId),
                SelectedFiles = _files.Where(f => f.FolderId == folderId).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateFolder(string folderName)
        {
            var newFolder = new Folder
            {
                Id = _folders.Count + 1,
                Name = folderName,
                Path = $"/Uploads/{folderName}" // Сохраняем путь относительно папки Uploads
            };

            string uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "Uploads", folderName);
            Console.WriteLine($"uploadsPath: {uploadsPath}"); // Проверяем путь к создаваемой папке
            Directory.CreateDirectory(uploadsPath);
            _folders.Add(newFolder);

            SaveFolders(); // Сохраняем папки в файл
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UploadFile(int folderId, Microsoft.AspNetCore.Http.IFormFile file)
        {
            var folder = _folders.FirstOrDefault(f => f.Id == folderId);
            if (folder != null && file != null)
            {
                string folderPath = Path.Combine(_hostEnvironment.WebRootPath, "Uploads", folder.Name);
                string filePath = Path.Combine(folderPath, file.FileName);
                Console.WriteLine($"folderPath: {folderPath}"); // Проверяем путь к папке для загрузки файла
                Console.WriteLine($"filePath: {filePath}"); // Проверяем путь к создаваемому файлу
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var newFile = new File
                {
                    Id = _files.Count + 1,
                    Name = file.FileName,
                    Path = $"/Uploads/{folder.Name}/{file.FileName}", // Сохраняем путь относительно папки Uploads
                    FolderId = folderId
                };

                _files.Add(newFile);
                SaveFiles(); // Сохраняем файлы в файл
            }

            return RedirectToAction("Index");
        }

        private List<Folder> LoadFolders()
        {
            string filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Data", "folders.json");
            if (!System.IO.File.Exists(filePath))
            {
                return new List<Folder>();
            }

            string json = System.IO.File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Folder>();
            }

            return JsonSerializer.Deserialize<List<Folder>>(json);
        }

        private void SaveFolders()
        {
            string filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Data", "folders.json");
            string json = JsonSerializer.Serialize(_folders);
            System.IO.File.WriteAllText(filePath, json);
        }

        private List<File> LoadFiles()
        {
            string filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Data", "files.json");
            if (!System.IO.File.Exists(filePath))
            {
                return new List<File>();
            }

            string json = System.IO.File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<File>();
            }

            return JsonSerializer.Deserialize<List<File>>(json);
        }

        private void SaveFiles()
        {
            string filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Data", "files.json");
            string json = JsonSerializer.Serialize(_files);
            System.IO.File.WriteAllText(filePath, json);
        }

        private void InitializeFiles()
        {
            string folderPath = Path.Combine(_hostEnvironment.ContentRootPath, "Data");
            string foldersFilePath = Path.Combine(folderPath, "folders.json");
            string filesFilePath = Path.Combine(folderPath, "files.json");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!System.IO.File.Exists(foldersFilePath))
            {
                System.IO.File.WriteAllText(foldersFilePath, "[]");
            }

            if (!System.IO.File.Exists(filesFilePath))
            {
                System.IO.File.WriteAllText(filesFilePath, "[]");
            }
        }


    }
}
