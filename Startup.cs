using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SqlDependency_Test.Startup))]
namespace SqlDependency_Test
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
