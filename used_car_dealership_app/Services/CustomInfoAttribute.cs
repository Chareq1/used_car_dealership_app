using System;

namespace used_car_dealership_app.Services;

//ATRYBUT DLA DODATKOWYCH INFORMACJI
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class CustomInfoAttribute : Attribute
{
    //WŁAŚCIWOŚCI
    public string Description { get; }
    public float Version { get; }

    
    //KONSTRUKTOR
    public CustomInfoAttribute(string description, float version)
    {
        Description = description;
        Version = version;
    }
}