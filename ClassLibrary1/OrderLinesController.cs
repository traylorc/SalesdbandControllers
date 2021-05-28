using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesDbLib
{
    public class OrderLinesController
    {
        private static Connection connection { get; set; }

        private OrderLine FillOrderLineFromSqlRow(SqlDataReader reader)
        {
            var orderline = new OrderLine()
            {
                Id = Convert.ToInt32(reader["Id"]),
                OrderId = Convert.ToInt32(reader["OrderId"]),
                Product = Convert.ToString(reader["Product"]),
                Description = Convert.ToString(reader["Description"]),
                Price = Convert.ToDecimal(reader["Price"]),
                Quantity = Convert.ToInt32(reader["Quantity"]),
            };
            return orderline;
        }

        private int AddWithValue(Product product, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@partnbr", product.PartNbr);
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@unit", product.Unit);
            cmd.Parameters.AddWithValue("@photopath", (object)product.PhotoPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@vendorid", product.VendorId);
            return cmd.ExecuteNonQuery();
        }

        public List<Product> GetAll()
        {
            var sql = "SELECT * From Products;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var reader = cmd.ExecuteReader();
            var products = new List<Product>();
            while (reader.Read())
            {
                var product = FillProductFromSqlRow(reader);
                products.Add(product);
            }
            reader.Close();
            GetVendorForProducts(products);
            return products;
        }

        public Product GetByPK(int id)
        {
            var sql = $"SELECT * From Products where Id = @id;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }
            reader.Read();
            var product = FillProductFromSqlRow(reader);
            reader.Close();
            return product;
        }

        private void GetVendorForProducts(List<Product> products)
        {
            foreach (var product in products)
            {
                GetVendorForProduct(product);
            }
        }
        private void GetVendorForProduct(Product product)
        {
            var vendCtrl = new VendorsController(connection);
            product.Vendor = vendCtrl.GetByPK(product.VendorId);
        }

        public bool Create(Product product, string VendorCode)
        {
            var vendCtrl = new VendorsController(connection);
            var vendor = vendCtrl.GetByCode(VendorCode);
            product.VendorId = vendor.Id;
            return Create(product);
        }

        public bool Create(Product product)
        {
            var sql = "INSERT into Products "
                + "(PartNbr, Name, Price, Unit, PhotoPath, VendorId) "
                + " VALUES (@partnbr, @name, @price, @unit, @photopath, @vendorid); ";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var rowsAffected = AddWithValue(product, cmd);

            return (rowsAffected == 1);
        }

        public bool Change(Product product)
        {
            var sql = $"UPDATE Products Set " +
                $"PartNbr = @partnbr, " +
                $"Name = @name, " +
                $"Price = @price, " +
                $"Unit = @unit, " +
                $"PhotoPath = @photopath, " +
                $"VendorId = @vendorid " +
                $"Where Id = @id;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", product.Id);
            var rowsAffected = AddWithValue(product, cmd);

            return (rowsAffected == 1);

        }

        public bool Delete(Product product)
        {
            var sql = $"DELETE from Products " +
                $"Where Id = @id;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", product.Id);
            var rowsAffected = cmd.ExecuteNonQuery();

            return (rowsAffected == 1);
        }

        public ProductsController(Connection connection)
        {
            ProductsController.connection = connection;
        }
    }
}
