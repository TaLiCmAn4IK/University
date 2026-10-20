namespace DAL_Celebrity_MSSQL
{
    public class Lifeevent
    {
        public Lifeevent()
        {
            this.Description = string.Empty;
        }

        public int Id { get; set; }
        public int CelebrityId { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string? ReqPhotoPath { get; set; }

        public virtual bool Update(Lifeevent lifeevent)
        {
            if (!(lifeevent.CelebrityId <= 0))
                this.CelebrityId = lifeevent.CelebrityId;
            if (!lifeevent.Date.Equals(new DateTime()))
                this.Date = lifeevent.Date;
            if (!string.IsNullOrEmpty(lifeevent.Description))
                this.Description = lifeevent.Description;
            if (!string.IsNullOrEmpty(lifeevent.ReqPhotoPath))
                this.ReqPhotoPath = lifeevent.ReqPhotoPath;
            return true;
        }
    }
}