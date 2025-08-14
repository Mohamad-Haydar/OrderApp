using OrderApp.Model;
using System.Collections.ObjectModel;

namespace OrderApp.Services
{
    public class ClientServices
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
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return null;
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
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return null;
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
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task AddClient(string name, string details)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(details))
                    throw new Exception("Fill all input");
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
            catch (Exception ex)
            {
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
