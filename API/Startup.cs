using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Application;

namespace API
{
    public partial class Startup : BusinessStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env) : base(configuration, env)
        {
        }
    }
}
