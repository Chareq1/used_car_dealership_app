using System.Threading.Tasks;
using used_car_dealership_app.Models;

namespace used_car_dealership_app.Services.Interfaces;

public interface IUserService
{
    Task<User> AuthenticateUserAsync(string username, string password);
}