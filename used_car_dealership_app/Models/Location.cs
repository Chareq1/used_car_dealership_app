using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA LOKALIZACJĘ
public class Location
{
    //POLA
    public Guid LocationId { get; set; }
    public String Name { get; set; }
    public String Phone { get; set; }
    public String Email { get; set; }
    public String Street { get; set; }
    public String City { get; set; }
    public String ZipCode { get; set; }
    public String HouseNumber { get; set; }

    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{LocationId}', '{Name}', '{Phone}', '{Email}', '{Street}', '{City}', '{ZipCode}', '{HouseNumber}')";
    }
}