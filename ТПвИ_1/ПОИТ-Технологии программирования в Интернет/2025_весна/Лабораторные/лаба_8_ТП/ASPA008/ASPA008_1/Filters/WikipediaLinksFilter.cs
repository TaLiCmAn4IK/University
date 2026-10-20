using ASPA008_1.Config;
using DAL_Celebrity_MSSQL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ASPA008_1.Filters
{
    /// <summary>
    /// Action-фильтр: формирует динамические ссылки на статьи Wikipedia
    /// для всех знаменитостей и кладёт их в ViewData["WikiLinks"].
    /// Используется на действии Details, отображающем знаменитость.
    /// </summary>
    public class WikipediaLinksFilter : IActionFilter
    {
        private readonly IRepository _repo;
        private readonly CelebritiesConfig _config;

        public WikipediaLinksFilter(IRepository repo, IOptions<CelebritiesConfig> options)
        {
            _repo   = repo;
            _config = options.Value;
        }

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is not Controller controller) return;

            var baseUrl = string.IsNullOrWhiteSpace(_config.WikipediaBaseUrl)
                          ? "https://en.wikipedia.org/wiki/"
                          : _config.WikipediaBaseUrl;

            var links = _repo.GetAllCelebrities()
                             .Select(c => new WikiLink
                             {
                                 Id       = c.Id,
                                 FullName = c.FullName,
                                 Url      = baseUrl + Uri.EscapeDataString(
                                                (c.FullName ?? string.Empty).Replace(' ', '_'))
                             })
                             .ToList();

            controller.ViewData["WikiLinks"] = links;
        }
    }

    public class WikiLink
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
