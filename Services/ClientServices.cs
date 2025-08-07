using OrderApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApp.Services
{
    public class ClientServices
    {

        public async Task<ObservableCollection<Client>> GetClientsForPopup()
        {
            ObservableCollection<Client> ClientsList = [];
            var connection = AdoDatabaseService.GetConnection();
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

        public async Task<List<Client>> GetClientsInfo()
        {
            List<Client> res = [];
            var connection = AdoDatabaseService.GetConnection();
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

        public async Task<string> GetCustomer(Order order)
        {
            var connection = AdoDatabaseService.GetConnection();
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

        public async Task AddClient(string name, string details)
        {
            var connection = AdoDatabaseService.GetConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                    INSERT INTO Clients (Name, Details)
                    VALUES ($name, $details)";

            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$details", details?? string.Empty);

            command.ExecuteNonQuery();
            connection.Close();
        }

    }
}
