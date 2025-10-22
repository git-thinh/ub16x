// <link href='@Url.CdnContent("~/assets/images/favicon.ico")'>

using Microsoft.AspNetCore.Mvc;
public static class UrlCdnContent
{
    static string _cdnHost = "https://mascotsitecore-1ccb8.kxcdn.com";
    static int _cdnVersion = 0;

    public static string CdnContent(this IUrlHelper url, string contentPath)
    {
        var _env = Common.WebEnv();
        bool isDev = false;
        if (_env != null)
            isDev = _env.IsDevelopment();

        var localPath = url.Content(contentPath);
        string lk = "";

        if (!isDev)
        {
            lk = _cdnHost + localPath;
        }
        else
        {
            string scheme = url.ActionContext.HttpContext.Request.Scheme;
            int? port = url.ActionContext.HttpContext.Request.Host.Port;
            string host = url.ActionContext.HttpContext.Request.Host.Host;
      
            var uriBuilder = new UriBuilder
            {
                Scheme = scheme,
                Port = Convert.ToInt32(port),
                Host = host,
                Path = localPath,
            };

            if (_cdnVersion > 0)
                uriBuilder.Query = string.Format("v={0}", _cdnVersion);

            lk = uriBuilder.ToString();
        }

        return lk;
    }
}
