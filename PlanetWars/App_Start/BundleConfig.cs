using System.Web;
using System.Web.Optimization;

namespace PlanetWars
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/core")
                        .Include(
                            "~/Scripts/jquery-{version}.js",
                            "~/Scripts/bootstrap.js",
                            "~/Scripts/respond.js")
                        .IncludeDirectory("~/Scripts", "*.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/head").Include(
                        "~/Scripts/head/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
