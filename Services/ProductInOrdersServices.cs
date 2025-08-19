using OrderApp.Exceptions;
using OrderApp.Model;
using OrderApp.Services.Interfaces;
using System.Diagnostics;

namespace OrderApp.Services
{
    public class ProductInOrdersServices : IProductInOrdersRepository
    {
        public async Task UpdateProductsInOrders(int quantity, int id)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                var updatePIOCommand = connection.CreateCommand();
                updatePIOCommand.CommandText = @"UPDATE ProductsInOrders SET Quantity = $quantity WHERE Id = $id";
                updatePIOCommand.Parameters.AddWithValue("$quantity", quantity);
                updatePIOCommand.Parameters.AddWithValue("$id", id);
                await updatePIOCommand.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in UpdateProductsInOrders: {ex}");
                throw new DataAccessException("Could not update product quantity in order.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task<int> GetQuantity(int id)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                var getOldCommand = connection.CreateCommand();
                getOldCommand.CommandText = @"SELECT Quantity FROM ProductsInOrders WHERE Id = $id";
                getOldCommand.Parameters.AddWithValue("$id", id);
                var oldQuantity = Convert.ToInt32(await getOldCommand.ExecuteScalarAsync());

                await connection.CloseAsync();
                return oldQuantity;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetQuantity: {ex}");
                throw new DataAccessException("Could not retrieve product quantity from order.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task InsertProductIntoProductsInOrder(int orderId, int productId, int quantity)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                // INSERT into ProductsInOrders asynchronously
                using var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"
            INSERT INTO ProductsInOrders (OrderId, ProductId, Quantity)
            VALUES ($orderId, $productId, $quantity);";

                insertCommand.Parameters.AddWithValue("$orderId", orderId);
                insertCommand.Parameters.AddWithValue("$productId", productId);
                insertCommand.Parameters.AddWithValue("$quantity", quantity);

                await insertCommand.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in InsertProductIntoProductsInOrder: {ex}");
                throw new DataAccessException("Could not insert product into order.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }

        public async Task DeleteProductInOrder(int orderId, int productId)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                using var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = @"DELETE FROM ProductsInOrders WHERE OrderId = $orderId AND ProductId = $productId";

                deleteCommand.Parameters.AddWithValue("$orderId", orderId);
                deleteCommand.Parameters.AddWithValue("$productId", productId);

                await deleteCommand.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not delete product from order.", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }
    }
}
