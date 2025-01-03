using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

public class UserRepository
{
    private readonly DatabaseService _databaseService;

    public UserRepository()
    {
        _databaseService = new DatabaseService();
    }
    
    public DataTable GetAllUsers()
    {
        return _databaseService.GetAll<User>("users");
    }
    
    public DataRow GetUserById(Guid id)
    {
        try
        {
            return _databaseService.GetById<User>("users", "userId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public void DeleteUser(Guid id)
    {
        try
        {
            _databaseService.Delete<User>("users", "userId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public void AddUser(User user)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { "userId", user.UserId },
                { "username", user.Username },
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

            _databaseService.Insert<User>("users", data);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public void UpdateUser(User user)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                {"userId", user.UserId},
                { "username", user.Username },
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

            _databaseService.Update<User>("users", data, "userId", user.UserId);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        try
        {
            return _databaseService.ExecuteQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}