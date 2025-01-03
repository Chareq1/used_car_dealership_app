using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

//KLASA REPOZYTORIUM DLA OBRAZKÓW
public class ImageRepository
{
    //POLE DLA USŁUGI BAZY DANYCH
    private readonly DatabaseService _databaseService;

    
    //KONSTRUKTOR
    public ImageRepository()
    {
        _databaseService = new DatabaseService();
    }

    
    //METODY
    //Metoda zwracająca wszystkie obrazki
    public DataTable GetAllImages()
    {
        return _databaseService.GetAll<Image>("images");
    }

    //Metoda zwracająca obrazki po identyfikatorze pojazdu
    public DataTable GetImagesByVehicleId(Guid vehicleId)
    {
        try
        {
            var query = "SELECT * FROM images WHERE \"vehicleId\" = @vehicleId";
            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@vehicleId", vehicleId)
            };
            return _databaseService.ExecuteQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda zwracająca obrazek po identyfikatorze
    public DataRow GetImageById(Guid id)
    {
        try
        {
            return _databaseService.GetById<Image>("images", "imageId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    //Metoda usuwająca obrazek
    public void DeleteImage(Guid id)
    {
        try
        {
            _databaseService.Delete<Image>("images", "imageId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda usuwająca obrazki po identyfikatorze pojazdu
    public void DeleteImagesByVehicleId(Guid id)
    {
        try
        {
            _databaseService.Delete<Image>("images", "vehicleId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }  

    //Metoda dodająca obrazek
    public void AddImage(Image image)
    {
        try
        {
            var data = new Dictionary<String, object>
            {
                { "imageId", image.ImageId },
                { "fileName", image.FileName },
                { "filePath", image.FilePath },
                { "vehicleId", image.VehicleId }
            };

            _databaseService.Insert<Image>("images", data);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    //Metoda aktualizująca obrazek
    public void UpdateImage(Image image)
    {
        try
        {
            var data = new Dictionary<String, object>
            {
                { "fileName", image.FileName },
                { "filePath", image.FilePath },
                { "vehicleId", image.VehicleId }
            };

            _databaseService.Update<Image>("images", data, "imageId", image.ImageId);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda wykonująca zapytanie
    public DataTable ExecuteQuery(String query, List<NpgsqlParameter> parameters)
    {
        try
        {
            return _databaseService.ExecuteQuery(query, parameters);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}