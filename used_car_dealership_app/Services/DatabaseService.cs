using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using Npgsql;
using used_car_dealership_app.Services.Interfaces;

namespace used_car_dealership_app.Services;

//KLASA DLa USŁUGI BAZY DANYCH
public class DatabaseService : IDatabaseService
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<Program>();
    
    
    //POLA DLA POŁĄCZENIA Z BAZĄ
    private static String connectionString = "";
    public NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    
    
    //POLA DO INNYCH ELEMENTÓW
    private NpgsqlCommand command = null;

    
    //METODY
    //Metoda do utworzenia połączenia z bazą danych
    public void Connect()
    {
        try
        {
            Env.Load();
            
            var host = Env.GetString("DB_HOST");
            var port = Env.GetString("DB_PORT");
            var dbName = Env.GetString("DB_NAME");
            var user = Env.GetString("DB_USER");
            var password = Env.GetString("DB_PASSWORD");
            
            connectionString = $"Server={host};Port={port};Username={user};Password={password};Database={dbName}";
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

    //Metoda do zakończenia połączenia z bazą danych
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
    
    //Metoda do pobrania wszystkich rekordów z tabeli
    public DataTable GetAll<T>(String tableName)
    {
        Connect();
        
        String query = $"SELECT * FROM \"{tableName}\";";
        command = new NpgsqlCommand(query, connection);
        
        var adapter = new NpgsqlDataAdapter(command);
        var table = new DataTable();
        
        adapter.Fill(table);
        
        _logger.LogInformation("Pobrano {0} rekordów z tabeli {1}!", table.Rows.Count, tableName);
        
        Disconnect();
        
        return table;
    }
    
    //Metoda do pobrania rekordu z tabeli po ID
    public DataRow GetById<T>(String tableName, String idColumnName, Guid id)
    {
        Connect();
        
        try
        {
            String query = $"SELECT * FROM \"{tableName}\" WHERE \"{idColumnName}\" = @id;";
            command = new NpgsqlCommand(query, connection);
            
            command.Parameters.AddWithValue("id", id);

            var adapter = new NpgsqlDataAdapter(command);
            var table = new DataTable();

            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                _logger.LogError("Nie znaleziono rekordu z tabeli {0} o ID {1}!", tableName, id);
                throw new DataException("Nie znaleziono rekordu z tabeli o podanym ID.");
            }
            
            _logger.LogInformation("Pobrano rekord z tabeli {0} o ID {1}!", tableName, id);
            return table.Rows[0];
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania rekordu z tabeli {0}!", tableName);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Metoda do dodania rekordu do bazy danych
    public void Insert<T>(String tableName, Dictionary<String, object> data)
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
                    if (tableName == "users")
                    {
                        values.Append($"@{kvp.Key}::usertype");
                    }
                    else
                    {
                        values.Append($"@{kvp.Key}::vehicletype");
                    }
                }
                else
                {
                    values.Append($"@{kvp.Key}");
                }

                parameters.Add(new NpgsqlParameter($"@{kvp.Key}", kvp.Value ?? DBNull.Value));
            }


            var commandText = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({values})";
            using var command = new NpgsqlCommand(commandText, connection);
            command.Parameters.AddRange(parameters.ToArray());

            command.ExecuteNonQuery();

            _logger.LogInformation("Dodano rekord do tabeli {0}!", tableName);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Błąd podczas dodawania rekordu do tabeli {0}!", tableName);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Metoda do aktualizacji rekordu w bazie danych
    public void Update<T>(String tableName, Dictionary<String, object> data, String idColumnName, Guid id)
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
                    if(tableName == "users")
                    {
                        setClause.Append($"\"{kvp.Key}\" = @{kvp.Key}::usertype");
                    }
                    else
                    {
                        setClause.Append($"\"{kvp.Key}\" = @{kvp.Key}::vehicletype");
                    }
                }
                else
                {
                    setClause.Append($"\"{kvp.Key}\" = @{kvp.Key}");
                }

                parameters.Add(new NpgsqlParameter($"@{kvp.Key}", kvp.Value ?? DBNull.Value));
            }

            var commandText = $"UPDATE \"{tableName}\" SET {setClause} WHERE \"{idColumnName}\" = @id";
            using var command = new NpgsqlCommand(commandText, connection);
            command.Parameters.AddRange(parameters.ToArray());
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();

            _logger.LogInformation("Zaktualizowano rekord w tabeli {0} o ID {1}!", tableName, id.ToString());
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji rekordu w tabeli {0}!", tableName);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Metoda do usunięcia rekordu z bazy danych
    public void Delete<T>(String tableName, String idColumnName, Guid id)
    {
        Connect();
        try
        {
            String query = $"DELETE FROM \"{tableName}\" WHERE \"{idColumnName}\" = @id;";
            command = new NpgsqlCommand(query, connection);
            
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            
            _logger.LogInformation("Usunięto rekord z tabeli {0} o ID {1}!", tableName, id);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania rekordu z tabeli {0}!", tableName);
            throw;
        }
        finally
        {
            Disconnect();
        }
    }
    
    //Metoa do wykonania zapytania do bazy danych
    public DataTable ExecuteQuery(String query, List<NpgsqlParameter> parameters)
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
        catch (NpgsqlException ex)
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
