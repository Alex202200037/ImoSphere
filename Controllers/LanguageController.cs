using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        if (string.IsNullOrEmpty(culture))
            culture = "pt";
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = "/";
        return LocalRedirect(returnUrl);
    }
}