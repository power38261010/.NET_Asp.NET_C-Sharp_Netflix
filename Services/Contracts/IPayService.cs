using System.Security.Claims;
using BussinessLayer.Services.Contracts;
using NetflixClone.DTO;
using NetflixClone.Models;

namespace NetflixClone.Services.Contracts
{
    public interface IPayService : IBaseService <Pay>
    {
        float CalculateAnnualPayment(float monthlyPayment);
        float CalculateInterestRatePayment(float monthlyPayment);
        Task<PayDto?> TypesPayToArg ();

    }
}
