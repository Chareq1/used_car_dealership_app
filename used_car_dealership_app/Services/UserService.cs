using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Services.Interfaces;

namespace used_car_dealership_app.Services;

//KLASA DLA USŁUGI UŻYTKOWNIKA
public class UserService
{
    //METODY
    //Metoda do autoryzacji użytkownika
    public async Task<User> AuthenticateUser(string username, string password)
    {
        password = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        
        var _userRepository = new UserRepository();
        var query = $"SELECT * FROM users WHERE \"username\" = @username AND \"password\" = @password ;";
        var parameters = new List<NpgsqlParameter>();
        
        parameters.Add(new NpgsqlParameter("@username", $"{username}"));
        parameters.Add(new NpgsqlParameter("@password", $"{password}"));
        
        var dataTable = await Task.Run(() => _userRepository.ExecuteQuery(query, parameters));
        
        if (dataTable.Rows.Count == 0)
        {
            return null;
        }
        else
        {
            var users = dataTable.AsEnumerable().Select(row => new User
            {
                UserId = Guid.Parse(row["userId"].ToString()),
                Password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(row["password"].ToString())),
                Username = row["username"].ToString(),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                PESEL = row["pesel"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString(),
                Street = row["street"].ToString(),
                City = row["city"].ToString(),
                ZipCode = row["zipCode"].ToString(),
                HouseNumber = row["houseNumber"].ToString(),
                Type = Enum.TryParse(row["type"].ToString(), out UserType userType) ? userType : UserType.WORKER
            }).ToList();
            
            return users[0];
        }
    }
}