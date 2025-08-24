using Microsoft.Data.Sqlite;
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
                var userId = Preferences.Get("UserId", 0);

                command.Parameters.AddWithValue("$userId", userId);

                using var reader = await command.ExecuteReaderAsync();
                // add all the orders to the order collection to see them in the view
                while (await reader.ReadAsync())
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
                Debug.WriteLine($"DB Error in GetOrders: {ex}");
                throw new DataAccessException("Could not retrieve orders.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task UpdateOrderAsync(ObservableCollection<ProductsInOrders> productsInOrders, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
        {
            bool FromOut = true;
            try
            {
                if (connection == null)
                {
                    FromOut = false;
                    connection = AdoDatabaseService.GetConnection();
                    await connection.OpenAsync();
                }
                if (transaction == null)
                    transaction = connection.BeginTransaction();
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
                            throw new ValidationException("Not enough stock available");
                        }
                    }

                    // Step 3: Update ProductsInOrders Or insert it if not exists
                    if (oldQuantity < 1)
                    {
                        // INSERT into ProductsInOrders
                        await _productInOrdersServices.InsertProductIntoProductsInOrder(item.OrderId, item.Product.Id, item.Quantity, connection, transaction);
                    }
                    else
                    {
                        await _productInOrdersServices.UpdateProductsInOrders(item.Quantity, item.Id, connection, transaction);
                    }

                    // Step 4: Update Product stock
                    await _productServices.UpdateProductStock(difference, item.Product.Id, connection, transaction);

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
                if(!FromOut) await transaction.RollbackAsync();
                Debug.WriteLine($"DB Error in UpdateOrderAsync: {ex}");
                throw new DataAccessException("Could not update order.", ex);
            }
            finally
            {
                if (!FromOut && connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task<Order> CreateOrder(int clientId, DateTime dateToPick)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                // create the command to create new order
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Orders (ClientId, Total, DateToPick, UserId)
                                        VALUES ($clientId, 0, $dateToPick, $userId);";

                var userId = Preferences.Get("UserId", 0);

                command.Parameters.AddWithValue("$clientId", clientId);
                command.Parameters.AddWithValue("$dateToPick", dateToPick.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$userId", userId);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();

                return new Order { ClientId = clientId, DateToPick = dateToPick, Total = 0  };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in CreateOrder: {ex}");
                throw new DataAccessException("Could not create order.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task UpdateTotalAsync(float addedAmount, int orderId)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                using var updateTotalCommand = connection.CreateCommand();
                updateTotalCommand.CommandText = @"UPDATE Orders 
                                                    SET Total = Total + $added 
                                                    WHERE Id = $orderId;";

                updateTotalCommand.Parameters.AddWithValue("$added", addedAmount);
                updateTotalCommand.Parameters.AddWithValue("$orderId", orderId);
                await updateTotalCommand.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in UpdateTotalAsync: {ex}");
                throw new DataAccessException("Could not update order total", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task SetTotalAsync(Order order, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
        {
            bool FromOut = true;
            try
            {
                if (connection == null)
                {
                    FromOut = false;
                    connection = AdoDatabaseService.GetConnection();
                    await connection.OpenAsync();
                }
                await connection.OpenAsync();
                if (transaction == null)
                    transaction = connection.BeginTransaction();

                var updateOrderCommand = connection.CreateCommand();
                updateOrderCommand.CommandText = @"UPDATE Orders SET Total = $total WHERE Id = $id";
                updateOrderCommand.Parameters.AddWithValue("$total", order.Total);
                updateOrderCommand.Parameters.AddWithValue("$id", order.Id);
                await updateOrderCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                await transaction?.RollbackAsync();
                Debug.WriteLine($"DB Error in SetTotalAsync: {ex}");
                throw new DataAccessException("Could not set order total.", ex);
            }
            finally
            {
                if (!FromOut && connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task DeleteOrder(int orderId)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "Delete FROM Orders WHERE Id=$id";
                command.Parameters.AddWithValue("$id", orderId);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteOrder: {ex}");
                throw new DataAccessException("Could not delete order.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }
    }
}
