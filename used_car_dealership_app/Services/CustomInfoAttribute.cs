using System;

namespace used_car_dealership_app.Services;

//ATRYBUT DLA DODATKOWYCH INFORMACJI
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class CustomInfoAttribute : Attribute
{
    //WŁAŚCIWOŚCI
    public String Description { get; }
    public float Version { get; }

    
    //KONSTRUKTOR
    public CustomInfoAttribute(String description, float version)
    {
        Description = description;
        Version = version;
    }
}