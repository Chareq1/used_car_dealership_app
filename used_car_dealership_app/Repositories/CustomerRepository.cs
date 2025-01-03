using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

//KLASA REPOZYTORIUM DLA KLIENTÓW
public class CustomerRepository
{
    //POLE DLA USŁUGI BAZY DANYCH
    private readonly DatabaseService _databaseService;

    
    //KONSTRUKTOR
    public CustomerRepository()
    {
        _databaseService = new DatabaseService();
    }
    
    
    //METODY
    //Metoda zwracająca wszystkich klientów
    public DataTable GetAllCustomers()
    {
        return _databaseService.GetAll<Customer>("customers");
    }
    
    //Metoda zwracająca klienta po identyfikatorze
    public DataRow GetCustomerById(Guid id)
    {
        try
        {
            return _databaseService.GetById<Customer>("customers", "customerId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda usuwająca klienta
    public void DeleteCustomer(Guid id)
    {
        try
        {
            _databaseService.Delete<Customer>("customers", "customerId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda dodająca klienta
    public void AddCustomer(Customer customer)
    {
        try
        {
            var data = new Dictionary<String, object>
            {
                { "customerId", customer.CustomerId },
                { "name", customer.Name },
                { "surname", customer.Surname },
                { "pesel", customer.PESEL },
                { "idCardNumber", customer.IdCardNumber },
                { "phone", customer.Phone },
                { "email", customer.Email },
                { "street", customer.Street },
                { "city", customer.City },
                { "zipCode", customer.ZipCode },
                { "houseNumber", customer.HouseNumber }
            };

            _databaseService.Insert<Customer>("customers", data);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda aktualizująca klienta
    public void UpdateCustomer(Customer customer)
    {
        try
        {
            var data = new Dictionary<String, object>
            {
                { "name", customer.Name },
                { "surname", customer.Surname },
                { "pesel", customer.PESEL },
                { "idCardNumber", customer.IdCardNumber },
                { "phone", customer.Phone },
                { "email", customer.Email },
                { "street", customer.Street },
                { "city", customer.City },
                { "zipCode", customer.ZipCode },
                { "houseNumber", customer.HouseNumber }
            };

            _databaseService.Update<Customer>("customers", data, "customerId", customer.CustomerId);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda wykonująca zapytanie
    public DataTable ExecuteQuery(String query, List<NpgsqlParameter> parameters)
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