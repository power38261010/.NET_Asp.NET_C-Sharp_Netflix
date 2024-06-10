using NetflixClone.Data;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
namespace NetflixClone.Cron
{
    public class CheckUserExpirationsCron {
        private readonly IServiceProvider _serviceProvider;

        public CheckUserExpirationsCron(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void CheckUserExpirations()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var users = context.Users.Where(u => u.ExpirationDate < DateTime.UtcNow).ToList();

                foreach (var user in users)
                {
                    user.IsPaid = false;
                }

                context.SaveChanges();
            }
        }
    }
}