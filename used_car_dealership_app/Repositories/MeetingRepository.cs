using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;

namespace used_car_dealership_app.Repositories;

public class MeetingRepository
{
    private readonly DatabaseService _databaseService;

    public MeetingRepository()
    {
        _databaseService = new DatabaseService();
    }

    public DataTable GetAllMeetings()
    {
        return _databaseService.GetAll<Meeting>("meetings");
    }

    public DataRow GetMeetingById(Guid id)
    {
        return _databaseService.GetById<Meeting>("meetings", "meetingId", id);
    }

    public void DeleteMeeting(Guid id)
    {
        _databaseService.Delete<Meeting>("meetings", "meetingId", id);
    }

    public void AddMeeting(Meeting meeting)
    {
        var data = new Dictionary<string, object>
        {
            { "meetingId", meeting.MeetingId },
            { "description", meeting.Description },
            { "date", meeting.Date },
            { "userId", meeting.UserId },
            { "customerId", meeting.Customer.CustomerId },
            { "locationId", meeting.Location.LocationId }
        };

        _databaseService.Insert<Meeting>("meetings", data);
    }

    public void UpdateMeeting(Meeting meeting)
    {
        var data = new Dictionary<string, object>
        {
            { "description", meeting.Description },
            { "date", meeting.Date },
            { "userId", meeting.UserId },
            { "customerId", meeting.Customer.CustomerId },
            { "locationId", meeting.Location.LocationId }
        };

        _databaseService.Update<Meeting>("meetings", data, "meetingId", meeting.MeetingId);
    }

    public DataTable ExecuteQuery(string query, List<NpgsqlParameter> parameters)
    {
        return _databaseService.ExecuteQuery(query, parameters);
    }
    
    public DataTable GetMeetingsByDateAndUser(DateTime date, Guid userId)
    {
        var query = "SELECT * FROM meetings WHERE \"date\"::date = @date AND \"userId\" = @userId ORDER BY \"date\" ASC";
        
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@date", date.Date),
            new NpgsqlParameter("@userId", userId)
        };
        

        return _databaseService.ExecuteQuery(query, parameters);
    }
    
}