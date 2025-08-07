namespace OrderApp.Services
{
    public class ProductInOrdersServices
    {
        public async Task UpdateProductsInOrders(int quantity, int id)
        {
            var connection = AdoDatabaseService.GetConnection();
            connection.Open();
            // Step 3: Update ProductsInOrders
            var updatePIOCommand = connection.CreateCommand();
            updatePIOCommand.CommandText = @"UPDATE ProductsInOrders SET Quantity = $quantity WHERE Id = $id";
            updatePIOCommand.Parameters.AddWithValue("$quantity", quantity);
            updatePIOCommand.Parameters.AddWithValue("$id", id);
            updatePIOCommand.ExecuteNonQuery();
            connection.Close();
        }

        public async Task<int> GetQuantity(int id)
        {
            var connection = AdoDatabaseService.GetConnection();
            connection.Open();
            var getOldCommand = connection.CreateCommand();
            getOldCommand.CommandText = @"SELECT Quantity FROM ProductsInOrders WHERE Id = $id";
            getOldCommand.Parameters.AddWithValue("$id",id);
            var oldQuantity = Convert.ToInt32(getOldCommand.ExecuteScalar());

            connection.Close();
            return oldQuantity;
        }

        public async Task InsertProduct(int orderId, int productId, int quantity)
        {
            var connection = AdoDatabaseService.GetConnection();
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
    }
}
