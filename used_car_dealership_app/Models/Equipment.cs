using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA WYPOSAŻENIE
public class Equipment
{
    //POLA
    public Guid EquipmentId { get; set; }
    public String Name { get; set; }

    
    //POLE POMOCNICZE DO ZAZNACZANIA WYPOSAŻENIA W FORMULARZU
    public bool IsSelected { get; set; }
    
    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{EquipmentId}', '{Name}')";
    }
}