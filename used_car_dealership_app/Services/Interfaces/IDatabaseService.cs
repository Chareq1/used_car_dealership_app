using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace used_car_dealership_app.Services;

public interface IDatabaseService
{
    public void Connect();
    public void Disconnect();
    public DataTable GetAll<T>(String tableName);
    public DataRow GetById<T>(string tableName, string idColumnName, Guid id);
    public void Insert<T>(string tableName, Dictionary<string, object> data);
    public void Update<T>(string tableName, Dictionary<string, object> data, string idColumnName, Guid id);
    public void Delete<T>(string tableName, string idColumnName, Guid id);
    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters);
}