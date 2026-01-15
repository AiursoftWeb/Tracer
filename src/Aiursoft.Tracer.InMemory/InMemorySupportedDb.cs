using Aiursoft.DbTools;
using Aiursoft.DbTools.InMemory;
using Aiursoft.Tracer.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Tracer.InMemory;

public class InMemorySupportedDb : SupportedDatabaseType<TemplateDbContext>
{
    public override string DbType => "InMemory";

    public override IServiceCollection RegisterFunction(IServiceCollection services, string connectionString)
    {
        return services.AddAiurInMemoryDb<InMemoryContext>();
    }

    public override TemplateDbContext ContextResolver(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<InMemoryContext>();
    }
}
