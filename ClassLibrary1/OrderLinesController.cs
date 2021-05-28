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

        private int AddWithValue(OrderLine orderLine, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@orderid", orderLine.OrderId);
            cmd.Parameters.AddWithValue("@product", orderLine.Product);
            cmd.Parameters.AddWithValue("@description", orderLine.Description);
            cmd.Parameters.AddWithValue("@quantity", orderLine.Quantity);
            cmd.Parameters.AddWithValue("@price", orderLine.Price);
            return cmd.ExecuteNonQuery();
        }

        public List<OrderLine> GetAll()
        {
            var sql = "SELECT * From OrderLines;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var reader = cmd.ExecuteReader();
            var orderLines = new List<OrderLine>();
            while (reader.Read())
            {
                var orderLine = FillOrderLineFromSqlRow(reader);
                orderLines.Add(orderLine);
            }
            reader.Close();
            GetOrderForOrderLines(orderLines);
            return orderLines;
        }

        public OrderLine GetByPK(int id)
        {
            var sql = $"SELECT * From OrderLines where Id = @id;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }
            reader.Read();
            var orderLine = FillOrderLineFromSqlRow(reader);
            reader.Close();
            return orderLine;
        }

        private void GetOrderForOrderLines(List<OrderLine> orderLines)
        {
            foreach (var orderLine in orderLines)
            {
                GetOrderForOrderLine(orderLine);
            }
        }
        private void GetOrderForOrderLine(OrderLine orderLine)
        {
            var OrdrCtrl = new OrdersController(connection);
            orderLine.Order = OrdrCtrl.GetByPk(orderLine.OrderId);
        }

        public bool Create(OrderLine orderLine, int orderId)
        {
            var ordrCtrl = new OrdersController(connection);
            var order = ordrCtrl.GetByPk(orderId);
            orderLine.OrderId = order.Id;
            return Create(orderLine);
        }

        public bool Create(OrderLine orderLine)
        {
            var sql = "INSERT into OrderLines "
                + "(OrderId, Product, Description, Quantity, Price) "
                + " VALUES (@orderid, @product, @description, @quantity, @price); ";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var rowsAffected = AddWithValue(orderLine, cmd);

            return (rowsAffected == 1);
        }

        public bool Change(OrderLine orderLine)
        {
            var sql = $"UPDATE OrderLines Set " +
                $"OrderId = @orderid, " +
                $"Product = @product, " +
                $"Description = @description, " +
                $"Quantity = @quantity, " +
                $"Price = @price " +
                $"Where Id = @id;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", orderLine.Id);
            var rowsAffected = AddWithValue(orderLine, cmd);

            return (rowsAffected == 1);

        }

        public bool Delete(OrderLine orderLine)
        {
            var sql = $"DELETE from OrderLines " +
                $"Where Id = @id;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", orderLine.Id);
            var rowsAffected = cmd.ExecuteNonQuery();

            return (rowsAffected == 1);
        }

        public OrderLinesController(Connection connection)
        {
            OrderLinesController.connection = connection;
        }
    }
}
