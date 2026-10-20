namespace DAL_Celebrity_MSSQL
// Реализует все методы для работы с БД
{
    public class Repository : IRepository
    {
        private Context context;

        public Repository()
        {
            this.context = new Context();
        }

        public Repository(string connectionstring)
        {
            this.context = new Context(connectionstring);
        }

        public static IRepository Create()
        {
            return new Repository();
        }

        public static IRepository Create(string connectionstring)
        {
            return new Repository(connectionstring);
        }

        // ========== ICelebrity ==========
        public List<Celebrity> GetAllCelebrities()
        {
            return context.Celebrities.ToList();
        }

        public Celebrity? GetCelebrityById(int Id)
        {
            return context.Celebrities.Find(Id);
        }

        public bool AddCelebrity(Celebrity celebrity)
        {
            try
            {
                context.Celebrities.Add(celebrity);
                context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool DelCelebrity(int id)
        {
            var celebrity = context.Celebrities.Find(id);
            if (celebrity == null) return false;
            context.Celebrities.Remove(celebrity);
            context.SaveChanges();
            return true;
        }

        public bool UpdCelebrity(int id, Celebrity celebrity)
        {
            var existing = context.Celebrities.Find(id);
            if (existing == null) return false;
            existing.Update(celebrity);
            context.SaveChanges();
            return true;
        }

        public int GetCelebrityIdByName(string name)
        {
            var celebrity = context.Celebrities.FirstOrDefault(c => c.FullName.Contains(name));
            return celebrity?.Id ?? -1;
        }

        // ========== ILifeevent ==========
        public List<Lifeevent> GetAllLifeevents()
        {
            return context.Lifeevents.ToList();
        }

        public Lifeevent? GetLifeevetById(int Id)
        {
            return context.Lifeevents.Find(Id);
        }

        public bool AddLifeevent(Lifeevent lifeevent)
        {
            try
            {
                context.Lifeevents.Add(lifeevent);
                context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool DelLifeevent(int id)
        {
            var lifeevent = context.Lifeevents.Find(id);
            if (lifeevent == null) return false;
            context.Lifeevents.Remove(lifeevent);
            context.SaveChanges();
            return true;
        }

        public bool UpdLifeevent(int id, Lifeevent lifeevent)
        {
            var existing = context.Lifeevents.Find(id);
            if (existing == null) return false;
            existing.Update(lifeevent);
            context.SaveChanges();
            return true;
        }

        // ========== IMix ==========
        public List<Lifeevent> GetLifeeventsByCelebrityId(int celebrityId)
        {
            return context.Lifeevents.Where(l => l.CelebrityId == celebrityId).ToList();
        }

        public Celebrity? GetCelebrityByLifeeventId(int lifeeventId)
        {
            var lifeevent = context.Lifeevents.Find(lifeeventId);
            if (lifeevent == null) return null;
            return context.Celebrities.Find(lifeevent.CelebrityId);
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}