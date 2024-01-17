using OrderFoodApplication.Models;
using System.Security.Claims;

namespace OrderFoodApplication.Repository
{
    public interface IData
    {
        Task<ApplicationUser> GetUser(ClaimsPrincipal claims);
    }
}
