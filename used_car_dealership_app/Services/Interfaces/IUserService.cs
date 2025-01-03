using System;
using System.Threading.Tasks;
using used_car_dealership_app.Models;

namespace used_car_dealership_app.Services.Interfaces;

//INTERFEJS DLA USŁUGI UŻYTKOWNIKA
public interface IUserService
{
    //METODY
    Task<User> AuthenticateUserAsync(String username, String password);
}