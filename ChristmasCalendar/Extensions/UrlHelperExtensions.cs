using Microsoft.AspNetCore.Mvc;

namespace ChristmasCalendar.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string GetLocalUrl(this IUrlHelper urlHelper, string? localUrl) =>
            urlHelper.IsLocalUrl(localUrl) ? localUrl : urlHelper.Page("/Index")!;
    }
}