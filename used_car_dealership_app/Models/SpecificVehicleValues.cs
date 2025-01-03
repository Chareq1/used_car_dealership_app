using System;
using used_car_dealership_app.Models.Interfaces;

namespace used_car_dealership_app.Models;

public abstract class SpecificVehicleValues: IElectricVehicle, IFuelVehicle
{
    public Decimal? BatterySize { get; set; }
    public Decimal? ElectricEnginePower { get; set; }
    public String? FuelType { get; set; }
    public int? EngineSize { get; set; }
    public int? Power { get; set; }
    public String? Co2Emission { get; set; }
}