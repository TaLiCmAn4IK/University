using ASPA008_1.Config;
using ASPA008_1.Filters;
using ASPA008_1.Models;
using ASPA008_1.Services;
using DAL_Celebrity_MSSQL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASPA008_1.Controllers
{
    public class CelebritiesController : Controller
    {
        private readonly IRepository _repo;
        private readonly CelebritiesConfig _config;
        private readonly IWebHostEnvironment _env;
        private readonly ICountryCodesService _countries;
        private readonly CelebrityTitles _titles;

        public CelebritiesController(
            IRepository repo,
            IOptions<CelebritiesConfig> options,
            IWebHostEnvironment env,
            ICountryCodesService countries,
            CelebrityTitles titles)
        {
            _repo      = repo;
            _config    = options.Value;
            _env       = env;
            _countries = countries;
            _titles    = titles;
        }

        public IActionResult Index()
        {
            ViewData["Title"]  = _titles.Index;
            var list = _repo.GetAllCelebrities();
            return View(list);
        }

        [ServiceFilter(typeof(WikipediaLinksFilter))]
        public IActionResult Details(int id)
        {
            var celebrity = _repo.GetCelebrityById(id);
            if (celebrity == null) return NotFound();

            ViewData["Title"]   = _titles.Details;
            ViewData["Events"]  = _repo.GetLifeeventsByCelebrityId(id);
            ViewData["Country"] = _countries.GetLabel(celebrity.Nationality) ?? celebrity.Nationality;
            return View(celebrity);
        }

        // Добавление
        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Title"] = _titles.Add;
            return View(new CelebrityInputModel());
        }

        // Добавление-подтверждение
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(CelebrityInputModel input)
        {
            ViewData["Title"] = _titles.Add;
            if (!ModelState.IsValid) return View(input);

            if (input.PhotoFile != null && input.PhotoFile.Length > 0)
            {
                var imagesDir = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imagesDir);

                var ext     = Path.GetExtension(input.PhotoFile.FileName);
                var tmpName = Guid.NewGuid() + ext;
                var tmpPath = Path.Combine(imagesDir, tmpName);

                using (var stream = new FileStream(tmpPath, FileMode.Create))
                    input.PhotoFile.CopyTo(stream);

                input.ReqPhotoPath = input.PhotoFile.FileName;
                ViewData["TempFileName"] = tmpName;
            }

            ViewData["Title"] = _titles.Confirm;
            return View("AddConfirm", input);
        }

        // Добавление: финальное подтверждение 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddConfirm(CelebrityInputModel input, string? tempFileName)
        {
            if (!string.IsNullOrEmpty(tempFileName) && !string.IsNullOrEmpty(input.ReqPhotoPath))
            {
                var tmpPath  = Path.Combine(_env.WebRootPath, "images", tempFileName);
                Directory.CreateDirectory(_config.PhotosFolder);
                var destPath = Path.Combine(_config.PhotosFolder, input.ReqPhotoPath);

                if (System.IO.File.Exists(tmpPath))
                {
                    System.IO.File.Copy(tmpPath, destPath, overwrite: true);
                    System.IO.File.Delete(tmpPath);
                }
            }

            _repo.AddCelebrity(new Celebrity
            {
                FullName     = input.FullName,
                Nationality  = input.Nationality,
                ReqPhotoPath = input.ReqPhotoPath
            });

            return RedirectToAction(nameof(Index));
        }

        // Редактирование: GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var celebrity = _repo.GetCelebrityById(id);
            if (celebrity == null) return NotFound();

            ViewData["Title"] = _titles.Edit;
            var input = new CelebrityInputModel
            {
                Id           = celebrity.Id,
                FullName     = celebrity.FullName,
                Nationality  = celebrity.Nationality,
                ReqPhotoPath = celebrity.ReqPhotoPath
            };
            return View(input);
        }

        // Редактирование: POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CelebrityInputModel input)
        {
            ViewData["Title"] = _titles.Edit;
            if (!ModelState.IsValid) return View(input);

            var existing = _repo.GetCelebrityById(input.Id);
            if (existing == null) return NotFound();

            // Если загружено новое фото — сохраняем в PhotosFolder.
            if (input.PhotoFile != null && input.PhotoFile.Length > 0)
            {
                Directory.CreateDirectory(_config.PhotosFolder);
                var fname = input.PhotoFile.FileName;
                var path  = Path.Combine(_config.PhotosFolder, fname);
                using (var stream = new FileStream(path, FileMode.Create))
                    input.PhotoFile.CopyTo(stream);
                input.ReqPhotoPath = fname;
            }

            _repo.UpdCelebrity(input.Id, new Celebrity
            {
                FullName     = input.FullName,
                Nationality  = input.Nationality,
                ReqPhotoPath = input.ReqPhotoPath
            });

            return RedirectToAction(nameof(Details), new { id = input.Id });
        }

        // Удаление: GET (подтверждение)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var celebrity = _repo.GetCelebrityById(id);
            if (celebrity == null) return NotFound();

            ViewData["Title"]   = _titles.Delete;
            ViewData["Country"] = _countries.GetLabel(celebrity.Nationality) ?? celebrity.Nationality;
            return View(celebrity);
        }

        // Удаление: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.DelCelebrity(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
