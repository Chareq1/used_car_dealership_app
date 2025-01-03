using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace used_car_dealership_app.Models;

//KLASA REPREZENTUJĄCA OBRAZ
public class Image
{
    //POLA
    public Guid ImageId { get; set; }
    public String FileName { get; set; }
    public String FilePath { get; set; }
    public Guid VehicleId { get; set; }
    
    
    //POLE POMOCNICZE DO WYŚWIETLANIA OBRAZKA W FORMULARZU
    public Bitmap Bitmap
    {
        get
        {
            return new Bitmap($"{FilePath}{FileName}");
        }
    }
    
    
    //NADPISANIE METODY ToString
    public override String ToString()
    {
        return $"('{ImageId}', '{FileName}', '{FilePath}', '{VehicleId}')";
    }
}