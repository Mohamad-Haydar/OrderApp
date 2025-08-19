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
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Clients";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ClientsList.Add(new Client
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
                connection.Close();
                return ClientsList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not Get Clients For Popup.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<List<Client>> GetClientsInfo()
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                List<Client> res = [];
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Details FROM Clients";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    res.Add(new Client
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Details = reader.GetString(2)
                    });
                }
                connection.Close();
                return res;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not Get Client Info", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<string> GetCustomer(Order order)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                var result = string.Empty;
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT c.Name FROM Clients as c Where c.Id = $clientId";

                command.Parameters.AddWithValue("$clientId", order.ClientId);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    result = reader.GetString(0);
                }

                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not Get Customer.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task AddClient(string name, string details)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(details))
                throw new ValidationException("Name and details must be filled and valid.");

            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Clients (Name, Details)
                    VALUES ($name, $details)";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$details", details ?? string.Empty);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not Add Client", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
