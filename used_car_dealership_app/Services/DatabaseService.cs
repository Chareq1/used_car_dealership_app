using System;
using System.Data;

namespace used_car_dealership_app.Services;

public interface DatabaseService
{
    public void Connect();
    public void Disconnect();
    public DataTable GetAll<T>(String tableName);
    public DataTable GetById<T>(String tableName, Guid id);
    public void Insert<T>(String tableName, T item);
    public void Update<T>(String tableName, T item);
    public void Delete<T>(String tableName, T item);
    public void ExecuteQuery(String tableName, string query);
}