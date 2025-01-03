using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA LOKALIZACJĘ
public class Location
{
    //POLA
    public Guid LocationId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string HouseNumber { get; set; }

    
    //NADPISANIE METODY ToString
    public override string ToString()
    {
        return $"('{LocationId}', '{Name}', '{Phone}', '{Email}', '{Street}', '{City}', '{ZipCode}', '{HouseNumber}')";
    }
}