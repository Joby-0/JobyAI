using Microsoft.EntityFrameworkCore;

namespace JobyAI.Api.Data;

public sealed class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
}
