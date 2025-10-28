using System;
using System.Collections.Generic;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BS
{
    /// <summary>
    /// Interaction logic for Shopupdate.xaml
    /// </summary>
    public partial class Shopupdate : Window
    {
        // Public properties to access input values

        
        public Shopupdate()
        {
            InitializeComponent();

           
        }
        public Shopupdate(string date, string company, string gstno, string address, string phone)
        {
            InitializeComponent();
            txtDate.Text = "Shop details"+'('+date+')';
            txtCompany.Text = company;
           
            txtGSTNO.Text = gstno;
            txtAddress.Text = address;
            txtPhone.Text = phone;
           // isEditMode = true;
            //btnSave.Content = "Update";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentDateTime = DateTime.Now;
            
            string Cn= txtCompany.Text.Trim();
            
            string gstno = txtGSTNO.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();



              if (string.IsNullOrWhiteSpace(txtAddress.Text) || string.IsNullOrWhiteSpace(txtCompany.Text) ||  string.IsNullOrWhiteSpace(txtGSTNO.Text) || string.IsNullOrWhiteSpace(txtPhone.Text) || string.IsNullOrWhiteSpace(pass.Text))
               {
                   MessageBox.Show("Please enter all details.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                   return;
               }
            




            if (pass.Text == "thispass")
            {
                var popups = new Billing(currentDateTime.ToString("dd MMMM yyyy"), Cn, gstno, address, phone);

                Close();
            }
            else
            {
                pass.Background = new SolidColorBrush(Colors.LightCoral);
                pass.Text = "Enter Correct password";
            }
           
        }
        

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            
            Close();
        }
    }
}
