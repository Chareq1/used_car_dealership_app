using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

//TYP WYMIENIENIOWY REPREZENTUJĄCY RODZAJ UŻYTKOWNIKA
public enum UserType
{
    ADMINISTRATOR,
    WORKER,
}


//KLASA REPREZENTUJĄCA UŻYTKOWNIKA
public class User
{
    //POLA
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public UserType Type { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PESEL { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string HouseNumber { get; set; }

    
    //NADPISANIE METODY ToString
    public override string ToString()
    {
        return $"('{UserId}', '{Username}', '{Password}, {Type}, '{Name}', '{Surname}', '{PESEL}', '{Phone}', '{Email}', '{Street}', '{City}', '{ZipCode}', '{HouseNumber}')";
    }
}