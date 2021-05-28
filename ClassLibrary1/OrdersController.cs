using SalesDbLib;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesDbLib
{
    public class OrdersController
    {
        private static Connection connection { get; set; }

        public OrdersController(Connection connection)
        {
            OrdersController.connection = connection;
        }


        public Order GetByCustomerId(int customerId)
        {
            var sql = " SELECT * from Orders where CustomerId = @customerid;";
            var cmd = new SqlCommand(sql, connection.Sqlconn);
            cmd.Parameters.AddWithValue("@customerId", customerId);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }
            reader.Read();
            var order = FillOrderFromSqlRow(reader);
            reader.Close();
            return order;
        }

        private void FillCmdParFromSqlRowsForOrders(SqlCommand cmd, Order order)
        {
            cmd.Parameters.AddWithValue("@customerid", order.CustomerId);
            cmd.Parameters.AddWithValue("@description", order.Description);
            cmd.Parameters.AddWithValue("@date", order.Date);

        }

        public bool Create(Order order, int CustomerCustomerId)
        {
            var custctrl = new CustomersController(connection);
            var customer = custctrl.GetByPK(CustomerCustomerId);
            order.CustomerId = customer.Id;
            return Create(order);
        }

        public bool Create(Order order)
        {
            var sql = $" Insert into Orders" +
                " (CustomerId, Date, Description)" +
                "Values (@customerid, @date, @description);";
            var cmd = new SqlCommand(sql, connection.SqlConn);

            FillCmdParFromSqlRowsForOrders(cmd, order);

            var rowsaffected = cmd.ExecuteNonQuery();
            return (rowsaffected == 1);
        }

        public bool Change(Order order)
        {
            var sql = $" Update Orders set" +
                    "values (@customerid, @date, @description);";
            var cmd = new SqlCommand(sql, connection.SqlConn);

            FillCmdParFromSqlRowsForOrders(cmd, order);

            var rowsaffected = cmd.ExecuteNonQuery();
            return (rowsaffected == 1);
        }

        public  bool Remove(int id)
        {
            var sql = $"Delete from orders " +
                    " where Id = @id; ";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", id);
            var rowsaffected = cmd.ExecuteNonQuery();

            return (rowsaffected == 1);
        }

        private void GetCustomerForOrder(Order order)
        {
            var custctrl = new CustomersController(connection);
            order.customer = custctrl.GetByPK(order.CustomerId);
        }

        private Order FillOrderFromSqlRow(SqlDataReader reader)
        {
            var product = new Order()
            {
                Id = Convert.ToInt32(reader["Id"]),
                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                Date = Convert.ToDateTime(reader["Date"]),
                Description = Convert.ToString(reader["Description"])
                
            };
            return product;
        }

        public List<Order> GetAllOrders()
        {
            var sql = $" SELECT * from Orders; ";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var reader = cmd.ExecuteReader();
            var orders = new List<Order>();

            while (reader.Read())
            {
                var order = FillOrderFromSqlRow(reader);
                orders.Add(order);
            }
            reader.Close();
            return orders;
        }

        public Order GetByPk(int id)
        {
            var sql = $"SELECT * From Orders where id = {id};";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }
            reader.Read();
            var order = FillOrderFromSqlRow(reader);
            reader.Close();
            return order;
        }
    }
}
