using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA KLIENTA
public class Customer
{
    //POLA
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PESEL { get; set; }
    public string IdCardNumber { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string HouseNumber { get; set; }

    
    //POLE POMOCNICZE DO WYŚWIETLANIA PEŁNEGO IMIENIA KLIENTA
    public string CustomerFullName => $"{Name} {Surname}";
    
    
    //NADPISANIE METODY ToString
    public override string ToString()
    {
        return $"('{CustomerId}', '{Name}', '{Surname}', '{PESEL}', '{IdCardNumber}', '{Phone}', '{Email}', '{Street}', '{City}', '{ZipCode}', '{HouseNumber}')";
    }
}