using System;

namespace used_car_dealership_app.Models.Interfaces;

public interface IElectricVehicle
{
    public Decimal? BatterySize { get; set; }
    public Decimal? ElectricEnginePower { get; set; }
}