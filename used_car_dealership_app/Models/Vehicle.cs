using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Avalonia.Media.Imaging;

namespace used_car_dealership_app.Models;

public enum VehicleType
{
    Samochód,
    Motocykl,
    Dostawczy,
    Ciągnik,
    Autobus,
    Ciężarówka,
    Naczepa
}

public class Vehicle : SpecificVehicleValues
{
    public Guid VehicleId { get; set; }
    public String Brand { get; set; }
    public String Model { get; set; }
    public VehicleType Type { get; set; }
    public String BodyType { get; set; }
    public int ProductionYear { get; set; }
    public String ProductionCountry { get; set; }
    public DateTime FirstRegistrationDate { get; set; }
    public String OriginCountry { get; set; }
    public int Mileage { get; set; }
    public int Doors { get; set; }
    public String Color { get; set; }
    public String Transmission { get; set; }
    public String VIN { get; set; }
    public String Description { get; set; }
    public String Drive { get; set; }
    public decimal Price { get; set; }
    public String EngineType { get; set; }
    public Decimal? Consumption { get; set; }
    public Location Location { get; set; }
    
    
    public List<Image> Images { get; set; }
    public List<Equipment> Equipment { get; set; }
    
    
    public String VehicleFullName => $"{Brand} {Model}";
    public String LocationNameAddress => $"{Location.Name}, ul. {Location.Street} {Location.HouseNumber}, {Location.ZipCode} {Location.City}";
    
    
    public Image FirstImage
    {
        get => Images.Count > 0 ? Images[0] : null;
        set
        {
            if (Images.Count > 0)
            {
                Images[0] = value;
            }
            else
            {
                Images.Add(value);
            }
        }
    }
    
    
    public bool IsFirstImage => FirstImage != null;


    public override string ToString()
    {
        return $"('{VehicleId}', '{Brand}', '{Model}', '{Type}', '{BodyType}', '{ProductionYear}', '{ProductionCountry}', '{FirstRegistrationDate}', '{OriginCountry}', '{Mileage}', '{Doors}', '{Color}', '{Transmission}', '{VIN}', '{Description}', '{Drive}', '{Price}', '{EngineType}', '{BatterySize}', '{ElectricEnginePower}', '{Consumption}', '{FuelType}', '{EngineSize}', '{Power}', '{Co2Emission}', '{Location.LocationId}')";
    }
}