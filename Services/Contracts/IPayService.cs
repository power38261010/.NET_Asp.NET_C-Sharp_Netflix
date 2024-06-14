using System.Security.Claims;
using BussinessLayer.Services.Contracts;
using NetflixClone.Controllers.ModelRequest;
using NetflixClone.DTO;
using NetflixClone.Models;
// using MercadoPago.Client.Payment;
// using MercadoPago.Config;
using MercadoPago.Resource.Payment;

namespace NetflixClone.Services.Contracts
{
    public interface IPayService : IBaseService <Pay, PayRequest>
    {
        decimal CalculateAnnualPayment(decimal MonthlyPayment);
        decimal CalculateInterestRatePayment(decimal MonthlyPayment);
        Task<IEnumerable<PayDto>> TypesPayToArg ();
        Task<Payment> CreatePaySubscription(decimal amount, string token, string description, string paymentMethodId, string payerEmail, int PayId, int UserId, bool IsAnual = false);
        Task<IEnumerable<PaySubscription>?> GetAllPaySubscription();
    }
}
