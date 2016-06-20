using ContactMe.App_Start;
using System;
using System.Web;
using System.Web.Routing;

namespace ContactMe
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutoMapperConfig.RegisterMappings();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Redirect the user to the error page.
            Server.ClearError();
            Response.Redirect("/Error/Index");
        }
    }
}