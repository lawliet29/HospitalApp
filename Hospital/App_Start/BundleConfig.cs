using System.Web;
using System.Web.Optimization;

namespace Hospital
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-multiselect.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js",
                      "~/Scripts/prettify.js",
                      "~/Scripts/bootstrap-table-pagination.js",
                      "~/Scripts/select2.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/bootstrap-datetimepicker.min.css",
                      "~/Content/prettify.css",
                      "~/Content/font-awesome.css",
                      "~/Content/select2-bootstrap.css",
                      "~/Content/css/select2.css",
                      // "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css",
                      "~/Content/site.css"));
        }
    }
}
