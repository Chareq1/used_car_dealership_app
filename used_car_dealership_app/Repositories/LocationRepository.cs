using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

public class LocationRepository
{
    private readonly DatabaseService _databaseService;

    public LocationRepository()
    {
        _databaseService = new DatabaseService();
    }
    
    public DataTable GetAllLocations()
    {
        return _databaseService.GetAll<Location>("locations");
    }
    
    public DataRow GetLocationById(Guid id)
    {
        try
        {
            return _databaseService.GetById<Location>("locations", "locationId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public void DeleteLocation(Guid id)
    {
        try
        {
            _databaseService.Delete<Location>("locations", "locationId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public void AddLocation(Location location)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { "locationId", location.LocationId },
                { "name", location.Name },
                { "phone", location.Phone },
                { "email", location.Email },
                { "street", location.Street },
                { "city", location.City },
                { "zipCode", location.ZipCode },
                { "houseNumber", location.HouseNumber }
            };

            _databaseService.Insert<Location>("locations", data);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public void UpdateLocation(Location location)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { "name", location.Name },
                { "phone", location.Phone },
                { "email", location.Email },
                { "street", location.Street },
                { "city", location.City },
                { "zipCode", location.ZipCode },
                { "houseNumber", location.HouseNumber }
            };

            _databaseService.Update<Location>("locations", data, "locationId", location.LocationId);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        try {
            return _databaseService.ExecuteQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}