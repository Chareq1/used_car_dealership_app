using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

public class EquipmentRepository
{
    private readonly DatabaseService _databaseService;

    public EquipmentRepository()
    {
        _databaseService = new DatabaseService();
    }

    public DataTable GetAllEquipment()
    {
        return _databaseService.GetAll<Equipment>("equipments");
    }
}