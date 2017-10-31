using System.Web;
using System.Web.Optimization;

namespace ManagerDesk
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Assets/Scripts/libs/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Assets/Scripts/libs/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Assets/Scripts/libs/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Assets/Scripts/libs/bootstrap.js",
                      "~/Assets/Scripts/libs/Sortable.min.js",
                      "~/Assets/Scripts/libs/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Assets/Content/bootstrap.css",
                      "~/Assets/Content/font-awesome-4.7.0/css/font-awesome.min.css",
                      "~/Assets/Content/config.css",
                      "~/Assets/Content/main.css",
                      "~/Assets/Content/animate.css",
                      "~/Assets/Content/dialog.css",
                      "~/Assets/Content/media.css").Include("~/Assets/Content/font-awesome-4.7.0/css/font-awesome.min.css", new CssRewriteUrlTransform()));
        }
    }
}
