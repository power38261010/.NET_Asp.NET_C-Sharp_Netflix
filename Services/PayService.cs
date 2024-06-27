
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

                int diffDays = 0;
                var today = DateTime.UtcNow;
                var expDate = user.ExpirationDate;

                if (user.IsPaid != null && (bool)user.IsPaid) {
                    var diffTime = expDate - today;
                    diffDays = diffTime.Value.Days;

                    if (user.SubscriptionId != pay.SubscriptionId) {
                        if (pay.SubscriptionId == 2) {
                            diffDays /= 2;
                        } else if (pay.SubscriptionId == 1) {
                            diffDays *= 2;
                        }
                    }
                }

                user.SubscriptionId = pay.SubscriptionId;
                user.IsPaid = true;
                var newExpirationDate = today.AddDays(diffDays).AddMonths(1);
                if (IsAnual) {
                    newExpirationDate = newExpirationDate.AddMonths(11); // Añadir 11 meses más si es anual
                }
                user.ExpirationDate = newExpirationDate;
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
            return await _context.Payments.Include(p => p.Subscription).FirstOrDefaultAsync(pay => pay.Id == id);
        }


        public async Task<Pay> Create(PayRequest pay) {
            var payAux = new Pay {
                Currency = pay.Currency,
                MonthlyPayment = pay.MonthlyPayment,
                SubscriptionId = pay.SubscriptionId
            };

            _context.Payments.Add(payAux);
            await _context.SaveChangesAsync();
            return payAux;
        }


        public async Task Edit(PayRequest pay) {
            var payAux = await _context.Payments.FirstOrDefaultAsync(p => p.Id == pay.Id);
            if (payAux != null) {
                payAux.Currency = pay.Currency ?? payAux.Currency;
                if (pay.MonthlyPayment != 0) payAux.MonthlyPayment = pay.MonthlyPayment;

                if (pay.SubscriptionId != 0) {
                    var subscriptionExists = await _context.Subscriptions.AnyAsync(s => s.Id == pay.SubscriptionId);
                    if (subscriptionExists) {
                        payAux.SubscriptionId = pay.SubscriptionId;
                    } else {
                        throw new Exception("El SubscriptionId proporcionado no existe.");
                    }
                }
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
                                            .Include(p => p.Subscription)
                                            .Where(u => u.Currency == "ARS")
                                            .ToListAsync();

            var payDtos = payments.Select(pay => new PayDto {
                Id= pay.Id,
                Currency = pay.Currency,
                MonthlyPayment = pay.MonthlyPayment,
                SubscriptionId = pay.SubscriptionId,
                Subscription = pay.Subscription,
                AnnualMultiplierPayment = CalculateAnnualPayment(pay.MonthlyPayment),
                InterestMonthlyPayment = CalculateInterestRatePayment(pay.MonthlyPayment)
            });

            return payDtos;
        }

    }
}



