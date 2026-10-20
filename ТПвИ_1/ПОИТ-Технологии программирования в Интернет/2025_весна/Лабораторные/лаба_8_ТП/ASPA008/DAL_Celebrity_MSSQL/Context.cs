using Microsoft.EntityFrameworkCore;

namespace DAL_Celebrity_MSSQL
{
    public class Context : DbContext
    {
        public string? ConnectionString { get; private set; } = null;

        public Context(string constring) : base()
        {
            this.ConnectionString = constring;
        }

        public Context() : base() { }

        public DbSet<Celebrity> Celebrities { get; set; }
        public DbSet<Lifeevent> Lifeevents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Если строка подключения не задана — используем значение по умолчанию
            if (this.ConnectionString is null)
                this.ConnectionString = @"Data source=.;";
            optionsBuilder.UseSqlServer(this.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Celebrity>().ToTable("Celebrities");
            modelBuilder.Entity<Celebrity>().HasKey(c => c.Id);
            modelBuilder.Entity<Celebrity>().Property(c => c.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Lifeevent>().ToTable("Lifeevents");
            modelBuilder.Entity<Lifeevent>().HasKey(l => l.Id);
            modelBuilder.Entity<Lifeevent>().Property(l => l.Id).ValueGeneratedOnAdd();

            // настройка связи один ко многим
            modelBuilder.Entity<Lifeevent>()
                .HasOne<Celebrity>()
                .WithMany()
                .HasForeignKey(l => l.CelebrityId);
        }
    }
}