using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;

namespace NZWalks.API.Infrastructure
{
    public static class DeploymentInitializationExtensions
    {
        public static async Task InitializeDeploymentAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILoggerFactory>()
                .CreateLogger("DeploymentInitialization");

            if (configuration.GetValue<bool>("Deployment:ApplyMigrationsOnStartup"))
            {
                await ApplyMigrationsAsync<NZWalksDbContext>(services, logger);
                await ApplyMigrationsAsync<NZWalksAuthDbContext>(services, logger);
            }

            if (configuration.GetValue<bool>("BootstrapAdmin:Enabled"))
            {
                await EnsureRolesAsync(services, logger);
                await BootstrapWriterAsync(services, configuration, logger);
            }
        }

        private static async Task ApplyMigrationsAsync<TContext>(
            IServiceProvider services,
            ILogger logger)
            where TContext : DbContext
        {
            var dbContext = services.GetRequiredService<TContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation(
                    "Applying {MigrationCount} pending migrations for {DbContext}.",
                    pendingMigrations.Count(),
                    typeof(TContext).Name);

                await dbContext.Database.MigrateAsync();
            }
        }

        private static async Task EnsureRolesAsync(IServiceProvider services, ILogger logger)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Reader", "Writer" };

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role))
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Failed to create role '{role}': {errors}");
                }

                logger.LogInformation("Created missing role {Role}.", role);
            }
        }

        private static async Task BootstrapWriterAsync(
            IServiceProvider services,
            IConfiguration configuration,
            ILogger logger)
        {
            var email = configuration["BootstrapAdmin:Email"];
            var password = configuration["BootstrapAdmin:Password"];

            if (string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(password)
                || password.Contains("CHANGE-ME", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning(
                    "BootstrapAdmin is enabled, but Email/Password are missing or still contain placeholder values.");
                return;
            }

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                    throw new InvalidOperationException(
                        $"Failed to create bootstrap writer user '{email}': {errors}");
                }

                logger.LogInformation("Created bootstrap writer user {Email}.", email);
            }

            await EnsureUserRoleAsync(userManager, user, "Reader");
            await EnsureUserRoleAsync(userManager, user, "Writer");
        }

        private static async Task EnsureUserRoleAsync(
            UserManager<IdentityUser> userManager,
            IdentityUser user,
            string role)
        {
            if (await userManager.IsInRoleAsync(user, role))
            {
                return;
            }

            var roleResult = await userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException(
                    $"Failed to add user '{user.Email}' to role '{role}': {errors}");
            }
        }
    }
}
