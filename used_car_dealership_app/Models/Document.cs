using System;
using System.IO;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA DOKUMENT
public class Document
{
    //POLA
    public Guid DocumentId { get; set; }
    public String Description { get; set; }
    public String File { get; set; }
    public Customer Customer { get; set; }
    public User User { get; set; }
    public Vehicle Vehicle { get; set; }
    public DateTime CreationDate { get; set; }
    
    
    //POLA POMOCNICZE DO WYŚWIETLANIA ODPOWIEDNICH DANYCH
    public String CustomerFullName => Customer.Name + " " + Customer.Surname + " ("+ Customer.Email+")";
    public String UserFullName => User.Name + " " + User.Surname + " ("+ User.Email+")";
    public String VehicleFullName => Vehicle.Brand + " " + Vehicle.Model + " ("+ Vehicle.VIN+")";
    public String FileName => Path.GetFileName(File);
    
    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{DocumentId}', '{Description}', '{File}', '{Customer.CustomerId}', '{User.UserId}', '{Vehicle.VehicleId}', '{CreationDate}')";
    }
}