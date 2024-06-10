using System.Collections.Generic;
using System.Threading.Tasks;
using BussinessLayer.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;

namespace NetflixClone.Services.Contracts
{
    public interface ISubscriptionService  : IBaseService <Subscription>
    {
    }
}
