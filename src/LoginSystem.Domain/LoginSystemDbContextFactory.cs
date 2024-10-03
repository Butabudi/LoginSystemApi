using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LoginSystem.Domain;

internal class LoginSystemDbContextFactory : IDesignTimeDbContextFactory<LoginSystemDbContext>
{
    public LoginSystemDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LoginSystemDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=tcp:localhost,1433;Initial Catalog=Login_Z;Persist Security Info=true;Integrated Security=true;MultipleActiveResultSets=False;Encrypt=false;TrustServerCertificate=False;Connection Timeout=30;"
        );

        return new LoginSystemDbContext(optionsBuilder.Options);
    }
}
