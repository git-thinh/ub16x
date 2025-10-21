using OpenIddict.Server;
using Umbraco.Cms.Core.Composing;

public class InsecureComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        if (builder.Config.GetValue<bool>("Umbraco:CMS:Global:UseHttps") == false)
        {
            builder.Services.PostConfigure<OpenIddictServerOptions>(x => { x.RequireProofKeyForCodeExchange = false; });
        }
    }
}
