using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

public class VehicleRepository
{
    private readonly DatabaseService _databaseService;

    public VehicleRepository()
    {
        _databaseService = new DatabaseService();
    }

    public DataTable GetAllVehicles()
    {
        return _databaseService.GetAll<Vehicle>("vehicles");
    }

    public DataRow GetVehicleById(Guid id)
    {
        try {
            return _databaseService.GetById<Vehicle>("vehicles", "vehicleId", id);
        } catch (Exception ex) {
            throw;
        }
    }

    public void DeleteVehicle(Guid id)
    {
        try {
            _databaseService.Delete<Vehicle>("vehicles", "vehicleId", id);
        } catch (Exception ex) {
            throw;
        }
    }

    public void DeleteEquipment(Guid id)
    {
        try{
            _databaseService.Delete<Vehicle>("equipmentList", "vehicleId", id);
        } catch (Exception ex) {
            throw;
        }
    }    
    
    public void AddVehicle(Vehicle vehicle)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { "vehicleId", vehicle.VehicleId },
                { "brand", vehicle.Brand },
                { "model", vehicle.Model },
                { "type", vehicle.Type.ToString() },
                { "bodyType", vehicle.BodyType },
                { "productionYear", vehicle.ProductionYear },
                { "productionCountry", vehicle.ProductionCountry },
                { "firstRegistrationDate", vehicle.FirstRegistrationDate },
                { "originCountry", vehicle.OriginCountry },
                { "mileage", vehicle.Mileage },
                { "doors", vehicle.Doors },
                { "color", vehicle.Color },
                { "transmission", vehicle.Transmission },
                { "VIN", vehicle.VIN },
                { "description", vehicle.Description },
                { "drive", vehicle.Drive },
                { "price", vehicle.Price },
                { "engineType", vehicle.EngineType },
                { "locationId", vehicle.Location.LocationId },
                { "batterySize", vehicle.BatterySize },
                { "electricEnginePower", vehicle.ElectricEnginePower },
                { "consumption", vehicle.Consumption },
                { "fuelType", vehicle.FuelType },
                { "engineSize", vehicle.EngineSize },
                { "power", vehicle.Power },
                { "co2Emission", vehicle.Co2Emission }
            };

            _databaseService.Insert<Vehicle>("vehicles", data);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { "brand", vehicle.Brand },
                { "model", vehicle.Model },
                { "type", vehicle.Type.ToString() },
                { "bodyType", vehicle.BodyType },
                { "productionYear", vehicle.ProductionYear },
                { "productionCountry", vehicle.ProductionCountry },
                { "firstRegistrationDate", vehicle.FirstRegistrationDate },
                { "originCountry", vehicle.OriginCountry },
                { "mileage", vehicle.Mileage },
                { "doors", vehicle.Doors },
                { "color", vehicle.Color },
                { "transmission", vehicle.Transmission },
                { "VIN", vehicle.VIN },
                { "description", vehicle.Description },
                { "drive", vehicle.Drive },
                { "price", vehicle.Price },
                { "engineType", vehicle.EngineType },
                { "locationId", vehicle.Location.LocationId },
                { "batterySize", vehicle.BatterySize },
                { "electricEnginePower", vehicle.ElectricEnginePower },
                { "consumption", vehicle.Consumption },
                { "fuelType", vehicle.FuelType },
                { "engineSize", vehicle.EngineSize },
                { "power", vehicle.Power },
                { "co2Emission", vehicle.Co2Emission }
            };

            _databaseService.Update<Vehicle>("vehicles", data, "vehicleId", vehicle.VehicleId);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public DataTable GetEquipment(Guid vehicleId)
    {
        try {
            var query = "SELECT \"equipments\".\"equipmentId\", \"equipments\".\"name\" FROM \"equipmentList\" JOIN \"equipments\" ON \"equipmentList\".\"equipmentId\" = \"equipments\".\"equipmentId\" WHERE \"equipmentList\".\"vehicleId\" = @vehicleId";
            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@vehicleId", vehicleId)
            };

            return _databaseService.ExecuteQuery(query, parameters);
        } catch (Exception ex) {
            throw;
        }
    }
    
    public void AddEquipment(Guid vehicleId, Guid equipmentId)
    {
        try {
            var data = new Dictionary<string, object>
            {
                { "vehicleId", vehicleId },
                { "equipmentId", equipmentId }
            };

            _databaseService.Insert<Vehicle>("equipmentList", data);
        } catch (Exception ex) {
            throw;
        }
    }
    
    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        try {
            return _databaseService.ExecuteQuery(query, parameters);
        } catch (Exception ex) {
            throw;
        }
    }
}