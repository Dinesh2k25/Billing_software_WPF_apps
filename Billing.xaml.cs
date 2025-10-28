using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BS
{
    /// <summary>
    /// Interaction logic for Billing.xaml
    /// </summary>
    public partial class Billing : Window
    {
        public Billing()
        {
            InitializeComponent();
            InitializeDatabase();
            
        }
        public static int billcount=0;
        public static List<string> itemList = new List<string>() { };
        public static string dbPath = "Data\\store.db",billno="",billNO="";
        public System.Data.DataTable orderTable, ShopTable , stockTable = new System.Data.DataTable();
        

        private void InitializeDatabase()
        {
            try
            {

                if (!Directory.Exists("Data"))
                    Directory.CreateDirectory("Data");


                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();

                    string STOCK = @"
             CREATE TABLE IF NOT EXISTS STOCK (
                 ID TEXT NOT NULL,
                 Name TEXT NOT NULL
             );";

                    string SHOP = @"
             CREATE TABLE IF NOT EXISTS SHOP(
                 Date TEXT NOT NULL,
                 CompanyName TEXT NOT NULL,
               
                 GSTNO TEXT NOT NULL,
                 Address TEXT NOT NULL,
                 Phone TEXT NOT NULL
             );";







                    string createBill = @"
              CREATE TABLE IF NOT EXISTS Bill (
      SNO INTEGER ,
  BillID Text NOT NULL,
 Date TEXT NOT NULL,
  CName TEXT NOT NULL,
  CCompanyName TEXT,
  CPhone TEXT NOT NULL,
  CAddress TEXT NOT NULL,
  CEmail TEXT,
  orderItems text,
  Total REAL
  );";




                    /*  string createBillDetails = @"
                  CREATE TABLE IF NOT EXISTS BillDetails (
                      BillID INTEGER PRIMARY KEY AUTOINCREMENT,
                      ProductID INTEGER NOT NULL,
                      Quantity INTEGER NOT NULL,
                      Price REAL NOT NULL,
                      FOREIGN KEY(BillID) REFERENCES Bills(BillID),
                      FOREIGN KEY(ProductID) REFERENCES Products(ProductID)
                  );";


                      using (var cmd = new SqliteCommand(createProducts, connection)) cmd.ExecuteNonQuery();
                      using (var cmd = new SqliteCommand(createCustomers, connection)) cmd.ExecuteNonQuery();*/

                    using (var cmd = new SqliteCommand(createBill, connection)) cmd.ExecuteNonQuery();
                    using (var cmd = new SqliteCommand(STOCK, connection)) cmd.ExecuteNonQuery();
                    using (var cmd = new SqliteCommand(SHOP, connection)) cmd.ExecuteNonQuery();
                    connection.Close();
                }
                stockTable = Gettable("STOCK");
                ShopTable = Gettable("SHOP");
                itemList = GetColumnValues(stockTable, "Name");


                productlist.ItemsSource = itemList.ToList();
                orderTable = new DataTable();

                orderTable.Columns.Add("ID", typeof(string));
                orderTable.Columns.Add("Name", typeof(string));
                orderTable.Columns.Add("CQFT", typeof(string));
                orderTable.Columns.Add("Rate", typeof(float));


                orderTable.Columns.Add("GST", typeof(float));
                orderTable.Columns.Add("Amount", typeof(float));
                selectedorder.ItemsSource = orderTable.DefaultView;
                selectedorder.CanUserAddRows = false; // remove blank row
                Dateup.Content ="Date : " + DateTime.Now.ToString("dd MMMM yyyy");
               
                if (ShopTable.Rows.Count > 0)
                {
                    GSTNO.Content = "GST NO."+ShopTable.Rows[0]["GSTNO"].ToString();
                    Shopname.Content = ShopTable.Rows[0]["CompanyName"].ToString();
                    Shopaddress.Content = ShopTable.Rows[0]["Address"].ToString() +".  Call :"+ ShopTable.Rows[0]["Phone"].ToString();
                    billcount = Convert.ToInt32(GetRecordCount()) ;
                   
                    BILLNO.Content = "Invoice NO. :" +GenerateBillId().ToString();

                }

                GST_update.ItemsSource = new List<string> { "None","Add","Minus"};
            }
            catch (Exception ex) { MessageBox.Show(ex + "en1"); }
            


        }
        public List<string> GetColumnValues(DataTable table, string columnName)
        {
            return table.AsEnumerable()
                        .Select(row => row.Field<string>(columnName))
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct()
                        .ToList();
        }


        private static string ConvertDigitToLetter(char digit)
        {
            // 1 → A, 2 → B, … 9 → I, 0 → J
            switch (digit)
            {
                case '1': return "A";
                case '2': return "B";
                case '3': return "C";
                case '4': return "D";
                case '5': return "E";
                case '6': return "F";
                case '7': return "G";
                case '8': return "H";
                case '9': return "I";
                case '0': return "J";
                default: return "X"; // fallback
            }
        }


        public static string GenerateBillId()
            
        {
                       DateTime now = DateTime.Now;

            // 1️⃣ Month as A–L
            char monthLetter = (char)('A' + now.Month - 1);


            // 2️⃣ Year (last 2 digits)
            string yearPart = (now.Year % 100).ToString("D2");

            // 3️⃣ Day: convert each digit to A–J
            string dayDigits = now.Day.ToString("D2");
            string dayPart = ConvertDigitToLetter(dayDigits[0]) + ConvertDigitToLetter(dayDigits[1]);

            // 4️⃣ Count (001–999)
            string countPart =(1+ billcount).ToString("D3");

            // Combine all parts
            string billId = $"{monthLetter}{yearPart}{dayPart}{countPart}";

            // Increment count for next bill
        

            return billId;
        }



        public static string Ccn="", cn="", cno="", ca = "", cm = "";

public DataTable Gettable(string tableName)
    {
            
        // Create an empty DataTable to hold the results
        DataTable dataTable = new DataTable();

            // Use 'using' statements to ensure connections and resources are properly closed
            using (var connection = new SqliteConnection($"Data Source={dbPath};"))
            {
                connection.Open();

                // Construct the SQL query to select all data from the specified table
                string selectSql = $"SELECT * FROM {tableName}";

                using (var command = new SqliteCommand(selectSql, connection))
                using (var reader = command.ExecuteReader())
                {
                    // Load the schema (column names and types) from the reader
                    dataTable.Load(reader);
                }
            }
        

            return dataTable;
    }
    private void ub_Click(object sender, RoutedEventArgs e)
        {
            Shopupdate();
        }
       
        
        private void Atc_Click(object sender, RoutedEventArgs e)
        {
           
           /* using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM STOCK";

                using (var cmd = new SqliteCommand(selectQuery, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    MessageBox.Show("hi12");
                    

                    MessageBox.Show("12");
                    /* Bind and format DataGrid

                    selectedorder.ItemsSource = stockTable.DefaultView;

                    selectedorder.CanUserAddRows = false; // remove blank row

                }
                connection.Close(); 
            }*/
         
            if (ProductID.Text == "" || productlist.SelectedItem == null || string.IsNullOrEmpty(Rate.Text )|| string.IsNullOrEmpty(txtGST.Text )||string.IsNullOrEmpty(Amount1.Text))
            {
                MessageBox.Show("Please fill all the fields.");
                return;
            }
            else
            {
                   orderTable.Rows.Add(
                   
                    ProductID.Text.ToString(),
                    productlist.SelectedItem.ToString(),
                    CQFT.Text == "" ? "0" : (CQFT.Text), 
                    Rate.Text == "" ? 0 : (float)Math.Round(float.Parse(Rate.Text), 2),
                    txtGST.Text == "" ? 0 : float.Parse(txtGST.Text),
                    Amount1.Text == "" ? 0 : (float)Math.Round(float.Parse(Amount1.Text), 2)

                // Fix: Convert int? to int for multiplication
                );
                ProductID.Clear(); productlist.SelectedItem = null; Rate.Clear();
                Amount1.Clear();
                txtGST.Clear();
                CQFT.Clear();
                selectedorder.ItemsSource = orderTable.DefaultView;

                selectedorder.CanUserAddRows = false; // remove blank row
            }
            orderTable.AcceptChanges();
            decimal amt = Convert.ToDecimal(orderTable.Compute("SUM(Amount)", string.Empty));
            Amount.Content= "Total :"+amt.ToString();

        }

        private void us_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM STOCK;";
                
                using (var cmd = new SqliteCommand(selectQuery, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    stockTable = new System.Data.DataTable();
                    stockTable.Load(reader);
                   
                    if (stockTable != null)
                        Application.Current.Dispatcher.Invoke(() =>
                    {
                        var popup = new Stock(stockTable);
                        
                        
                        popup.Show();
                    });
                    else {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                           
                            var popup = new Stock();
                            popup.Show();
                        });
                    }

                        // Bind and format DataGrid


                }

                connection.Close();
            }
           

        }

       

        

      





        private void productlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            productlist.ItemsSource = itemList.ToList();
            if (productlist.SelectedItem == null)
            {
                ProductID.Text = "";
                
                return;
            }
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();



                // 2️⃣ Check if data exists
                string selectQuery = $"SELECT * FROM STOCK Where Name = '{productlist.SelectedItem.ToString()}';";
                using (var cmd = new SqliteCommand(selectQuery, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ProductID.Text = reader.GetString(0);
                       
                    }
                    else
                    {
                        ProductID.Text = "";
                       

                    }
                }
                connection.Close();
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void txtGST_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            calcule();
            
        }
        private void calcule()





        {
             Rate.Text= string.IsNullOrEmpty(Rate.Text) ? "0" :Rate.Text;
             txtGST.Text = string.IsNullOrEmpty(txtGST.Text) ? "0" : txtGST.Text;
             CQFT.Text = string.IsNullOrEmpty(CQFT.Text) ? "0" : CQFT.Text;
            
             Amount1.Text = Amount1.Text == "" ? "0" : Amount1.Text;

            //  (float)Math.Round(float.Parse(Rate.Text), 2);
           
            GST_update.SelectedItem = GST_update.SelectedItem == null ? "None" : GST_update.SelectedItem;

            

            float temp = (float.Parse(Rate.Text)) * (float.Parse(CQFT.Text));
                temp = (float)Math.Round(temp, 2);
            Amount1.Text = temp.ToString();
            if (GST_update.SelectedItem.ToString() == "Add")
                    Amount1.Text = (temp + (temp * float.Parse(txtGST.Text) / 100)).ToString();
                else if (GST_update.SelectedItem.ToString() == "Minus")
                    Amount1.Text = (temp - (temp * float.Parse(txtGST.Text) / 100)).ToString();
                else
                    Amount1.Text = temp.ToString();
          
        }

        private static bool IsTextNumeric(string text)
        {
            return float.TryParse(text, out _);
        }
        
        private void GST_update_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            calcule();
            if (!string.IsNullOrEmpty(txtGST.Text))
            {
                float temp = (float.Parse(Rate.Text)) * (float.Parse(CQFT.Text));
                temp = (float)Math.Round(temp, 2);

                if (GST_update.SelectedItem.ToString() == "Add")
                    Amount1.Text = (temp + (temp * float.Parse(txtGST.Text) / 100)).ToString();
                else if (GST_update.SelectedItem.ToString() == "Minus")
                    Amount1.Text = (temp - (temp * float.Parse(txtGST.Text) / 100)).ToString();
                else
                    Amount1.Text =  temp.ToString();
            }

        }

        private void Rate_TextChanged(object sender, TextChangedEventArgs e)
        {
            calcule();
               
        }

        private void CQFT_TextChanged(object sender, TextChangedEventArgs e)
        {
            calcule();

        }

        private void Rate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void CQFT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void txtGST_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void Amount1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void btnLoadProducts_Click(object sender, RoutedEventArgs e)
        {
           MessageBox.Show("Load Products clicked");
        }



         public static string date="", company="", gstno="", address="", phone = "",cqft="";
        public void Shopupdate()
        {

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

              
               
                // 2️⃣ Check if data exists
                string selectQuery = "SELECT * FROM SHOP LIMIT 1";
                using (var cmd = new SqliteCommand(selectQuery, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // ✅ Data exists — read it
                        
                        company = reader.GetString(1);
                       
                        gstno = reader.GetString(2);
                        address = reader.GetString(3);
                        phone = reader.GetString(4);
                      

                        // 3️⃣ Open popup and pass values
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var popup = new Shopupdate(date, company, gstno, address, phone);
                             popup.Show();
                        });
                    }
                    else
                    {
                        // 🟡 No data — open empty popup
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var popup = new Shopupdate();
                            popup.Show();
                        });
                    }
                    
                }
                connection.Close();    
            }
        }


        //For the SHOP UPDATE:
        public Billing(string date, string company, string gstno, string address, string phone)
        {
            InitializeComponent();
            try
            {
                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();
                  
                    // Use a single command object for multiple operations with updated CommandText.
                    using (var cmd = connection.CreateCommand())
                    {
                        // 1. Delete all records from the table.
                        cmd.CommandText = "DELETE FROM SHOP;"; 
                        cmd.ExecuteNonQuery();
                        // 2. Insert new records using a parameterized query.
                        cmd.CommandText = "INSERT INTO Shop (Date,CompanyName,GSTNO,Address,Phone) VALUES (@Date,@Company,@GSTNO,@Address,@Phone)";
                       
                        // Example of adding parameters and executing the insert command.
                        // You would typically do this inside a loop or a function.
                        cmd.Parameters.AddWithValue("@Date", date);
                        cmd.Parameters.AddWithValue("@Company", company);
                       
                        cmd.Parameters.AddWithValue("@GSTNO", gstno);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.ExecuteNonQuery();

                        // Add more records as needed...
                        cmd.Parameters.Clear(); // Clear parameters for the next insert
                       
                        // ... and so on for each new row.
                    }connection.Close();
                }

                ShopTable = Gettable("SHOP");
                if (ShopTable.Rows.Count > 0)
                {
                  
                   gstno = ShopTable.Rows[0]["GSTNO"].ToString();
                    GSTNO.Content =gstno;
                    Shopname.Content = ShopTable.Rows[0]["CompanyName"].ToString();
                    Shopaddress.Content = ShopTable.Rows[0]["Address"].ToString() + ShopTable.Rows[0]["Phone"].ToString();

                }
            }
            catch (SqliteException ex)
            {
                // Handle any potential exceptions
                MessageBox.Show($"Database error: {ex.Message}");
            }


        }

        //FOR THE STOCK UPDATE
        public void UPDATESTOCK(DataTable dt)
        {

            stockTable = dt;

           
            // Establish the connection outside the loop for efficiency.
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();


                using (var cmd = connection.CreateCommand())
                {
                    // 1. Delete all records from the table.
                    cmd.CommandText = "DELETE FROM STOCK;";
                    cmd.ExecuteNonQuery();
                }


                connection.Close();
                connection.Open();

                // Wrap multiple inserts in a transaction for better performance.
                using (var transaction = connection.BeginTransaction())
                {
                    // Define the parameterized INSERT statement once.
                    // The values are the named parameters defined below.

                    string insertSql = "INSERT INTO STOCK (Id, Name) VALUES ($Id, $Name);";

                    using (var command = new SqliteCommand(insertSql, connection, transaction))
                    {
                        // Add the parameters to the command object once.
                     
                        command.Parameters.Add(new SqliteParameter("$Id", SqliteType.Text));
                        command.Parameters.Add(new SqliteParameter("$Name", SqliteType.Text));
                      
                        // Loop through each row of the DataTable.
                        foreach (DataRow row in stockTable.Rows)
                        {
                            // Assign the value from the DataTable row to each parameter.

                            command.Parameters["$Id"].Value = row["Id"];
                            command.Parameters["$Name"].Value = row["Name"];
                           

                            // Execute the insert for the current row.
                            command.ExecuteNonQuery();

                        }
                    }


                    // Commit the transaction to save all changes at once.
                    transaction.Commit(); connection.Close();


                }
                

               
            }
           
            stockTable = Gettable("STOCK");
            
            itemList = GetColumnValues(stockTable, "Name");
            productlist.ItemsSource = itemList.ToList();
            

        }



        private void gb_Click(object sender, RoutedEventArgs e)
        {
       
            Ccn = Cgstno.Text; cn = CustomerName.Text; cno = Conductno.Text; ca = Address.Text; cm = MailId.Text;cqft=CQFT.Text ;
            

            if (string.IsNullOrEmpty(Ccn) || string.IsNullOrEmpty(cn) || string.IsNullOrEmpty(cno) || string.IsNullOrEmpty(ca) || string.IsNullOrEmpty(cm))
            {
                MessageBox.Show("Please fill all the fields.");
                return;
            }
            else
            {
                if (orderTable.Rows.Count<1)
                {
                    MessageBox.Show("Please add items to the order.");
                  
                }
                else
                {
                    string orderlist = "";
                    
                    decimal amt = Convert.ToDecimal(orderTable.Compute("SUM(Amount)",string.Empty));
                    orderTable.AcceptChanges();

                    foreach (DataRow row in orderTable.Rows)
                    {
                        string id = row.Field<string>("ID");
                        string name = row.Field<string>("Name");
                        string rate = row.Field<float>("Rate").ToString();
                        string CQFT = row.Field<string>("CQFT").ToString();
                        string GST = row.Field<float>("GST").ToString();
                        string Amt = row.Field<float>("Amount").ToString();

                        orderlist = orderlist + id

                            + "," + name + "," + rate + "," + CQFT +","+ GST+","+Amt +"||";
                       
                    }
         






                   
                        
                        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO BIll (SNO,BILLID,Date,CName,CCompanyName,CPhone,CAddress,CEmail,orderItems,Total) VALUES (@SNO,@BILLID,@Date,@CName,@CCompanyName,@CPhone,@CAddress,@CEmail,@orderItems,@Amt)";
                        billcount= Convert.ToInt32(GetRecordCount()) ;
                        billNO =GenerateBillId().ToString();
                       

                        using (var cmd = new SqliteCommand(insertQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@SNO", billcount);
                            cmd.Parameters.AddWithValue("@BILLID", billNO);
                            cmd.Parameters.AddWithValue("@Date", DateTime.Now.ToString("dd MMMM yyyy") );
                            ;
                            cmd.Parameters.AddWithValue("@CName", cn); 
                            cmd.Parameters.AddWithValue("@CCompanyName", Ccn);
                            cmd.Parameters.AddWithValue("@CPhone", cno);
                            cmd.Parameters.AddWithValue("@CAddress", ca);
                            cmd.Parameters.AddWithValue("@CEmail", cm);
                            cmd.Parameters.AddWithValue("@orderItems", orderlist);
                            cmd.Parameters.AddWithValue("@Amt", amt);
                              
                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                        BILLNO.Content = "BILLNO"+billNO;
                        
                        BillRepository BP= new BillRepository();
                    BP.PRINT(ShopTable.Rows[0]["Address"].ToString() + "\nCall :" + ShopTable.Rows[0]["Phone"].ToString(), billNO,GSTNO.Content.ToString(), cn,Ccn, ca, cno,  orderTable, amt.ToString());
                }


               

            }
        }

        private long GetRecordCount()
        {

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();


                // SQL query to get the count of records in a table
                string sql = "SELECT COUNT(*) FROM Bill"; // Replace YourTableName

                using (var command = new SqliteCommand(sql, connection))
                {
                    long count = (long)command.ExecuteScalar();
                    return count;
                }

            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove the selected item from the DataGrid's ItemsSource

            if (orderTable == null)
            {
                MessageBox.Show("Data table is not loaded yet.");
                return;
            }
            var button = sender as Button;
            if (button == null || button.Tag == null)
                return;

            string productId = button.Tag.ToString();
          
            // 🔹 Find matching row in DataTable
            var rowToDelete = orderTable.AsEnumerable()
                                          .FirstOrDefault(r => r.Field<string>("ID") == productId);
            
            if (rowToDelete != null)
            {
                // 🔹 Remove from DataTable
                orderTable.Rows.Remove(rowToDelete);

                // 🔹 Refresh DataGrid
                selectedorder.Items.Refresh();

                orderTable.AcceptChanges();
               

            }
            if (orderTable.Rows.Count == 0)
            {
                Amount.Content = "0";
                return;
            }
            decimal amt = Convert.ToDecimal(orderTable.Compute("SUM(Amount)", string.Empty));
            Amount.Content = "Total :"+amt.ToString();

        }

    }


}
/*

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();



                // 2️⃣ Check if data exists
                string selectQuery = "SELECT count(*) FROM STOCK ;";
                using (var cmd = new SqliteCommand(selectQuery, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // ✅ Data exists — read it
                        string date = reader.GetString(0);
                        MessageBox.Show("Data exists: " + date);

                    }
                }
            }*/

