using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL_Celebrity_MSSQL;

namespace ASPA007_1.Pages
{
    public class PhotoModel : PageModel
    {
        private readonly IRepository _repo;
        public PhotoModel(IRepository repo) { _repo = repo; }

        public Celebrity? Celebrity { get; set; }
        public List<Lifeevent> Events { get; set; } = new();

        public void OnGet(int id)
        {
            Celebrity = _repo.GetCelebrityById(id);
            if (Celebrity != null)
                Events = _repo.GetLifeeventsByCelebrityId(id);
        }
    }
}
