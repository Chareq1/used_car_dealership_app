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
    public String Username { get; set; }
    public String Password { get; set; }
    public UserType Type { get; set; }
    public String Name { get; set; }
    public String Surname { get; set; }
    public String PESEL { get; set; }
    public String Phone { get; set; }
    public String Email { get; set; }
    public String Street { get; set; }
    public String City { get; set; }
    public String ZipCode { get; set; }
    public String HouseNumber { get; set; }

    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{UserId}', '{Username}', '{Password}, {Type}, '{Name}', '{Surname}', '{PESEL}', '{Phone}', '{Email}', '{Street}', '{City}', '{ZipCode}', '{HouseNumber}')";
    }
}