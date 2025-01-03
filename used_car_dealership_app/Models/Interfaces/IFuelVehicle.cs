using System;

namespace used_car_dealership_app.Models.Interfaces;

//INTERFEJS Z POLAMI DLA POJAZDÓW Z SILNIKAMI SPALINOWYMI
public interface IFuelVehicle
{
    public String? FuelType { get; set; }
    public int? EngineSize { get; set; }
    public int? Power { get; set; }
    public String? Co2Emission { get; set; }
}