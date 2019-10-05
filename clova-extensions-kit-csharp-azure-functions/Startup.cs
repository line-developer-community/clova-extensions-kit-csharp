using LineDC.CEK.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(LineDC.CEK.Startup))]
namespace LineDC.CEK
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ClovaConfiguration { DefaultLang = Lang.En };

            builder.Services
                .AddSingleton(config)
                .AddClova<ILoggableClova, MySimpleClova>()  // Simple
                .AddClova<IDurableClova, MyDurableClova>(); // Durable Functions
        }
    }
}
