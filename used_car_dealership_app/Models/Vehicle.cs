using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Avalonia.Media.Imaging;

namespace used_car_dealership_app.Models;

//TYP WYLICZENIOWY DLA TYPÓW POJAZDU
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

//KLASA REPREZENTUJĄCA POJAZD
public class Vehicle : SpecificVehicleValues
{
    //POLA
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
    
    
    //LISTY ZE ZDJĘCIAMI I WYPOSAŻENIEM
    public List<Image> Images { get; set; }
    public List<Equipment> Equipment { get; set; }
    
    
    //POLA WSPOMAGAJĄCE WYŚWIETLANIE INFORMACJI
    public String VehicleFullName => $"{Brand} {Model}";
    public String FullNameVin => $"{Brand} {Model} ({VIN})";
    public String LocationNameAddress => $"{Location.Name}, ul. {Location.Street} {Location.HouseNumber}, {Location.ZipCode} {Location.City}";
    
    
    //POLE POMOCNICZE DO WYŚWIETLANIA PIERWSZEGO OBRAZU POJAZDU W FORMULARZU
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
    
    
    //POLE POMOCNICZE DO SPRAWDZANIA CZY POJAZD MA ZDJĘCIE
    public bool IsFirstImage => FirstImage != null;
    
    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{VehicleId}', '{Brand}', '{Model}', '{Type}', '{BodyType}', '{ProductionYear}', '{ProductionCountry}', '{FirstRegistrationDate}', '{OriginCountry}', '{Mileage}', '{Doors}', '{Color}', '{Transmission}', '{VIN}', '{Description}', '{Drive}', '{Price}', '{EngineType}', '{Consumption}', '{Location.LocationId}')";
    }
}