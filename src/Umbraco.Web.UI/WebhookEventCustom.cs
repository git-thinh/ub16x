using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Webhooks;
using Umbraco.Cms.Core.Webhooks.Events;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using static Umbraco.Cms.Core.Constants;


[WebhookEvent("Content Published", WebhookEvents.Types.Content)]
public class WebhookEventCustom : ExtendedContentWebhookEventBase<ContentPublishedNotification>
{
    private readonly IApiContentResponseBuilder _apiContentBuilder;
    private readonly IPublishedContentCache _publishedContentCache;

    public WebhookEventCustom(
        IWebhookFiringService webhookFiringService,
        IWebhookService webhookService,
        IOptionsMonitor<WebhookSettings> webhookSettings,
        IServerRoleAccessor serverRoleAccessor,
        IApiContentResponseBuilder apiContentBuilder,
        IPublishedContentCache publishedContentCache,
        IOutputExpansionStrategyAccessor outputExpansionStrategyAccessor,
        IVariationContextAccessor variationContextAccessor)
        : base(
            webhookFiringService,
            webhookService,
            webhookSettings,
            serverRoleAccessor,
            outputExpansionStrategyAccessor,
            variationContextAccessor)
    {
        _apiContentBuilder = apiContentBuilder;
        _publishedContentCache = publishedContentCache;
    }

    public override string Alias => WebhookEvents.Aliases.ContentPublish;

    protected override IEnumerable<IContent> GetEntitiesFromNotification(ContentPublishedNotification notification) =>
        notification.PublishedEntities;

    protected override object? ConvertEntityToRequestPayload(IContent entity)
    {
        IPublishedContent? publishedContent = _publishedContentCache.GetById(entity.Key);
        IApiContentResponse? deliveryContent =
            publishedContent is null ? null : _apiContentBuilder.Build(publishedContent);

        if (deliveryContent == null)
        {
            return null;
        }

        Dictionary<string, object> cultures = BuildCultureProperties(publishedContent!, deliveryContent);

        var r = new
        {
            deliveryContent.Id,
            deliveryContent.ContentType,
            deliveryContent.Name,
            deliveryContent.CreateDate,
            deliveryContent.UpdateDate,
            Cultures = cultures,
        };

        return r;
    }
}

public class AllWebhookComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.WebhookEvents().Remove<ExtendedContentPublishedWebhookEvent>();
        builder.WebhookEvents().Add<WebhookEventCustom>();
    }
}
