using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA SPOTKANIE
public class Meeting
{
    //POLA
    public Guid MeetingId { get; set; }
    public String Description { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public Customer Customer { get; set; }
    public Location Location { get; set; }
    
    
    //POLA POMOCNICZE DO WYŚWIETLANIA PEŁNEGO IMIENIA KLIENTA I PEŁNEGO ADRESU SPOTKANIA
    public String CustomerFullName => $"{Customer.Name} {Customer.Surname}";
    public String CustomerFullAddress => $"ul. {Customer.Street} {Customer.HouseNumber}, {Customer.ZipCode} {Customer.City}";
    public String LocationFullNameAddress => $"{Location.Name}, ul. {Location.Street} {Location.HouseNumber}, {Location.ZipCode} {Location.City}";
    
    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{MeetingId}', '{Description}', '{Date}', '{UserId}', '{Customer.CustomerId}', '{Location.LocationId}')";;
    }
}