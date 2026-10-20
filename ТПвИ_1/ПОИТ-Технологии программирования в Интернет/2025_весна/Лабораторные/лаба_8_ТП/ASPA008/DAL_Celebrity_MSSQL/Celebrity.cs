namespace DAL_Celebrity_MSSQL
{
    public class Celebrity
    {
        public Celebrity()
        {
            this.FullName = string.Empty;
            this.Nationality = string.Empty;
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string? ReqPhotoPath { get; set; }

        public virtual bool Update(Celebrity celebrity)
        {
            if (!string.IsNullOrEmpty(celebrity.FullName))
                this.FullName = celebrity.FullName;
            if (!string.IsNullOrEmpty(celebrity.Nationality))
                this.Nationality = celebrity.Nationality;
            if (!string.IsNullOrEmpty(celebrity.ReqPhotoPath))
                this.ReqPhotoPath = celebrity.ReqPhotoPath;
            return true;
        }
    }
}