
public class Common
{
    // var path = Common.WebEnv().WebRootPath;
    public static IWebHostEnvironment? WebEnv()
    {
        var _accessor = new HttpContextAccessor();
        IWebHostEnvironment? r = _accessor?.HttpContext?.RequestServices?.GetRequiredService<IWebHostEnvironment>();
        return  r;
    }
}
