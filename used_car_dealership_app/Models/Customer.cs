using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA KLIENTA
public class Customer
{
    //POLA
    public Guid CustomerId { get; set; }
    public String Name { get; set; }
    public String Surname { get; set; }
    public String PESEL { get; set; }
    public String IdCardNumber { get; set; }
    public String Phone { get; set; }
    public String Email { get; set; }
    public String Street { get; set; }
    public String City { get; set; }
    public String ZipCode { get; set; }
    public String HouseNumber { get; set; }

    
    //POLE POMOCNICZE DO WYŚWIETLANIA PEŁNEGO IMIENIA KLIENTA
    public String CustomerFullName => $"{Name} {Surname}";
    
    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{CustomerId}', '{Name}', '{Surname}', '{PESEL}', '{IdCardNumber}', '{Phone}', '{Email}', '{Street}', '{City}', '{ZipCode}', '{HouseNumber}')";
    }
}