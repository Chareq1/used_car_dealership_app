using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using used_car_dealership_app.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tmds.DBus.Protocol;

namespace used_car_dealership_app.Database;

public class Database
{
    //POLA DLA POŁĄCZENIA Z BAZĄ
    private static String connectionString = "";
    public NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    
    //POLA DO INNYCH ELEMENTÓW
    private NpgsqlCommand command = null;
    
    //METODY
    //Utworzenie połączenia z bazą danych
    public void Connect()
    {
        try
        {
            connectionString="Server=localhost;Port=5432;Username=ucda_admin;Password=zaq1@WSX;Database=ucda";
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Connection established");
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); throw; }
    }

    //Zamknięcie połączenia z bazą danych
    public void Disconnect()
    {
        try
        {
            if (connection.State == ConnectionState.Open && connection != null)
            {
                connection.Close();
                Console.WriteLine("Connection closed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to disconnect from database: {ex.Message}");
            throw;
        }
    }
    
    //Pobranie wszystkich danych z bazy danych
    public DataTable GetAll<T>(string tableName)
    {
        Connect();
        string query = $"SELECT * FROM {tableName};";
        command = new NpgsqlCommand(query, connection);
        
        var adapter = new NpgsqlDataAdapter(command);
        var table = new DataTable();
        
        adapter.Fill(table);
        
        foreach (DataRow row in table.Rows)
        {
            Console.WriteLine(string.Join(", ", row.ItemArray));
        }
        
        Disconnect();
        
        return table;
    }
    
    //Pobranie rekordu z bazy danych o wskazanym UUID
    public DataRow GetById<T>(string tableName, string idColumnName, Guid id)
    {
        Connect();
        try
        {
            string query = $"SELECT * FROM {tableName} WHERE {idColumnName} = @Id;";
            command = new NpgsqlCommand(query, connection);
            
            command.Parameters.AddWithValue("@Id", id);

            var adapter = new NpgsqlDataAdapter(command);
            var table = new DataTable();

            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                throw new Exception("No record found with the given ID.");
            }
            
            return table.Rows[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to retrieve record by ID: {ex.Message}");
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
}
