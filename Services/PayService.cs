
using Microsoft.EntityFrameworkCore;
using NetflixClone.Controllers.ModelRequest;
using NetflixClone.Data;
using NetflixClone.DTO;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;

namespace NetflixClone.Services {
    public class PayService : IPayService {
        private readonly ApplicationDbContext _context;
        private readonly decimal _annualMultiplier;
        private readonly decimal _interestRateMultiplier;

        public PayService(IConfiguration configuration, ApplicationDbContext context) {
            _context = context;

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (!decimal.TryParse(configuration["PaymentValues:Annual"], out _annualMultiplier)) {
                throw new InvalidOperationException("Invalid annual multiplier configuration value.");
            }

            if (!decimal.TryParse(configuration["PaymentValues:WithInterestRate"], out _interestRateMultiplier)) {
                throw new InvalidOperationException("Invalid interest rate multiplier configuration value.");
            }
            MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];
        }
        public async Task <Payment> CreatePaySubscription(decimal amount, string token, string description, string paymentMethodId, string payerEmail, int PayId, int UserId, bool IsAnual = false) {
            var paymentRequest = new PaymentCreateRequest {
                TransactionAmount = amount,
                Token = token,
                Description = description,
                PaymentMethodId = paymentMethodId,
                Installments = 1,
                Payer = new PaymentPayerRequest {
                    Email = payerEmail
                }
            };
            var dateNow = DateTime.Now;
            var client = new PaymentClient();
            var response = await client.CreateAsync(paymentRequest);
            var paymentSubscription = new PaySubscription {
                IsAnual = IsAnual,
                PayerEmail = payerEmail,
                Token = token,
                Description = description,
                status = response.Status,
                PaidDate = dateNow,
                Amount = amount,
                PayId = PayId,
                UserId = UserId
            };
            if ( response.Status == "approved") {
                var pay = await GetById(PayId);
                var user = await _context.Users.FindAsync(UserId);
                user.SubscriptionId = pay.SubscriptionId;
                user.IsPaid = true;
                user.ExpirationDate = DateTime.UtcNow.AddMonths(1);
                if (IsAnual) user.ExpirationDate = DateTime.UtcNow.AddMonths(12);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            _context.PaySubscriptions.Add(paymentSubscription);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<IEnumerable<Pay>?> GetAll() {
            return await _context.Payments.ToListAsync();
        }

        public async Task<IEnumerable<PaySubscription>?> GetAllPaySubscription() {
            return await _context.PaySubscriptions.ToListAsync();
        }

        public async Task<Pay?> GetById(int id) {
            return await _context.Payments.FindAsync(id);
        }

        public async Task Create(PayRequest pay) {
            var payAux = new Pay {Currency = pay.Currency,
                                    MonthlyPayment = pay.MonthlyPayment,
                                    SubscriptionId = pay.SubscriptionId};

            _context.Payments.Add(payAux);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(PayRequest pay) {
            var payAux = await _context.Payments.FirstOrDefaultAsync(p => p.Id == pay.Id);
            if (payAux != null) {
                payAux.Currency = pay.Currency ?? payAux.Currency;
                if (pay.MonthlyPayment != 0) payAux.MonthlyPayment = pay.MonthlyPayment;
                if (pay.SubscriptionId != 0) payAux.SubscriptionId = payAux.SubscriptionId;
                _context.Entry(payAux).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

        }

        public async Task Delete(int id) {
            var pay = await _context.Payments.FindAsync(id);
            if (pay != null) {
                _context.Payments.Remove(pay);
                await _context.SaveChangesAsync();
            }
        }
        public decimal CalculateAnnualPayment( decimal monthlyPayment ) {
            return monthlyPayment * _annualMultiplier;
        }

        public decimal CalculateInterestRatePayment ( decimal monthlyPayment ) {
            return monthlyPayment * _interestRateMultiplier;
        }
        public async Task<IEnumerable<PayDto>> TypesPayToArg() {
            var payments = await _context.Payments
                                            .Where(u => u.Currency == "ARS")
                                            .ToListAsync();

            var payDtos = payments.Select(pay => new PayDto {
                Currency = pay.Currency,
                MonthlyPayment = pay.MonthlyPayment,
                SubscriptionId = pay.SubscriptionId,
                AnnualMultiplierPayment = CalculateAnnualPayment(pay.MonthlyPayment),
                InterestMonthlyPayment = CalculateInterestRatePayment(pay.MonthlyPayment)
            });

            return payDtos;
        }

    }
}



