using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;

namespace NetflixClone.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Subscription>?> GetAll() {
            return await _context.Subscriptions.ToListAsync();
        }

        public async Task<Subscription?> GetById(int id) {
            return await _context.Subscriptions.FindAsync(id);
        }

        public async Task Create(SubscriptionRequest subscription) {
            var sub = new Subscription {Type = subscription.Type};
            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(SubscriptionRequest subscription) {
            var sub = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == subscription.Id);
            if (sub != null) {
                sub.Type = subscription.Type ?? sub.Type;
                _context.Entry(sub).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id) {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null) {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }

    }
}
