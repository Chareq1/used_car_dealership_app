using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace used_car_dealership_app.Services.Interfaces;

//INTERFEJS DLA USŁUGI BAZY DANYCH
public interface IDatabaseService
{
    //METODY
    public void Connect();
    public void Disconnect();
    public DataTable GetAll<T>(String tableName);
    public DataRow GetById<T>(String tableName, String idColumnName, Guid id);
    public void Insert<T>(String tableName, Dictionary<String, object> data);
    public void Update<T>(String tableName, Dictionary<String, object> data, String idColumnName, Guid id);
    public void Delete<T>(String tableName, String idColumnName, Guid id);
    public DataTable ExecuteQuery(String query, List<NpgsqlParameter> parameters);
}