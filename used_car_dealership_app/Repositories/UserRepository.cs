using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

//KLASA REPOZYTORIUM DLA UŻYTKOWNIKÓW
public class UserRepository
{
    //POLE DLA USŁUGI BAZY DANYCH
    private readonly DatabaseService _databaseService;

    
    //KONSTRUKTOR
    public UserRepository()
    {
        _databaseService = new DatabaseService();
    }
    
    
    //METODY
    //Metoda zwracająca wszystkich użytkowników
    public DataTable GetAllUsers()
    {
        return _databaseService.GetAll<User>("users");
    }
    
    //Metoda zwracająca użytkownika po identyfikatorze
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
    
    //Metoda usuwająca użytkownika
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
    
    //Metoda dodająca użytkownika
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
    
    //Metoda aktualizująca użytkownika
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
    
    //Metoda wykonująca zapytanie
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