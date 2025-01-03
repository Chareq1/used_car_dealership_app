using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

public class CustomerRepository
{
    private readonly DatabaseService _databaseService;

    public CustomerRepository()
    {
        _databaseService = new DatabaseService();
    }
    
    public DataTable GetAllCustomers()
    {
        return _databaseService.GetAll<Customer>("customers");
    }
    
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
    
    public void AddCustomer(Customer customer)
    {
        try
        {
            var data = new Dictionary<string, object>
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
    
    public void UpdateCustomer(Customer customer)
    {
        try
        {
            var data = new Dictionary<string, object>
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