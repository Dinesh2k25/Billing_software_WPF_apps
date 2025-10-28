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
using System.Windows.Markup;

namespace BS
{
    /// <summary>
    /// Interaction logic for test1.xaml
    /// </summary>
    public partial class test1 : Window
    {

        private readonly BillRepository _billRepo = new BillRepository("Data//store.db");
        private FlowDocument _billDocument ;
        public test1()
        {
            InitializeComponent();
            // Example bill ID
            GenerateBillDocument("J25AA016");
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (_billDocument == null)
            {
                MessageBox.Show("No bill to print.");
                return;
            }

            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                dlg.PrintDocument(((IDocumentPaginatorSource)_billDocument).DocumentPaginator, "Bill Print");
            }
        }
        private void GenerateBillDocument(string billId)
        {
            MessageBox.Show(billId);
            DataTable bill = null;//_billRepo.GetBill(billId);
            DataTable items = null;// _billRepo.GetBillItems(billId);

            if (bill.Rows.Count == 0)
            {
                MessageBox.Show("Bill not found.");
                return;
            }

            var billRow = bill.Rows[0];

            // Create document
            _billDocument = new FlowDocument
            {
                PagePadding = new Thickness(50),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13
            };

            // Header
            Paragraph header = new Paragraph(new Run("★ My Company Pvt. Ltd. ★"))
            {
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            _billDocument.Blocks.Add(header);

            _billDocument.Blocks.Add(new Paragraph(new Run("123 Main Street, Chennai 600001"))
            { TextAlignment = TextAlignment.Center });

            _billDocument.Blocks.Add(new Paragraph(new Run("GST No: 29ABCDE1234F1Z5"))
            { TextAlignment = TextAlignment.Center });

            _billDocument.Blocks.Add(new Paragraph(new Run("\nInvoice\n"))
            { FontSize = 18, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center });

            // Bill info
            Table infoTable = new Table();
            infoTable.Columns.Add(new TableColumn { Width = new GridLength(300) });
            infoTable.Columns.Add(new TableColumn { Width = new GridLength(200) });

            TableRowGroup infoRows = new TableRowGroup();
            infoTable.RowGroups.Add(infoRows);

            void AddInfoRow(string label, string value)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(label))) { FontWeight = FontWeights.Bold });
                row.Cells.Add(new TableCell(new Paragraph(new Run(value))));
                infoRows.Rows.Add(row);
            }

            AddInfoRow("Bill No:", billRow["BillID"].ToString());
            AddInfoRow("Date:", billRow["Date"].ToString());
            AddInfoRow("Customer:", billRow["CName"].ToString());

            _billDocument.Blocks.Add(infoTable);
            _billDocument.Blocks.Add(new Paragraph(new Run("\n")));

            // Item table
            Table itemTable = new Table();
            itemTable.CellSpacing = 0;
            itemTable.Columns.Add(new TableColumn { Width = new GridLength(30) });
            itemTable.Columns.Add(new TableColumn { Width = new GridLength(200) });
            itemTable.Columns.Add(new TableColumn { Width = new GridLength(80) });
            itemTable.Columns.Add(new TableColumn { Width = new GridLength(80) });
            itemTable.Columns.Add(new TableColumn { Width = new GridLength(100) });

            TableRowGroup body = new TableRowGroup();
            itemTable.RowGroups.Add(body);

            // Header row
            var headerRow = new TableRow();
            string[] headers = { "No","ProductID", "GOODs", "CQFT","Rate","GST","Amount"};
            foreach (var h in headers)
            {
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run(h)))
                { FontWeight = FontWeights.Bold, BorderBrush = Brushes.Black, BorderThickness = new Thickness(0, 0, 0, 1) });
            }
            body.Rows.Add(headerRow);

            // Items
            int index = 1;
            foreach (DataRow row in items.Rows)
            {
                double qty = 1223;
                double price = 1458;
                double total = 21478;

                TableRow tRow = new TableRow();
                tRow.Cells.Add(new TableCell(new Paragraph(new Run(index.ToString()))));
                tRow.Cells.Add(new TableCell(new Paragraph(new Run(row["ID"].ToString()))));
                tRow.Cells.Add(new TableCell(new Paragraph(new Run(row["Name"].ToString()))));
                tRow.Cells.Add(new TableCell(new Paragraph(new Run(row["CQFT"].ToString()))));
                tRow.Cells.Add(new TableCell(new Paragraph(new Run(qty.ToString("0")))));
                tRow.Cells.Add(new TableCell(new Paragraph(new Run(price.ToString("0.00")))));
                tRow.Cells.Add(new TableCell(new Paragraph(new Run(total.ToString("0.00")))));
                body.Rows.Add(tRow);
                index++;
            }

            _billDocument.Blocks.Add(itemTable);
            _billDocument.Blocks.Add(new Paragraph(new Run("\n")));

            // Total
            Paragraph totalPara = new Paragraph(new Run($"Total Amount: ₹{billRow["Total"]}"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right
            };
            _billDocument.Blocks.Add(totalPara);

            _billDocument.Blocks.Add(new Paragraph(new Run("\nThank you for your business!"))
            { TextAlignment = TextAlignment.Center });

            // Display document
            BillViewer.Document = null; // Clear previous document

            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            // Render the FlowDocument to a visual and add it to the FixedPage
            DocumentPaginator paginator = ((IDocumentPaginatorSource)_billDocument).DocumentPaginator;
            DocumentPage docPage = paginator.GetPage(0);
            if (docPage.Visual is UIElement uiElement)
            {
                fixedPage.Children.Add(uiElement);
            }
            pageContent.Child = fixedPage;
            fixedDoc.Pages.Add(pageContent);

            BillViewer.Document = fixedDoc;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            GenerateBillDocument("J25AA016");


        }
    }
}
