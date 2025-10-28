using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BS
{
    /// <summary>
    /// Interaction logic for Stock.xaml
    /// </summary>
    public partial class Stock : Window
    {
        private System.Data.DataTable dt = new System.Data.DataTable();
        public Stock()
        {
            InitializeComponent();
        }
        public Stock(DataTable td)
        {
           
            InitializeComponent();
            this.dt = td;

            products.ItemsSource = dt.DefaultView;
            products.CanUserAddRows = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove the selected item from the DataGrid's ItemsSource

            if (dt == null)
            {
                MessageBox.Show("Data table is not loaded yet.");
                return;
            }
            var button = sender as Button;
            if (button == null || button.Tag == null)
                return;

            string productId = button.Tag.ToString();

            // 🔹 Find matching row in DataTable
            var rowToDelete = dt.AsEnumerable()
                                          .FirstOrDefault(r => r.Field<string>("ID") == productId);

            if (rowToDelete != null)
            {
                // 🔹 Remove from DataTable
                dt.Rows.Remove(rowToDelete);

                // 🔹 Refresh DataGrid
                products.Items.Refresh();

                dt.AcceptChanges();


            }
        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }

        private void add_click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else {

                dt.Rows.Add(txtID.Text.ToString(), txtName.Text.ToString());
                txtID.Clear();txtName.Clear();;
            }     
            dt.AcceptChanges ();

        }

        private void Update_click(object sender, RoutedEventArgs e)
        {
           dt.AcceptChanges();
            Billing pop = new Billing();
           
            pop.UPDATESTOCK(dt);
            
         Close();





        }
    }
}
