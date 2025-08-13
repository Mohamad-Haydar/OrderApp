using Microsoft.Data.Sqlite;

namespace OrderApp.Services
{
    public class AdoDatabaseService
    {
        private static string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={dbPath}"); ;
        }

        public static void Init()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            
            string clientTalbeQuery = "CREATE TABLE IF NOT EXISTS Clients (\r\n    Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n    Name TEXT NOT NULL,\r\n    Details TEXT\r\n);";
            string usersTableQuery = "CREATE TABLE IF NOT EXISTS Users (\r\n    Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n    Username TEXT NOT NULL UNIQUE,\r\n    Email TEXT NOT NULL UNIQUE,\r\n    Password TEXT NOT NULL\r\n);";
            string ordersTableQUery = "CREATE TABLE IF NOT EXISTS Orders (\r\n    Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n    ClientId INTEGER NOT NULL,\r\n   UserId INTEGER NOT NULL,\r\n  Total REAL NOT NULL,\r\n    DateToPick TEXT NOT NULL,\r\n FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,\r\n   FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE\r\n);";
            string productsTableQuery = "CREATE TABLE IF NOT EXISTS Products (\r\n    Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n    Name TEXT NOT NULL,\r\n    Description TEXT,\r\n    Price REAL NOT NULL CHECK (Price >= 0),\r\n    Quantity INTEGER NOT NULL CHECK (Quantity >= 0),\r\n  ImageUrl TEXT);";
            string productsInOrdersTableQuery = "CREATE TABLE IF NOT EXISTS ProductsInOrders (\r\n    Id INTEGER PRIMARY KEY AUTOINCREMENT,\r\n    OrderId INTEGER NOT NULL,\r\n    ProductId INTEGER NOT NULL,\r\n    Quantity INTEGER NOT NULL,\r\n    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,\r\n    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE\r\n);";
            string eventsTableQuery = @"CREATE TABLE IF NOT EXISTS Events (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        UserId INTEGER NOT NULL,
                                        EventName TEXT NOT NULL,
                                        Description TEXT,
                                        EventType TEXT NOT NULL, -- e.g., 'Meeting', 'Vacation', 'Call'
                                        StartTime DateTime NOT NULL,
                                        EndTime DateTime NOT NULL,
                                        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE);";
            connection.Open();

            
            var command = connection.CreateCommand();
            command.CommandText = clientTalbeQuery + usersTableQuery + ordersTableQUery + productsTableQuery + productsInOrdersTableQuery + eventsTableQuery;

            command.ExecuteNonQuery();   
            connection.Close();


        }

        public static void AddProduct(string name, string description, double price, int quantity)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            using var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Products (Name, Description, Price, Quantity) 
            VALUES ($name, $description, $price, $quantity);";

            insertCmd.Parameters.AddWithValue("$name", name);
            insertCmd.Parameters.AddWithValue("$description", description);
            insertCmd.Parameters.AddWithValue("$price", price);
            insertCmd.Parameters.AddWithValue("$quantity", quantity);

            insertCmd.ExecuteNonQuery();
            connection.Close();
        }

        public static List<(int Id, string Name, string Description, float Price, int Quantity)> GetProducts()
        {
            var products = new List<(int, string, string, float, int)>();

            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Products;";

            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                var description = reader.GetString(2);
                var price = reader.GetFloat(3);
                var quantity = reader.GetInt16(4);
                products.Add((id, name, description, price, quantity));
            }
            connection.Close();
            return products;
        }

    }
}
