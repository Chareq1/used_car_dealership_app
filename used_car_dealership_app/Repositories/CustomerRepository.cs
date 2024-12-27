using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Database;

namespace used_car_dealership_app.Repositories;

public class CustomerRepository
{
    private readonly Database.Database database;

    public CustomerRepository()
    {
        database = new Database.Database();
    }
    
    public DataTable GetAllCustomers()
    {
        return database.GetAll<Customer>("customers");
    }
    
    public DataRow GetCustomerById(Guid id)
    {
        return database.GetById<Customer>("customers", "customerId", id);
    }
    
    public void DeleteCustomer(Guid id)
    {
        database.Delete<Customer>("customers", "customerId", id);
    }
    
    public void AddCustomer(Customer customer)
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

        database.Insert<Customer>("customers", data);
    }
    
    public void UpdateCustomer(Customer customer)
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

        database.Update<Customer>("customers", data, "customerId", customer.CustomerId);
    }
    
    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        return database.ExecuteQuery(query, parameters);
    }
}