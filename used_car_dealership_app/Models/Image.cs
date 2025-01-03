using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace used_car_dealership_app.Models;

public class Image
{
    public Guid ImageId { get; set; }
    public String FileName { get; set; }
    public String FilePath { get; set; }
    public Guid VehicleId { get; set; }
    
    public Bitmap Bitmap
    {
        get
        {
            return new Bitmap($"{FilePath}{FileName}");
        }
    }
    
    public override string ToString()
    {
        return $"('{ImageId}', '{FileName}', '{FilePath}', '{VehicleId}')";
    }
}