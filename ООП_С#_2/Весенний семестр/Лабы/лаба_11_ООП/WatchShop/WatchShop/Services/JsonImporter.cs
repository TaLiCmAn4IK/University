using System;

namespace WatchShop.Services
{
    /// <summary>
    /// При первом запуске (когда БД создана с нуля) переносит существующие
    /// данные из JSON-файлов %LocalAppData%/WatchShop/ в реляционную БД.
    /// Это делается один раз, чтобы не потерять то, что было в старой версии
    /// из лаб 5-7. После переноса все операции идут уже через ADO.NET.
    /// </summary>
    public class JsonImporter
    {
        private readonly DataService dataService = new();
        private readonly WatchRepository watchRepo = new();
        private readonly UserRepository userRepo = new();

        public void ImportAll()
        {
            ImportUsers();
            ImportWatches();
        }

        private void ImportUsers()
        {
            try
            {
                var users = dataService.LoadUsers();
                foreach (var u in users)
                {
                    if (userRepo.UsernameExists(u.Username)) continue;
                    try
                    {
                        userRepo.Register(u.Username, u.Password, u.Email);
                    }
                    catch
                    {
                        // конкретного пользователя пропускаем
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JsonImporter Users: " + ex.Message);
            }
        }

        private void ImportWatches()
        {
            try
            {
                var watches = dataService.LoadWatches();
                foreach (var w in watches)
                {
                    try
                    {
                        // Сбрасываем Id, чтобы БД сгенерировала свой.
                        // Manufacturer/Category — строки в JSON, репозиторий
                        // сам разрешит их в записи Manufacturers/Categories.
                        w.Id = 0;
                        watchRepo.InsertWatch(w);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Watch '{w.ShortName}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JsonImporter Watches: " + ex.Message);
            }
        }
    }
}
