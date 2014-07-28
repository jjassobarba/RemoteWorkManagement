using System.Web;
using System.Web.Optimization;

namespace RemoteWorkManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-1.10.2.min.js",
                        "~/Scripts/jquery/jquery-ui-1.10.3.full.min.js",
                        "~/Scripts/jquery/jquery-ui-1.10.3.custom.min.js",
                        "~/Scripts/jquery/jquery-ui-1.10.3.custom.min.js",
                        "~/Scripts/jquery/chosen.jquery.min.js",
                        "~/Scripts/jquery/select2.min.js",
                        "~/Scripts/jquery/typeahead-bs2.min.js",
                        "~/Scripts/jquery/jquery.autosize.min.js",
                        "~/Scripts/jquery/jquery.inputlimiter.1.3.1.min.js",
                        "~/Scripts/jquery/jquery.dataTables.bootstrap.js",
                        "~/Scripts/jquery/jquery.dataTables.min.js",
                        "~/Scripts/jquery/jquery.hotkeys.min.js",
                        "~/Scripts/jquery/jquery.ui.touch-punch.min.js",
                        "~/Scripts/jquery/jquery.slimscroll.min.js",
                        "~/Scripts/jquery/jquery.maskedinput.min.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/mainScripts").Include(
                        "~/Scripts/scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/angular/angular.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap/bootstrap.min.js",
                      "~/Scripts/bootstrap/bootstrap-tag.min.js",
                      "~/Scripts/bootstrap/bootstrap.wysiwyg.min.js",
                      "~/Scripts/bootstrap/ace-extra.min.js",
                      "~/Scripts/bootstrap/ace-elements.min.js",
                      "~/Scripts/bootstrap/ace.min.js",
                      "~/Scripts/respond/respond.js"));            

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
