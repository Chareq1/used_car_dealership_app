using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using used_car_dealership_app.Models;

namespace used_car_dealership_app.Repositories;

public class UserRepository
{
    private readonly Database.Database database;

    public UserRepository()
    {
        database = new Database.Database();
    }
    
    public DataTable GetAllUsers()
    {
        return database.GetAll<User>("users");
    }
    
    public DataRow GetUserById(Guid id)
    {
        return database.GetById<User>("users", "userId", id);
    }
    
    public void DeleteUser(Guid id)
    {
        database.Delete<User>("users", "userId", id);
    }
    
    public void AddUser(User user)
    {
        var data = new Dictionary<string, object>
        {
            { "userId", user.UserId },
            { "username", user.Username},
            { "password", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.Password)) },
            { "type", user.Type.ToString() },
            { "name", user.Name },
            { "surname", user.Surname },
            { "pesel", user.PESEL },
            { "phone", user.Phone },
            { "email", user.Email },
            { "street", user.Street },
            { "city", user.City },
            { "zipCode", user.ZipCode },
            { "houseNumber", user.HouseNumber }
        };

        database.Insert<User>("users", data);
    }
    
    public void UpdateUser(User user)
    {
        var data = new Dictionary<string, object>
        {
            { "username", user.Username },
            { "password", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.Password)) },
            { "type", user.Type.ToString()},
            { "name", user.Name },
            { "surname", user.Surname },
            { "pesel", user.PESEL },
            { "phone", user.Phone },
            { "email", user.Email },
            { "street", user.Street },
            { "city", user.City },
            { "zipCode", user.ZipCode },
            { "houseNumber", user.HouseNumber }
        };
        
        database.Update<User>("users", data, "userId", user.UserId);
    }
    
    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        return database.ExecuteQuery(query, parameters);
    }
}