using OrderApp.Exceptions;
using OrderApp.Model;
using OrderApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OrderApp.Services
{
    public class OrderServices : IOrderRepository
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
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                List<Order> orders = [];
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT o.Id, o.ClientId, o.Total, o.DateToPick, c.Name
                        FROM Orders as o JOIN Clients as c Where o.ClientId = c.Id AND o.UserId = $userId";

                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));

                using var reader = await command.ExecuteReaderAsync();
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
                return orders;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not retrieve orders.", ex);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task UpdateOrderAsync(ObservableCollection<ProductsInOrders> productsInOrders)
        {
            try
            {
                // ToList() makes a snapshot copy, so I can safely remove from the original collection while looping
                foreach (var item in productsInOrders.ToList())
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

                    // Step 3: Update ProductsInOrders Or insert it if not exists
                    if (oldQuantity < 1)
                    {
                        // INSERT into ProductsInOrders
                        await _productInOrdersServices.InsertProductIntoProductsInOrder(item.OrderId, item.Product.Id, item.Quantity);
                    }
                    else
                    {
                        await _productInOrdersServices.UpdateProductsInOrders(item.Quantity, item.Id);
                    }

                    // Step 4: Update Product stock
                    await _productServices.UpdateProductStock(difference, item.Product.Id);

                    // Step 5: if product in order is 0 remove the product from this order
                    if (item.Quantity == 0)
                    {
                        await _productInOrdersServices.DeleteProductInOrder(item.OrderId, item.Product.Id);
                        productsInOrders.Remove(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not update order.", ex);
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
                command.CommandText = @"INSERT INTO Orders (ClientId, Total, DateToPick, UserId)
                                        VALUES ($clientId, 0, $dateToPick, $userId);";

                command.Parameters.AddWithValue("$clientId", clientId);
                command.Parameters.AddWithValue("$dateToPick", dateToPick.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$userId", Preferences.Get("UserId", 0));

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not create order.", ex);
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
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not update order total", ex);
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
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not set order total.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task DeleteOrder(int orderId)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "Delete FROM Orders WHERE Id=$id";
                command.Parameters.AddWithValue("$id", orderId);

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not delete order.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
