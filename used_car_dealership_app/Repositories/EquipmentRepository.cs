using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

//KLASA REPOZYTORIUM DLA WYPOSAŻENIA
public class EquipmentRepository
{
    //POLE DLA USŁUGI BAZY DANYCH
    private readonly DatabaseService _databaseService;

    
    //KONSTRUKTOR
    public EquipmentRepository()
    {
        _databaseService = new DatabaseService();
    }

    
    //METODY
    //Metoda zwracająca wszystkie wyposażenia
    public DataTable GetAllEquipment()
    {
        return _databaseService.GetAll<Equipment>("equipments");
    }
}