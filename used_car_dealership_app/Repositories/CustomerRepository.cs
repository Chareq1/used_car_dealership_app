using System;
using System.Collections.Generic;
using System.Data;
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
}