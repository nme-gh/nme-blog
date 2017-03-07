using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace nme_blog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /*
             * If the slug is implemented in BlogPosts Controller for Create Action (HttpPost) and Details Action (HttpGet), 
             * but the new route is not integrated as seen below here (name: "NewSlug"), 
             * the URL still works by following the Default route (name: "Default") E.g. root/BlogPosts/Details?slug=blog7
             * However the slug doesn't show in the URL as intended until the new route is added. 
             */
            // root/Blog/blog7
            routes.MapRoute(
                name: "NewSlug",
                url: "Blog/{slug}",
                defaults: new { controller = "BlogPosts", action = "Details", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "NewSlugEdit",
                url: "BlogEdit/{slug}",
                defaults: new { controller = "BlogPosts", action = "Edit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "NewSlugDelete",
                url: "BlogDel/{slug}",
                defaults: new { controller = "BlogPosts", action = "Delete", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "BlogPosts", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
