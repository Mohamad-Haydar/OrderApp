using OrderApp.Exceptions;
using OrderApp.Model;
using OrderApp.Services.Interfaces;
using System.Diagnostics;

namespace OrderApp.Services
{
    public class EventsServices : IEventRepository
    {
        public async Task<List<EventModel>> GetEvents(DateOnly date)
        {
            try
            {
                var events = new List<EventModel>();

                using var connection = AdoDatabaseService.GetConnection();
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Events where UserId=$userId AND (
                                DATE(StartTime) = $targetDate 
                                OR DATE(EndTime) = $targetDate
                                OR ($targetDate BETWEEN DATE(StartTime) AND DATE(EndTime)))";

                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));
                command.Parameters.AddWithValue("$targetDate", date.ToString("yyyy-MM-dd"));

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    events.Add(new EventModel
                    {
                        Id = reader.GetInt32(0),
                        EventName = reader.GetString(2),
                        Description = reader.GetString(3),
                        EventType = reader.GetString(4),
                        From = reader.GetDateTime(5),
                        To = reader.GetDateTime(6)
                    });
                }
                await connection.CloseAsync();

                return events;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetEvents: {ex}");
                throw new DataAccessException("Could Not Get Events.", ex);
            }
        }

        public async Task<List<EventModel>> GetAllEvents()
        {
            using var connection = AdoDatabaseService.GetConnection();
            try
            {
                var events = new List<EventModel>();

                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Events where UserId=$userId";

                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    events.Add(new EventModel
                    {
                        Id = reader.GetInt32(0),
                        EventName = reader.GetString(2),
                        Description = reader.GetString(3),
                        EventType = reader.GetString(4),
                        From = reader.GetDateTime(5),
                        To = reader.GetDateTime(6)
                    });
                }
                await connection.CloseAsync();

                return events;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetAllEvents: {ex}");
                throw new DataAccessException("Could not Get All Events.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task<int> AddEvent(string eventName, string description, string eventType, string startDateTime, string endDateTime)
        {
            if (string.IsNullOrEmpty(eventName) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(eventType) || string.IsNullOrEmpty(startDateTime) || string.IsNullOrEmpty(endDateTime))
                throw new ValidationException("All event fields must be filled and valid.");

            using var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Events 
                                        (UserId, EventName, Description, EventType, StartTime, EndTime) 
                                        VALUES ($userId, $EventName, $Description, $SelectedEventType, $startDateTime, $endDateTime)";

                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));
                command.Parameters.AddWithValue("$EventName", eventName);
                command.Parameters.AddWithValue("$Description", description);
                command.Parameters.AddWithValue("$SelectedEventType", eventType);
                command.Parameters.AddWithValue("$startDateTime", startDateTime);
                command.Parameters.AddWithValue("$endDateTime", endDateTime);

                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT last_insert_rowid();";
                var result = await command.ExecuteScalarAsync();
                int insertedId = Convert.ToInt32(result);
                await connection.CloseAsync();

                return insertedId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in AddEvent: {ex}");
                throw new DataAccessException("Could not add event.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }
    
        public async Task<int> UpdateEvent(int id, string eventName, string description, string eventType, string startDateTime, string endDateTime)
        {
            using var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE Events 
            SET EventName = $EventName, Description = $Description, EventType = $SelectedEventType, StartTime = $startDateTime, EndTime = $endDateTime 
            WHERE Id = $id AND UserId = $userId";
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));
                command.Parameters.AddWithValue("$EventName", eventName);
                command.Parameters.AddWithValue("$Description", description);
                command.Parameters.AddWithValue("$SelectedEventType", eventType);
                command.Parameters.AddWithValue("$startDateTime", startDateTime);
                command.Parameters.AddWithValue("$endDateTime", endDateTime);
                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT last_insert_rowid();";
                var result = await command.ExecuteScalarAsync();
                int insertedId = Convert.ToInt32(result);
                await connection.CloseAsync();

                return insertedId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in UpdateEvent: {ex}");
                throw new DataAccessException("Could not Update the event.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task<bool> DeleteEvent(int id)
        {
            using var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"DELETE FROM Events WHERE Id = $id AND UserId = $userId";
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));
                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteEvent: {ex}");
                throw new DataAccessException("Could not delete The Event.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }
    }
}
