using DAL_Celebrity;
using DAL_Celebrity_MSSQL;

namespace DAL_Celebrity_MSSQL_Test1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Строка подключения к БД
            string CS = @"Data source=DESKTOP-IMNOJB5; Initial Catalog=LES01; TrustServerCertificate=True; Integrated Security=True";

            // Инициализация БД
            Init init = new Init(CS);
            Init.Execute(delete: true, create: true);

            Func<Celebrity, string> printC = (c) => $"Id = {c.Id}, FullName = {c.FullName}, Nationality = {c.Nationality}, ReqPhotoPath = {c.ReqPhotoPath}";
            Func<Lifeevent, string> printL = (l) => $"Id = {l.Id}, CelebrityId = {l.CelebrityId}, Date = {l.Date}, Description = {l.Description}, ReqPhotoPath = {l.ReqPhotoPath}";
            Func<string, string> puri = (string f) => $"{f}";

            using (IRepository repo = Repository.Create(CS))
            {
                // ========== 1. GetAllCelebrities() ==========
                Console.WriteLine("\n------ GetAllCelebrities() ------------- ");
                repo.GetAllCelebrities().ForEach(celebrity => Console.WriteLine(printC(celebrity)));

                // ========== 2. GetAllLifeevents() ==========
                Console.WriteLine("\n------ GetAllLifeevents() ------------- ");
                repo.GetAllLifeevents().ForEach(life => Console.WriteLine(printL(life)));

                // ========== 3. AddCelebrity() - добавляем Einstein ==========
                Console.WriteLine("\n------ AddCelebrity() (Einstein) --------------- ");
                Celebrity c1 = new Celebrity { FullName = "Albert Einstein", Nationality = "DE", ReqPhotoPath = puri("Einstein.jpg") };
                if (repo.AddCelebrity(c1))
                    Console.WriteLine($"OK: AddCelebrity: {printC(c1)}");
                else
                    Console.WriteLine($"ERROR: AddCelebrity: {printC(c1)}");

                // ========== 4. AddCelebrity() - добавляем Huntington ==========
                Console.WriteLine("\n------ AddCelebrity() (Huntington) --------------- ");
                Celebrity c2 = new Celebrity { FullName = "Samuel Huntington", Nationality = "US", ReqPhotoPath = puri("Huntington.jpg") };
                if (repo.AddCelebrity(c2))
                    Console.WriteLine($"OK: AddCelebrity: {printC(c2)}");
                else
                    Console.WriteLine($"ERROR: AddCelebrity: {printC(c2)}");

                // ========== 5. DelCelebrity() - удаляем Einstein ==========
                Console.WriteLine("\n------ DelCelebrity() --------------- ");
                int id = 0;
                if ((id = repo.GetCelebrityIdByName("Einstein")) > 0)
                {
                    Celebrity? c = repo.GetCelebrityById(id);
                    if (c != null)
                    {
                        Console.WriteLine($"Найден: {printC(c)}");
                        if (!repo.DelCelebrity(id))
                            Console.WriteLine($"ERROR: DelCelebrity: {id}");
                        else
                            Console.WriteLine($"OK: DelCelebrity: {id}");
                    }
                    else
                        Console.WriteLine($"ERROR: GetCelebrityById: {id}");
                }
                else
                    Console.WriteLine($"ERROR: GetCelebrityIdByName (Einstein)");

                // ========== 6. UpdCelebrity() - обновляем Huntington ==========
                Console.WriteLine("\n------ UpdCelebrity() --------------- ");
                if ((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                {
                    Celebrity? c = repo.GetCelebrityById(id);
                    if (c != null)
                    {
                        Console.WriteLine($"До обновления: {printC(c)}");
                        c.FullName = "Samuel Phillips Huntington";
                        if (!repo.UpdCelebrity(id, c))
                            Console.WriteLine($"ERROR: UpdCelebrity: {id}");
                        else
                        {
                            Console.WriteLine($"OK: UpdCelebrity: {id}");
                            Celebrity? c1_updated = repo.GetCelebrityById(id);
                            if (c1_updated == null)
                                Console.WriteLine($"ERROR: GetCelebrityById {id}");
                            else
                                Console.WriteLine($"После обновления: {printC(c1_updated)}");
                        }
                    }
                    else
                        Console.WriteLine($"ERROR: GetCelebrityById: {id}");
                }
                else
                    Console.WriteLine($"ERROR: GetCelebrityIdByName (Huntington)");

                // ========== 7. AddLifeevent() - добавляем события для Huntington ==========
                Console.WriteLine("\n------ AddLifeevent() --------------- ");
                if ((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                {
                    Celebrity? c = repo.GetCelebrityById(id);
                    if (c != null)
                    {
                        Console.WriteLine($"Добавляем события для: {printC(c)}");
                        Lifeevent l1, l2;
                        if (repo.AddLifeevent(l1 = new Lifeevent { CelebrityId = id, Date = new DateTime(1927, 4, 18), Description = "Дата рождения" }))
                            Console.WriteLine($"OK: AddLifeevent, {printL(l1)}");
                        else
                            Console.WriteLine($"ERROR: AddLifeevent, {printL(l1)}");

                        if (repo.AddLifeevent(l2 = new Lifeevent { CelebrityId = id, Date = new DateTime(2008, 12, 24), Description = "Дата смерти" }))
                            Console.WriteLine($"OK: AddLifeevent, {printL(l2)}");
                        else
                            Console.WriteLine($"ERROR: AddLifeevent, {printL(l2)}");
                    }
                    else
                        Console.WriteLine($"ERROR: GetCelebrityById: {id}");
                }
                else
                    Console.WriteLine($"ERROR: GetCelebrityIdByName (Huntington)");

                // ========== 8. DelLifeevent() - удаляем событие с id=22 ==========
                Console.WriteLine("\n------ DelLifeevent() --------------- ");
                int delId = 22;
                if (repo.DelLifeevent(delId))
                    Console.WriteLine($"OK: DelLifeevent: {delId}");
                else
                    Console.WriteLine($"ERROR: DelLifeevent: {delId}");

                // ========== 9. UpdLifeevent() - обновляем событие с id=23 ==========
                Console.WriteLine("\n------ UpdLifeevent() --------------- ");
                int updId = 23;
                Lifeevent? l1_update;
                if ((l1_update = repo.GetLifeevetById(updId)) != null)
                {
                    Console.WriteLine($"До обновления: {printL(l1_update)}");
                    l1_update.Description = "Дата смерти (обновлено)";
                    if (repo.UpdLifeevent(updId, l1_update))
                        Console.WriteLine($"OK: UpdLifeevent {updId}, {printL(l1_update)}");
                    else
                        Console.WriteLine($"ERROR: UpdLifeevent {updId}");
                }
                else
                    Console.WriteLine($"ERROR: GetLifeevetById: {updId}");

                // ========== 10. GetLifeeventsByCelebrityId() ==========
                Console.WriteLine("\n------ GetLifeeventsByCelebrityId() ------------- ");
                if ((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                {
                    Celebrity? c = repo.GetCelebrityById(id);
                    if (c != null)
                    {
                        var events = repo.GetLifeeventsByCelebrityId(c.Id);
                        if (events.Count > 0)
                            events.ForEach(l => Console.WriteLine($"OK: GetLifeeventsByCelebrityId, {id}, {printL(l)}"));
                        else
                            Console.WriteLine($"Нет событий для знаменитости {id}");
                    }
                    else
                        Console.WriteLine($"ERROR: GetCelebrityById: {id}");
                }
                else
                    Console.WriteLine($"ERROR: GetCelebrityIdByName (Huntington)");

                // ========== 11. GetCelebrityByLifeeventId() ==========
                Console.WriteLine("\n------ GetCelebrityByLifeeventId() ------------- ");
                int lifeId = 23;
                Celebrity? foundCelebrity;
                if ((foundCelebrity = repo.GetCelebrityByLifeeventId(lifeId)) != null)
                    Console.WriteLine($"OK: Событие {lifeId} принадлежит знаменитости: {printC(foundCelebrity)}");
                else
                    Console.WriteLine($"ERROR: GetCelebrityByLifeeventId, {lifeId}");
            }

            Console.WriteLine("\n------------> Тест завершён. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}