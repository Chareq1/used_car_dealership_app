using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

public class Meeting
{
    public Guid MeetingId { get; set; }
    public String Description { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public Customer Customer { get; set; }
    public Location Location { get; set; }
    
    public string CustomerFullName => $"{Customer.Name} {Customer.Surname}";
    public string CustomerFullAddress => $"ul. {Customer.Street} {Customer.HouseNumber}, {Customer.ZipCode} {Customer.City}";
    public string LocationFullNameAddress => $"{Location.Name}, ul. {Location.Street} {Location.HouseNumber}, {Location.ZipCode} {Location.City}";
    
    public override string ToString()
    {
        return $"('{MeetingId}', '{Description}', '{Date}', '{UserId}', '{Customer.CustomerId}', '{Location.LocationId}')";;
    }
}