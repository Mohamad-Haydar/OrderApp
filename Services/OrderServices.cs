using CommunityToolkit.Maui.Views;
using OrderApp.Model;
using System.Collections.ObjectModel;

namespace OrderApp.Services
{
    public class OrderServices
    {
        private ProductsServices _productServices;
        private ProductInOrdersServices _productInOrdersServices;

        public OrderServices(ProductsServices productServices, ProductInOrdersServices productInOrdersServices)
        {
            _productServices = productServices;
            _productInOrdersServices = productInOrdersServices;
        }
        public async Task<List<Order>> GetOrders()
        {
            try
            {
                List<Order> orders = [];
                var connection = AdoDatabaseService.GetConnection();
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT o.Id, o.ClientId, o.Total, o.DateToPick, c.Name
                        FROM Orders as o JOIN Clients as c Where o.ClientId = c.Id AND o.UserId = $userId";

                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));

                using var reader = command.ExecuteReader();
                // add all the orders to the order collection to see them in the view
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        Id = reader.GetInt16(0),
                        ClientId = reader.GetInt32(1),
                        Total = reader.GetFloat(2),
                        DateToPick = reader.GetDateTime(3),
                        ClientName = reader.GetString(4)
                    });
                }
                connection.Close();
                return orders;
            }
            catch (Exception ex)
            {
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return null;
            }
        }

        public async Task AddProductToOrder(Order order, Product product, int quantity)
        {
            try
            {
                // INSERT into ProductsInOrders
                await _productInOrdersServices.InsertProduct(order.Id, product.Id, quantity);

                // UPDATE the product stock
                await _productServices.UpdateProductStock(quantity, product.Id);

                // Calculate cost to add to order total
                float addedAmount = quantity * product.Price;

                // UPDATE the order total
                await UpdateTotalAsync(addedAmount, order.Id);
            }
            catch (Exception ex)
            {
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return;
            }
        }
        public async Task UpdateOrderAsync(ObservableCollection<ProductsInOrders> productsInOrders)
        {
          
            foreach (var item in productsInOrders)
            {
                // Step 1: Get the old quantity from the database
                var oldQuantity = await _productInOrdersServices.GetQuantity(item.Id);

                // Step 2: Calculate the difference
                int difference = item.Quantity - oldQuantity;

                if (difference > 0)
                {
                    // Step 2.1: Check available stock
                    var availableStock = await _productServices.GetStuckQuantity(item.Product.Id);

                    if (difference > availableStock)
                    {
                        await Shell.Current.DisplayAlert("Error", "Not enough stock available", "OK");
                        return;
                    }
                }

                // Step 3: Update ProductsInOrders
                await _productInOrdersServices.UpdateProductsInOrders(item.Quantity, item.Id);

                // Step 4: Update Product stock
                await _productServices.UpdateProductStock(difference, item.Product.Id);
            }
           
        }

        public async Task CreateOrder(int clientId, DateTime dateToPick )
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {

                // create the command to create new order
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO Orders (ClientId, Total, DateToPick, UserId)
            VALUES ($clientId, 0, $dateToPick, $userId);";

                command.Parameters.AddWithValue("$clientId", clientId);
                command.Parameters.AddWithValue("$dateToPick", dateToPick.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task UpdateTotalAsync(float addedAmount, int orderId)
        {
                var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                using var updateTotalCommand = connection.CreateCommand();
                updateTotalCommand.CommandText = @"UPDATE Orders 
            SET Total = Total + $added 
            WHERE Id = $orderId;";

                updateTotalCommand.Parameters.AddWithValue("$added", addedAmount);
                updateTotalCommand.Parameters.AddWithValue("$orderId", orderId);
                updateTotalCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task SetTotalAsync(Order order)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                var updateOrderCommand = connection.CreateCommand();
                updateOrderCommand.CommandText = @"UPDATE Orders SET Total = $total WHERE Id = $id";
                updateOrderCommand.Parameters.AddWithValue("$total", order.Total);
                updateOrderCommand.Parameters.AddWithValue("$id", order.Id);
                updateOrderCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task DeleteOrder(Order order)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "Delete FROM Orders WHERE Id=$id";
                command.Parameters.AddWithValue("$id", order.Id);

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                // Log exception somewhere
                Console.WriteLine($"Error retrieving stock: {ex.Message}");

                // Return a safe default value or rethrow a custom exception
                return;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
