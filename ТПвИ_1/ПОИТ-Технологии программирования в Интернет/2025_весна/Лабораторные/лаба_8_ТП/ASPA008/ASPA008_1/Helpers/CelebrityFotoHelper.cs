using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace ASPA008_1.Helpers
{
    public static class CelebrityFotoHtmlHelper
    {
        public static IHtmlContent CelebrityFoto(
            this IHtmlHelper html,
            DAL_Celebrity_MSSQL.Celebrity? celebrity,
            int width = 180,
            int height = 180,
            string? css = null,
            string? title = null)
        {
            if (celebrity == null)
                return HtmlString.Empty;

            var fname = celebrity.ReqPhotoPath ?? string.Empty;
            var src   = string.IsNullOrEmpty(fname) ? "/images/NewCelebrity.jpg" : $"/photo/{fname}";
            var alt   = HtmlEncoder.Default.Encode(celebrity.FullName ?? string.Empty);
            var ttl   = HtmlEncoder.Default.Encode(title ?? celebrity.FullName ?? string.Empty);
            var cls   = css == null ? "" : $" class=\"{HtmlEncoder.Default.Encode(css)}\"";

            var img = $"<img src=\"{src}\" alt=\"{alt}\" title=\"{ttl}\" " +
                      $"width=\"{width}\" height=\"{height}\"{cls} " +
                      $"onerror=\"this.src='/images/NewCelebrity.jpg'\" " +
                      $"style=\"object-fit:cover; border-radius:8px;\" />";
            return new HtmlString(img);
        }
    }
}
