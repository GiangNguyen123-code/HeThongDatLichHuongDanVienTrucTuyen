using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeThongDatLich.Helpers
{
    public static class ThongBaoTrungLich
    {
        public static IHtmlContent ShowError(this IHtmlHelper html, string message)
        {
            if (string.IsNullOrEmpty(message)) return HtmlString.Empty;

            var htmlString = $@"
                <div style='background-color: #f8d7da; color: #721c24; padding: 10px; border-radius: 6px; margin-bottom: 15px; border: 1px solid #f5c6cb;'>
                    <i class='fa-solid fa-circle-exclamation'></i> {message}
                </div>";

            return new HtmlString(htmlString);
        }
    }
}
