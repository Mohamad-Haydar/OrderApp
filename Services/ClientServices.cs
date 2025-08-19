using OrderApp.Exceptions;
using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;
using System.Diagnostics;

namespace OrderApp.Services
{
    public class ClientServices : IClientRepository
    {

        public async Task<ObservableCollection<Client>> GetClientsForPopup()
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                ObservableCollection<Client> ClientsList = [];
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Clients";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ClientsList.Add(new Client
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
                await connection.CloseAsync();
                return ClientsList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetClientsForPopup: {ex}");
                throw new DataAccessException("Could not Get Clients For Popup.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task<List<Client>> GetClientsInfo()
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                List<Client> res = [];
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Details FROM Clients";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    res.Add(new Client
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Details = reader.GetString(2)
                    });
                }
                await connection.CloseAsync();
                return res;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetClientsInfo: {ex}");
                throw new DataAccessException("Could not Get Client Info", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task<string> GetCustomer(Order order)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                var result = string.Empty;
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT c.Name FROM Clients as c Where c.Id = $clientId";

                command.Parameters.AddWithValue("$clientId", order.ClientId);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    result = reader.GetString(0);
                }

                await connection.CloseAsync();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetCustomer: {ex}");
                throw new DataAccessException("Could not Get Customer.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task AddClient(string name, string details)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(details))
                throw new ValidationException("Name and details must be filled and valid.");

            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Clients (Name, Details)
                    VALUES ($name, $details)";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$details", details ?? string.Empty);
                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in AddClient: {ex}");
                throw new DataAccessException("Could not Add Client", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }
    }
}
