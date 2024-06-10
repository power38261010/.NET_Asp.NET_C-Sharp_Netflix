
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTO;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;


namespace NetflixClone.Services
{
    public class PayService : IPayService
    {
        private readonly ApplicationDbContext _context;
        private readonly float _annualMultiplier;
        private readonly float _interestRateMultiplier;


        public PayService(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (!float.TryParse(configuration["PaymentValues:Annual"], out _annualMultiplier))
            {
                throw new InvalidOperationException("Invalid annual multiplier configuration value.");
            }

            if (!float.TryParse(configuration["PaymentValues:WithInterestRate"], out _interestRateMultiplier))
            {
                throw new InvalidOperationException("Invalid interest rate multiplier configuration value.");
            }
        }

        public async Task<List<Pay>?> GetAll()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<Pay?> GetById(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task Create(Pay pay)
        {
            _context.Payments.Add(pay);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(Pay pay)
        {
            _context.Entry(pay).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var pay = await _context.Payments.FindAsync(id);
            if (pay != null)
            {
                _context.Payments.Remove(pay);
                await _context.SaveChangesAsync();
            }
        }
        public float CalculateAnnualPayment( float monthlyPayment ) {
            return monthlyPayment * _annualMultiplier;
        }

        public float CalculateInterestRatePayment ( float monthlyPayment ) {
            return monthlyPayment * _interestRateMultiplier;
        }
        public async Task<PayDto?> TypesPayToArg () {
            var pay = await _context.Payments.FirstOrDefaultAsync(u => u.Currency == "ARS");

            return new PayDto {Currency = pay.Currency,
                                MonthlyPayment = pay.MonthlyPayment,
                                SubscriptionId = pay.SubscriptionId,
                                AnnualMultiplierPayment = CalculateAnnualPayment (pay.MonthlyPayment),
                                InterestMonthlyPayment = CalculateInterestRatePayment (pay.MonthlyPayment)
            };
        }
    }
}
