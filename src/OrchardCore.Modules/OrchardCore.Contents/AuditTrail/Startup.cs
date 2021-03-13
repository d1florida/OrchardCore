using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.AuditTrail.Providers;
using OrchardCore.AuditTrail.Services;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Contents.AuditTrail.Controllers;
using OrchardCore.Contents.AuditTrail.Handlers;
using OrchardCore.Contents.AuditTrail.Indexes;
using OrchardCore.Contents.AuditTrail.Providers;
using OrchardCore.Contents.AuditTrail.Services;
using OrchardCore.Contents.AuditTrail.Shapes;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using YesSql.Indexes;

namespace OrchardCore.Contents.AuditTrail
{
    [RequireFeatures("OrchardCore.AuditTrail")]
    public class Startup : StartupBase
    {
        private readonly AdminOptions _adminOptions;

        public Startup(IOptions<AdminOptions> adminOptions)
        {
            _adminOptions = adminOptions.Value;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IIndexProvider, ContentAuditTrailEventIndexProvider>();

            services.AddScoped<IAuditTrailEventHandler, ContentAuditTrailEventHandler>();

            services.AddScoped<IShapeTableProvider, ContentAuditTrailEventShapesTableProvider>();

            services.AddScoped<IAuditTrailEventProvider, ContentAuditTrailEventProvider>();

            services.AddScoped<IAuditTrailContentEventHandler, AuditTrailContentTypesEvents>();

            services.AddScoped<IContentHandler, GlobalContentHandler>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var contentControllerName = typeof(ContentController).ControllerName();

            routes.MapAreaControllerRoute(
               name: "DetailContentItem",
               areaName: "OrchardCore.Contents",
               pattern: _adminOptions.AdminUrlPrefix + "/AuditTrail/Content/{versionNumber}/{auditTrailEventId}",
               defaults: new { controller = contentControllerName, action = nameof(ContentController.Detail) }
           );

            routes.MapAreaControllerRoute(
               name: "RestoreContentItem",
               areaName: "OrchardCore.Contents",
               pattern: _adminOptions.AdminUrlPrefix + "/AuditTrail/Content/{auditTrailEventId}",
               defaults: new { controller = contentControllerName, action = nameof(ContentController.Restore) }
           );
        }
    }
}