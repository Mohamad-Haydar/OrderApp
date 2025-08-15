using OrderApp.Exceptions;
using OrderApp.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OrderApp.Services
{
    public class ProductsServices
    {
        public async Task<ObservableCollection<Product>> GetProducts()
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                ObservableCollection<Product> ProductList = [];
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT * From Products";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ProductList.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        Price = reader.GetFloat(3),
                        Quantity = reader.GetInt32(4),
                        ImageUrl = reader.GetString(5)
                    });
                }
                connection.Close();
                return ProductList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetProducts: {ex}");
                throw new DataAccessException("Failed to load products from the database.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<int> GetStuckQuantity(int productId)
        {
            var connection = AdoDatabaseService.GetConnection();

            try
            {
                connection.Open();
                // CHECK the available stock first
                using var checkStockCommand = connection.CreateCommand();
                checkStockCommand.CommandText = @"SELECT Quantity FROM Products WHERE Id = $productId";
                checkStockCommand.Parameters.AddWithValue("$productId", productId);
                var availableStock = Convert.ToInt32(checkStockCommand.ExecuteScalar());

                connection.Close();

                return availableStock;
            }
            catch (Exception ex)
            {
                // Log technical details
                Debug.WriteLine($"DB Error: {ex}");

                // Either throw a custom exception:
                throw new DataAccessException("Could not retrieve products.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<float> GetProductsInOrders(ObservableCollection<ProductsInOrders> products, Order o)
        {
            float t = 0;
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT pio.Id, pio.OrderId, pio.Quantity,
                p.Id AS ProductId, p.Name, p.Description, p.Price, p.Quantity AS StockQuantity, p.ImageUrl
                FROM ProductsInOrders pio
                JOIN Products p ON pio.ProductId = p.Id
                WHERE pio.OrderId = $orderId
                Order BY p.Id DESC;";

                command.Parameters.AddWithValue("$orderId", o.Id);


                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new ProductsInOrders
                    {
                        Id = reader.GetInt32(0),
                        OrderId = reader.GetInt32(1),
                        Quantity = reader.GetInt32(2),
                        Product = new Product
                        {
                            Id = reader.GetInt32(3),
                            Name = reader.GetString(4),
                            Description = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Price = reader.GetFloat(6),
                            Quantity = reader.GetInt32(7),
                            ImageUrl = reader.GetString(8)
                        }
                    });
                    t += reader.GetInt32(2) * reader.GetFloat(6);
                }
                connection.Close();
                return t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetProductsInOrders: {ex}");
                throw new DataAccessException("Failed to retrieve products for the order.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task UpdateProductStock(int difference, int productId)
        {
            /*
             * when the difference is negative that means i am returning product to the stock so 
             * the stock increase
            */
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                var updateProductCommand = connection.CreateCommand();
                updateProductCommand.CommandText = @"UPDATE Products SET Quantity = Quantity - $difference WHERE Id = $productId";
                updateProductCommand.Parameters.AddWithValue("$difference", difference);
                updateProductCommand.Parameters.AddWithValue("$productId", productId);
                updateProductCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in UpdateProductStock: {ex}");
                throw new DataAccessException("Failed to update product stock.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task AddProductAsync(string name, string description, float price, int quantity)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || price <= 0 || quantity <= 0)
                throw new ValidationException("All product fields must be filled and valid.");

            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Products (Name, Description, Price, Quantity)
                VALUES ($name, $description, $price, $quantity);";

                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$description", description ?? string.Empty);
                command.Parameters.AddWithValue("$price", price);
                command.Parameters.AddWithValue("$quantity", quantity);

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in AddProductAsync: {ex}");
                throw new DataAccessException("Failed to add the product.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task UpdateProductImage(Product product)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                connection.Open();
                var updateProductCommand = connection.CreateCommand();
                updateProductCommand.CommandText = @"UPDATE Products SET ImageUrl = $image WHERE Id = $productId";
                updateProductCommand.Parameters.AddWithValue("$image", product.ImageUrl);
                updateProductCommand.Parameters.AddWithValue("$productId", product.Id);
                await updateProductCommand.ExecuteNonQueryAsync();
                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in UpdateProductImage: {ex}");
                throw new DataAccessException("Failed to update product image.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
