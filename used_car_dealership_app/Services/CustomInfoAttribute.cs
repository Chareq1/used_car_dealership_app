using System;

namespace used_car_dealership_app.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class CustomInfoAttribute : Attribute
{
    public string Description { get; }
    public float Version { get; }

    public CustomInfoAttribute(string description, float version)
    {
        Description = description;
        Version = version;
    }
}