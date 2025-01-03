using System;

namespace used_car_dealership_app.Models.Interfaces;

//INTERFEJS Z POLAMI DLA POJAZDÓW ELEKTRYCZNYCH
public interface IElectricVehicle
{
    public Decimal? BatterySize { get; set; }
    public Decimal? ElectricEnginePower { get; set; }
}