using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesDbLib
{
    public class OrdersController
    {

    }

    public Order GetByCustomerId(string customerId)
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
        cmd.Parameters.AddWithValue("@customerid", Order.CustomerId);
        cmd.Parameters.AddWithValue("@description", Order.Description);
        cmd.Parameters.AddWithValue("@date", Order.Date);
       
    }

    public bool Create(Order order, int CustomerCustomerId)
    {
        var custctrl = new CustomersController(connection);
        var customer = custctrl.GetByPK(CustomerCustomerId);
        order.CutomerId = customer.Id;
        return Create(order);
    }

    public bool Create(Order order)
    {
        var sql = " Insert into Orders" +
            " (CustomerId, Date, Description)" +
            "Values (@customerid, @date, @description);";
        var cmd = new SqlCommand(sql, connection.SqlConn);

        FillCmdParFromSqlRowsForOrders(cmd, order);

        var rowsaffected = cmd.ExecuteNonQuery();
        return (rowsaffected == 1);
    }

    public  bool
}
