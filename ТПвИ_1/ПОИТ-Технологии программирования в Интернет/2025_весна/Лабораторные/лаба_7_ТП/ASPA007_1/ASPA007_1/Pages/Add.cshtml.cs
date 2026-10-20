using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ASPA007_1.Config;
using DAL_Celebrity_MSSQL;

namespace ASPA007_1.Pages
{
    public class AddModel : PageModel
    {
        private readonly IRepository _repo;
        private readonly CelebritiesConfig _config;
        private readonly IWebHostEnvironment _env;

        public AddModel(IRepository repo, IOptions<CelebritiesConfig> config, IWebHostEnvironment env)
        {
            _repo = repo;
            _config = config.Value;
            _env = env;
        }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Nationality { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? PhotoFile { get; set; }

        [BindProperty]
        public string? OriginalFileName { get; set; }

        [BindProperty]
        public string? TempFileName { get; set; }

        public bool ShowConfirm { get; set; } = false;
        public string? Message { get; set; }

        public void OnGet() { }

        // ШАГ 1: пользователь заполнил форму - показываем подтверждение
        public IActionResult OnPostStep1()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Nationality))
            {
                Message = "Заполните все поля!";
                return Page();
            }

            // Сохраняем фото во временную папку wwwroot/images
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var imagesDir = Path.Combine(_env.WebRootPath, "images");
                // создаём папку если не существует
                if (!Directory.Exists(imagesDir))
                    Directory.CreateDirectory(imagesDir);

                TempFileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                OriginalFileName = PhotoFile.FileName;

                var tempPath = Path.Combine(imagesDir, TempFileName);
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    PhotoFile.CopyTo(stream);
                }
            }

            ShowConfirm = true;
            return Page();
        }

        // ШАГ 2: подтверждение - добавляем в БД, копируем фото в PhotosFolder
        public IActionResult OnPostConfirm()
        {
            // Копируем фото из wwwroot/images в PhotosFolder
            if (!string.IsNullOrEmpty(TempFileName) && !string.IsNullOrEmpty(OriginalFileName))
            {
                var tempPath = Path.Combine(_env.WebRootPath, "images", TempFileName);

                if (!Directory.Exists(_config.PhotosFolder))
                    Directory.CreateDirectory(_config.PhotosFolder);

                var destPath = Path.Combine(_config.PhotosFolder, OriginalFileName);

                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Copy(tempPath, destPath, overwrite: true);
                    System.IO.File.Delete(tempPath);
                }
            }

            // Добавляем в БД
            var celebrity = new Celebrity
            {
                FullName = FullName,
                Nationality = Nationality,
                ReqPhotoPath = OriginalFileName
            };
            _repo.AddCelebrity(celebrity);

            return RedirectToPage("/Index");
        }
    }
}
