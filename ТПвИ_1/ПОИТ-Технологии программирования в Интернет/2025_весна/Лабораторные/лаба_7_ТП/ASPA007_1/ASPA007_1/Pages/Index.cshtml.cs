using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL_Celebrity_MSSQL;

namespace ASPA007_1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRepository _repo;
        public IndexModel(IRepository repo) { _repo = repo; }

        public List<Celebrity> Celebrities { get; set; } = new();

        public void OnGet()
        {
            Celebrities = _repo.GetAllCelebrities();
        }
    }
}
