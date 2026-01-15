using System.Diagnostics.CodeAnalysis;
using Aiursoft.Tracer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Tracer.MySql;

[ExcludeFromCodeCoverage]

public class MySqlContext(DbContextOptions<MySqlContext> options) : TemplateDbContext(options);
