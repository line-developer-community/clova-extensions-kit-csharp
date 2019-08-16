using Microsoft.Azure.Functions.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(LineDC.CEK.Startup))]
namespace LineDC.CEK
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddClova<ILoggableClova, MySimpleClova>()      // Simple
                .AddClova<IDurableClova, MyDurableClova>();     // Durable Functions
        }
    }
}
