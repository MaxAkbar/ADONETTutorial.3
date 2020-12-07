using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Xml;

namespace ADONETTutorial._3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static void UpdateUsingSqlDataAdapter()
        {
            var sqlConnStringBuilder = new SqlConnectionStringBuilder
            {
                ApplicationName = "Sql Example",
                DataSource = "localhost",
                UserID = "sa",
                Password = "Pa$$w0rd1",
                InitialCatalog = "SqlExamples"
            };

            using (SqlConnection connection = new SqlConnection(sqlConnStringBuilder.ConnectionString))
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("select * from Table_1", connection);
                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);

                sqlDataAdapter.UpdateCommand = new SqlCommand("Update Table_1 set testdata=@p2 where id=@p1");

                sqlDataAdapter.UpdateCommand.Parameters.Add("@p1", SqlDbType.Int, 4, "Id");
                sqlDataAdapter.UpdateCommand.Parameters.Add("@p2", SqlDbType.NChar, 10, "testdata");

                DataTable dataTable = new DataTable();

                sqlDataAdapter.FillSchema(dataTable, SchemaType.Source);

                DataRow rowUpdate;

                rowUpdate = dataTable.NewRow();
                rowUpdate["Id"] = 20;
                rowUpdate["testdata"] = "updated";

                dataTable.Rows.Add(rowUpdate);
                dataTable.AcceptChanges();

                rowUpdate.SetModified();

                sqlDataAdapter.Update(dataTable);
            }
        }

        private static void PushDataTableToDatabase()
        {
            DataTable dataTable = new DataTable("Table_1");

            DataColumn columnId = new DataColumn();

            columnId.DataType = Type.GetType("System.Int32");
            columnId.AllowDBNull = true;
            columnId.ColumnName = "Id";

            DataColumn columntestdata = new DataColumn();

            columntestdata.DataType = Type.GetType("System.String");
            columntestdata.AllowDBNull = true;
            columntestdata.ColumnName = "testdata";

            dataTable.Columns.Add(columnId);
            dataTable.Columns.Add(columntestdata);

            DataRow row;
            for (int i = 20; i < 30; i++)
            {
                row = dataTable.NewRow();
                row["Id"] = i + 1;
                row["testdata"] = $"blah_{i + 1}";

                dataTable.Rows.Add(row);
            }

            DataSet dataSet = new DataSet("SqlExamples");

            dataSet.Tables.Add(dataTable);


            var sqlConnStringBuilder = new SqlConnectionStringBuilder
            {
                ApplicationName = "Sql Example",
                DataSource = "localhost",
                UserID = "sa",
                Password = "Pa$$w0rd1",
                InitialCatalog = "SqlExamples"
            };

            using (SqlConnection connection = new SqlConnection(sqlConnStringBuilder.ConnectionString))
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("select * from Table_1", connection);
                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);

                sqlDataAdapter.Update(dataTable);

                SqlCommand sqlCommandInsert = sqlCommandBuilder.GetInsertCommand();
            }
        }

        private static void LoadDataTable()
        {
            var sqlConnStringBuilder = new SqlConnectionStringBuilder
            {
                ApplicationName = "Sql Example",
                DataSource = "localhost",
                UserID = "sa",
                Password = "Pa$$w0rd1",
                InitialCatalog = "Northwind"
            };

            using (SqlConnection connection = new SqlConnection(sqlConnStringBuilder.ConnectionString))
            {
                SqlCommand sqlCommand = connection.CreateCommand();

                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "Select * from Customers";

                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                DataTable dataTable = new DataTable();

                dataTable.Load(sqlDataReader);
            }
        }

        private static void DataView()
        {
            var sqlConnStringBuilder = new SqlConnectionStringBuilder
            {
                ApplicationName = "Sql Example",
                DataSource = "localhost",
                UserID = "sa",
                Password = "Pa$$w0rd1",
                InitialCatalog = "Northwind"
            };

            using (SqlConnection connection = new SqlConnection(sqlConnStringBuilder.ConnectionString))
            {
                SqlDataAdapter custAdapter = new SqlDataAdapter("Select * from customers", connection);
                DataSet custDataSet = new DataSet();

                custAdapter.Fill(custDataSet, "Customers");

                DataTable custTable = custDataSet.Tables["Customers"];
                string filter = "Country = 'USA'";
                string sort = "ContactName";

                DataView custDataView = new DataView(custTable, filter, sort, DataViewRowState.CurrentRows);

                foreach (DataRowView rowView in custDataView)
                {
                    for (int i = 0; i < custDataView.Table.Columns.Count; i++)
                    {
                        Console.Write($"{rowView[i]}\t");
                    }

                    Console.WriteLine();
                }

                DataTable dataTable = custDataView.ToTable();
            }
        }

        private static void DataRelation()
        {
            var sqlConnStringBuilder = new SqlConnectionStringBuilder
            {
                ApplicationName = "Sql Example",
                DataSource = "localhost",
                UserID = "sa",
                Password = "Pa$$w0rd1",
                InitialCatalog = "Northwind"
            };

            using (SqlConnection connection = new SqlConnection(sqlConnStringBuilder.ConnectionString))
            {
                SqlDataAdapter custAdapter = new SqlDataAdapter("Select * from customers", connection);
                SqlDataAdapter ordAdapter = new SqlDataAdapter("Select * from orders", connection);


                DataSet dataSet = new DataSet("Northwind");

                custAdapter.Fill(dataSet, "Customers");
                ordAdapter.Fill(dataSet, "Orders");

                DataRelation relation = dataSet.Relations.Add("CustOrders",
                    dataSet.Tables["Customers"].Columns["CustomerID"],
                    dataSet.Tables["Orders"].Columns["CustomerID"]);

                foreach (DataRow primaryRow in dataSet.Tables["Customers"].Rows)
                {
                    Console.WriteLine(primaryRow["CustomerID"]);

                    foreach (DataRow SecondaryRow in primaryRow.GetChildRows(relation))
                    {
                        Console.WriteLine($"\t{SecondaryRow["OrderID"]}");
                    }
                }
            }
        }
    }
}

