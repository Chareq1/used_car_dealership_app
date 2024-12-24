using System;

namespace used_car_dealership_app.Models;

public class Customer
{
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

    public override string ToString()
    {
        return $"('{CustomerId}', '{Name}', '{Surname}', '{PESEL}', '{Phone}', '{Email}', '{Street}', '{City}', '{ZipCode}', '{HouseNumber}')";
    }
}