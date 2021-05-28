using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesDbLib
{
    public class CustomersController
    {
        private static Connection connection { get; set; }

        private Customer FillCustomerFromSqlRow(SqlDataReader reader)
        {
            var customer = new Customer()
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = Convert.ToString(reader["Name"]),
                City = Convert.ToString(reader["City"]),
                State = Convert.ToString(reader["State"]),
                Sales = Convert.ToDecimal(reader["Sales"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
            return customer;
        }

        private int AddWithValue(Customer customer, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@name", customer.Name);
            cmd.Parameters.AddWithValue("@city", customer.City);
            cmd.Parameters.AddWithValue("@state", customer.State);
            cmd.Parameters.AddWithValue("@sales", customer.Sales);
            cmd.Parameters.AddWithValue("@isactive", customer.IsActive);
            return cmd.ExecuteNonQuery();
        }

        public List<Customer> GetAll()
        {
            var sql = "SELECT * From Customers;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var reader = cmd.ExecuteReader();
            var customers = new List<Customer>();
            while (reader.Read())
            {
                var customer = FillCustomerFromSqlRow(reader);
                customers.Add(customer);
            }
            reader.Close();
            return customers;
        }

        public Customer GetByPK(int id)
        {
            var sql = $"SELECT * From Customer where Id = {id};";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }
            reader.Read();
            var customer = FillCustomerFromSqlRow(reader);
            reader.Close();
            return customer;
        }

        public bool Create(Customer customer)
        {
            var sql = $"INSERT into Customers " +
                $" (Name, City, State, Sales, IsActive) VALUES " +
                $"(@name, @city, @state, @sales, @isactive)";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            var rowsAffected = AddWithValue(customer, cmd);

            return (rowsAffected == 1);
        }

        public bool Change(Customer customer)
        {
            var sql = $"UPDATE Customers Set " +
                $"Name = @name, " +
                $"City = @city, " +
                $"State = @state, " +
                $"Sales = @sales, " +
                $"IsActive = @isactive " +
                $"Where Id = @id;";
            var cmd = new SqlCommand(sql, connection.SqlConn);
            cmd.Parameters.AddWithValue("@id", customer.Id);
            var rowsAffected = AddWithValue(customer, cmd);

            return (rowsAffected == 1);
        }

        public bool Delete(Customer customer)
        {
            var sql = $"DELETE from Customers " +
                $"Where Id = @id;";
            var sqlcmd = new SqlCommand(sql, connection.SqlConn);
            sqlcmd.Parameters.AddWithValue("@id", customer.Id);
            var rowsAffected = sqlcmd.ExecuteNonQuery();

            return (rowsAffected == 1);
        }

        public CustomersController(Connection connection)
        {
            CustomersController.connection = connection;
        }
    }
}
