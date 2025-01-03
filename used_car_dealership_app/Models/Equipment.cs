using System;
using System.ComponentModel.DataAnnotations;

namespace used_car_dealership_app.Models;

public class Equipment
{
    public Guid EquipmentId { get; set; }
    public string Name { get; set; }

    public bool IsSelected { get; set; }
    
    public override string ToString()
    {
        return $"('{EquipmentId}', '{Name}')";
    }
}