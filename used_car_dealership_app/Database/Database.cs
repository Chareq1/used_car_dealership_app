using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using used_car_dealership_app.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tmds.DBus.Protocol;


namespace used_car_dealership_app.Database;

public class Database : DatabaseService
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<Program>();
    
    //POLA DLA POŁĄCZENIA Z BAZĄ
    private static String connectionString = "";
    public NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    
    //POLA DO INNYCH ELEMENTÓW
    private NpgsqlCommand command = null;

    
    //Utworzenie połączenia z bazą danych
    public void Connect()
    {
        try
        {
            connectionString = "Server=localhost;Port=5432;Username=ucda_admin;Password=zaq1@WSX;Database=ucda";
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
            _logger.LogInformation("Utworzono połączenie z bazą danych!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia połączenia z bazą danych!");
            throw;
        }
    }

    //Zamknięcie połączenia z bazą danych
    public void Disconnect()
    {
        try
        {
            if (connection.State == ConnectionState.Open && connection != null)
            {
                connection.Close();
                _logger.LogInformation("Zamknięto połączenie z bazą danych!");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas zamykania połączenia z bazą danych!");
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
        
        _logger.LogInformation("Pobrano {0} rekordów z tabeli {1}!", table.Rows.Count, tableName);
        
        Disconnect();
        
        return table;
    }
    
    //Pobranie rekordu z bazy danych o wskazanym UUID
    public DataRow GetById<T>(string tableName, string idColumnName, Guid id)
    {
        Connect();
        
        try
        {
            string query = $"SELECT * FROM {tableName} WHERE \"{idColumnName}\" = @id;";
            command = new NpgsqlCommand(query, connection);
            
            command.Parameters.AddWithValue("id", id);

            var adapter = new NpgsqlDataAdapter(command);
            var table = new DataTable();

            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                _logger.LogError("Nie znaleziono rekordu z tabeli {0} o ID {1}!", tableName, id);
                throw new Exception("No record found with the given ID.");
            }
            
            _logger.LogInformation("Pobrano rekord z tabeli {0} o ID {1}!", tableName, id);
            return table.Rows[0];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania rekordu z tabeli {0} o ID {1}!", tableName, id);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Dodanie rekordu do bazy danych
    public void Insert<T>(string tableName, Dictionary<string, object> data)
    {
        Connect();
        try
        {
            var columns = new StringBuilder();
            var values = new StringBuilder();
            var parameters = new List<NpgsqlParameter>();

            foreach (var kvp in data)
            {
                if (columns.Length > 0)
                {
                    columns.Append(", ");
                    values.Append(", ");
                }

                columns.Append($"\"{kvp.Key}\"");

                if (kvp.Key == "type")
                {
                    values.Append($"@{kvp.Key}::usertype");
                }
                else
                {
                    values.Append($"@{kvp.Key}");
                }

                parameters.Add(new NpgsqlParameter($"@{kvp.Key}", kvp.Value ?? DBNull.Value));
            }


            var commandText = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            using var command = new NpgsqlCommand(commandText, connection);
            command.Parameters.AddRange(parameters.ToArray());

            command.ExecuteNonQuery();
            
            _logger.LogInformation("Dodano rekord do tabeli {0}!", tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dodawania rekordu do tabeli {0}!", tableName);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Aktualizacja rekordu w bazie danych
    public void Update<T>(string tableName, Dictionary<string, object> data, string idColumnName, Guid id)
    {
        Connect();
        try
        {
            var setClause = new StringBuilder();
            var parameters = new List<NpgsqlParameter>();

            foreach (var kvp in data)
            {
                if (setClause.Length > 0)
                {
                    setClause.Append(", ");
                }

                if (kvp.Key == "type")
                {
                    setClause.Append($"\"{kvp.Key}\" = @{kvp.Key}::usertype");
                }
                else
                {
                    setClause.Append($"\"{kvp.Key}\" = @{kvp.Key}");
                }

                parameters.Add(new NpgsqlParameter($"@{kvp.Key}", kvp.Value ?? DBNull.Value));
            }

            var commandText = $"UPDATE {tableName} SET {setClause} WHERE \"{idColumnName}\" = @id";
            using var command = new NpgsqlCommand(commandText, connection);
            command.Parameters.AddRange(parameters.ToArray());
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();

            _logger.LogInformation("Zaktualizowano rekord w tabeli {0} o ID {1}!", tableName, id.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji rekordu w tabeli {0} o ID {1}!", tableName, id.ToString());
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Usunięcie rekordu z bazy danych
    public void Delete<T>(string tableName, string idColumnName, Guid id)
    {
        Connect();
        try
        {
            string query = $"DELETE FROM {tableName} WHERE \"{idColumnName}\" = @id;";
            command = new NpgsqlCommand(query, connection);
            
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            
            _logger.LogInformation("Usunięto rekord z tabeli {0} o ID {1}!", tableName, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania rekordu z tabeli {0} o ID {1}!", tableName, id);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Wykonanie zapytania SQL
    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        Connect();
        try
        {
            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());

            var adapter = new NpgsqlDataAdapter(command);
            var table = new DataTable();
            adapter.Fill(table);

            _logger.LogInformation("Wykonano zapytanie: {0}!", query);
            return table;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wykonywania zapytania: {0}!", query);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
}
