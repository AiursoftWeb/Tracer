using Aiursoft.Tracer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Tracer.MySql;

public class MySqlContext(DbContextOptions<MySqlContext> options) : TemplateDbContext(options);
