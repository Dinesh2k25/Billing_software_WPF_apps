using Microsoft.Win32;
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
using Microsoft.Win32;

namespace BS
{
    public partial class BillPrintWindow : Window
    {
        private BillRepository _repo;

        public BillPrintWindow()
        {
            InitializeComponent();
            _repo = new BillRepository("Data/store.db");
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnGeneratePdf_Click(object sender, RoutedEventArgs e)
        {

            // Get Bill data
            var bill = _repo.GetBillById(txtBillID.Text); // Example BillID

            if (bill == null)
            {
                MessageBox.Show("Bill not found!");
                return;
            }

            // Ask where to save
            var dlg = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"Bill_{bill.BillID}.pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                PdfInvoiceGenerator.GeneratePdf(dlg.FileName, bill);
                MessageBox.Show("✅ Bill PDF generated successfully!");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}