using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

//KLASA REPOZYTORIUM DLA DOKUMENTÓW
public class DocumentRepository
{
    //POLE DLA USŁUGI BAZY DANYCH
    private readonly DatabaseService _databaseService;
    
    
    //KONSTRUKTOR
    public DocumentRepository()
    {
        _databaseService = new DatabaseService();
    }
    
    
    //METODY
    //Metoda zwracająca wszystkie dokumenty
    public DataTable GetAllDocuments()
    {
        return _databaseService.GetAll<Document>("documents");
    }
    
    //Metoda zwracająca dokument po identyfikatorze
    public DataRow GetDocumentById(Guid id)
    {
        try
        {
            return _databaseService.GetById<Document>("documents", "documentId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda usuwająca dokument
    public void DeleteDocument(Guid id)
    {
        try
        {
            _databaseService.Delete<Document>("documents", "documentId", id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda dodająca dokument
    public void AddDocument(Document document)
    {
        try
        {
            var data = new Dictionary<String, object>
            {
                { "documentId", document.DocumentId },
                { "description", document.Description },
                { "file", document.File },
                { "customerId", document.Customer.CustomerId },
                { "userId", document.User.UserId },
                { "vehicleId", document.Vehicle.VehicleId },
                { "creationDate", document.CreationDate }
            };
            
            _databaseService.Insert<Document>("documents", data);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //Metoda aktualizująca dokument
    public void UpdateDocument(Document document)
    {
        try
        {
            var data = new Dictionary<String, object>
            {
                { "description", document.Description },
                { "file", document.File },
                { "customerId", document.Customer.CustomerId },
                { "userId", document.User.UserId },
                { "vehicleId", document.Vehicle.VehicleId },
                { "creationDate", document.CreationDate }
            };

            _databaseService.Update<Document>("documents", data, "documentId", document.DocumentId);
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