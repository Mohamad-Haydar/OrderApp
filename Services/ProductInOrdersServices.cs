using OrderApp.Exceptions;
using System.Diagnostics;

namespace OrderApp.Services
{
    public class ProductInOrdersServices
    {
        public async Task UpdateProductsInOrders(int quantity, int id)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                var updatePIOCommand = connection.CreateCommand();
                updatePIOCommand.CommandText = @"UPDATE ProductsInOrders SET Quantity = $quantity WHERE Id = $id";
                updatePIOCommand.Parameters.AddWithValue("$quantity", quantity);
                updatePIOCommand.Parameters.AddWithValue("$id", id);
                updatePIOCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in UpdateProductsInOrders: {ex}");
                throw new DataAccessException("Could not update product quantity in order.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<int> GetQuantity(int id)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                var getOldCommand = connection.CreateCommand();
                getOldCommand.CommandText = @"SELECT Quantity FROM ProductsInOrders WHERE Id = $id";
                getOldCommand.Parameters.AddWithValue("$id", id);
                var oldQuantity = Convert.ToInt32(getOldCommand.ExecuteScalar());

                connection.Close();
                return oldQuantity;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetQuantity: {ex}");
                throw new DataAccessException("Could not retrieve product quantity from order.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task InsertProductIntoProductsInOrder(int orderId, int productId, int quantity)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                // INSERT into ProductsInOrders
                using var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"
            INSERT INTO ProductsInOrders (OrderId, ProductId, Quantity)
            VALUES ($orderId, $productId, $quantity);";

                insertCommand.Parameters.AddWithValue("$orderId", orderId);
                insertCommand.Parameters.AddWithValue("$productId", productId);
                insertCommand.Parameters.AddWithValue("$quantity", quantity);

                insertCommand.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in InsertProductIntoProductsInOrder: {ex}");
                throw new DataAccessException("Could not insert product into order.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task DeleteProductInOrder(int orderId, int productId)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                // INSERT into ProductsInOrders
                using var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"DELETE FROM ProductsInOrders WHERE OrderId = $orderId AND ProductId = $productId";

                insertCommand.Parameters.AddWithValue("$orderId", orderId);
                insertCommand.Parameters.AddWithValue("$productId", productId);

                insertCommand.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in DeleteProductInOrder: {ex}");
                throw new DataAccessException("Could not delete product from order.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
