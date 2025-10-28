using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace BS
{
    public class BillRepository
    {
        static string dbPath = "Data\\store.db";
        private readonly string _connectionString;

        public BillRepository(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
        }
        public BillRepository()
        {
            _connectionString = $"Data Source={dbPath}";
        }

        public BillModel GetBillById(string billId)
        {
            BillModel bill = null;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Bill WHERE BillID = @BillID LIMIT 1;";
                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@BillID", billId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bill = new BillModel
                            {
                                BillID = reader["BillID"].ToString(),
                                Date = reader["Date"].ToString(),
                                CName = reader["CName"].ToString(),
                                CCompanyName = reader["CCompanyName"]?.ToString(),
                                CPhone = reader["CPhone"].ToString(),
                                CAddress = reader["CAddress"].ToString(),
                                CEmail = reader["CEmail"]?.ToString(),
                                Total = Convert.ToDecimal(reader["Total"]),
                            };

                            // Parse the orderItems column
                            string itemsRaw = reader["orderItems"]?.ToString();
                            if (!string.IsNullOrEmpty(itemsRaw))
                            {
                                bill.Items = ParseOrderItems(itemsRaw);
                            }
                        }
                    }
                }
            }

            return bill;
        }

        private List<BillItem> ParseOrderItems(string itemsRaw)
        {
            var list = new List<BillItem>();

            // orderItems format: "ID,Name,Rate,CQFT,GST,Amount||ID,Name,Rate,CQFT,GST,Amount"
            string[] itemParts = itemsRaw.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in itemParts)
            {
                string[] fields = item.Split(',');

                if (fields.Length >= 6)
                {
                    list.Add(new BillItem
                    {
                        ID = int.TryParse(fields[0], out int id) ? id : 0,
                        Name = fields[1],
                        Rate = decimal.TryParse(fields[2], out decimal rate) ? rate : 0,
                        CQFT = decimal.TryParse(fields[3], out decimal qty) ? qty : 0,
                        GST = decimal.TryParse(fields[4], out decimal gst) ? gst : 0,
                        Amount = decimal.TryParse(fields[5], out decimal amt) ? amt : 0
                    });
                }
            }

            return list;
        }
     
      public void PRINT
                (string shop,string invo, string gstno, string name,string cgstno,string address,string phn,DataTable order,string total)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {

                    FlowDocument doc = CreateInvoiceFlowDocument(shop,invo,cgstno,gstno,name,address,phn,order,total);

                    // Set page properties for proper printing
                    doc.PageHeight = printDialog.PrintableAreaHeight;
                    doc.PageWidth = printDialog.PrintableAreaWidth;
                    doc.PagePadding = new Thickness(50);
                    doc.ColumnGap = 0;
                    doc.ColumnWidth = printDialog.PrintableAreaWidth - 100;

                    IDocumentPaginatorSource idpSource = doc;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Invoice");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print Error: {ex.Message}");
            }
        }
        public FlowDocument CreateInvoiceFlowDocument(string shop, string invo,string cgstno,string gstno, string name, string address, string phn, DataTable order, string total)
        {
            FlowDocument doc = new FlowDocument
            {


                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 12,
                Foreground = Brushes.Black
            };


            try
            {// Create a horizontal container(StackPanel)
                StackPanel logoPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 0)
                };

                // --- Logo 1 ---
                Image logo1 = new Image
                {
                    Width = 580,
                    Height = 55,
                    Margin = new Thickness(0, 0, 20, 0),
                    Stretch = Stretch.Uniform
                };
                BitmapImage bitmap1 = new BitmapImage();
                bitmap1.BeginInit();
                bitmap1.UriSource = new Uri(@""+Environment.CurrentDirectory+"\\213.png", UriKind.Absolute); // Change path as needed
                bitmap1.EndInit();
                logo1.Source = bitmap1;

              

                // Add both to the panel
                logoPanel.Children.Add(logo1);
  

                // Wrap panel inside a BlockUIContainer for the FlowDocument
                BlockUIContainer logoContainer = new BlockUIContainer(logoPanel);
                doc.Blocks.Add(logoContainer);
            }
            catch (Exception ex)
            {
                // If logos fail to load, show text placeholders
                Paragraph logoPlaceholder = new Paragraph
                {
                    TextAlignment = TextAlignment.Center,
                    FontSize = 12,
                    Margin = new Thickness(0, 0, 20,10)
                };
                logoPlaceholder.Inlines.Add(new Run("[LOGO 1]"));
                doc.Blocks.Add(logoPlaceholder);
            }

             // Invoice Title
            Paragraph title = new Paragraph();
            title.TextAlignment = TextAlignment.Center;
            title.FontSize = 16;
            
            title.Margin = new Thickness(0, 0, 0, 10);
            title.Inlines.Add(new Run(shop));
            doc.Blocks.Add(title);

            // Invoice Details
            Paragraph details = new Paragraph();
             details.Inlines.Add(new LineBreak());
            details.Inlines.Add(new Run($"Date          : {DateTime.Now.ToString("dd MMMM yyyy")}"));
            details.Inlines.Add(new LineBreak());
            details.Inlines.Add(new Run($"GST No        : {gstno}"));
            details.Inlines.Add(new LineBreak());
            details.Inlines.Add(new Run($"Invoice No    : {invo}"));
            details.Inlines.Add(new LineBreak());

            details.Margin = new Thickness(0, 0, 0,0);
            details.BorderBrush = Brushes.Black;
            details.BorderThickness = new Thickness(0,1,0,1);
            doc.Blocks.Add(details);

            Paragraph details1 = new Paragraph();
            details1.Inlines.Add(new Run("Customer Info.............."));
            details1.Inlines.Add(new LineBreak());
            details1.Inlines.Add(new LineBreak());

            details1.Inlines.Add(new Run($"Customer's GST No.:{cgstno} "));
            details1.Inlines.Add(new LineBreak());

            details1.Inlines.Add(new Run($"Name         :{name} "));
            details1.Inlines.Add(new LineBreak());


            details1.Inlines.Add(new Run($"Phone         :{phn}"));
            details1.Inlines.Add(new LineBreak());
            details1.Inlines.Add(new Run($"Address       :{address}"));
            details1.Inlines.Add(new LineBreak());

            
            details1.Margin = new Thickness(0, 0, 0, 0);
            details1.BorderBrush = Brushes.Black;
            details1.BorderThickness = new Thickness(0,0,0,1);
            doc.Blocks.Add(details1);

            // Items Table
            Table table = new Table();
            table.CellSpacing = 0;
            table.BorderBrush = Brushes.Black;
            table.BorderThickness = new Thickness(1);

            // Define columns
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.3, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) });

            table.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.6, GridUnitType.Star) });


            // Header row
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            headerRow.Background = Brushes.LightGray;
            headerRow.Cells.Add(CreateTableCell("S.No", true));
            headerRow.Cells.Add(CreateTableCell("Good's id", true));
            headerRow.Cells.Add(CreateTableCell("Item", true));
            headerRow.Cells.Add(CreateTableCell("CQFT", true));
            headerRow.Cells.Add(CreateTableCell("Rate", true));
            headerRow.Cells.Add(CreateTableCell("CGST(%)", true));
            headerRow.Cells.Add(CreateTableCell("SGST(%)", true));
            headerRow.Cells.Add(CreateTableCell("IGST(%)", true));
            headerRow.Cells.Add(CreateTableCell("Total", true));
            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);



            // Data rows
            TableRowGroup dataGroup = new TableRowGroup();
            int ind = 1,c=0;
            foreach (DataRow dr in order.Rows)
            {
                float sg=float.Parse(dr["GST"].ToString()) , cg=float.Parse(dr["GST"].ToString()) ;
                
                dataGroup.Rows.Add(CreateDataRow(
                    (ind++).ToString(),
                    dr["ID"].ToString(),
                    dr["Name"].ToString(),
                    dr["CQFT"].ToString(),
                    dr["Rate"].ToString(),
                     (sg/2).ToString(),
                      (cg/2).ToString(),
                    dr["GST"].ToString(),
                    dr["Amount"].ToString()
                ));
                c++;
              
            }

            for(int i=c;i<15;i++)
            {
                dataGroup.Rows.Add(CreateDataRow(
                   "",
                    "",
                    "",
                    "",
                    "",
                    "","",
                    "",
                    ""
                ));
            }
          

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            // Total
            Paragraph Total = new Paragraph();
            Total.TextAlignment = TextAlignment.Right;
            Total.FontSize = 14;
            Total.FontWeight = FontWeights.Bold;
            Total.Margin = new Thickness(0, 20, 0, 20);
            Total.Inlines.Add(new Run($"Grand Total: {total}"));
            doc.Blocks.Add(Total);

            // Invoice Details
            Paragraph dis = new Paragraph();
            dis.Inlines.Add(new LineBreak());
            dis.Inlines.Add(new Run("Declaration:\n We declare that this invoice shows the actual price of the goods described and that all particulars are true and correct."));
           
            dis.Margin = new Thickness(0, 0, 0, 0);
            dis.BorderBrush = Brushes.Black;
            dis.BorderThickness = new Thickness(0, 1, 0, 1);
            doc.Blocks.Add(dis);

            return doc;
        }

        public TableCell CreateTableCell(string text, bool isHeader = false)
        {

            TableCell cell = new TableCell();
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run(text));

            if (isHeader)
            {
                para.FontWeight = FontWeights.Bold;

                cell.BorderThickness = new Thickness(0, 0, 0, 1);
                cell.BorderBrush = Brushes.Black;
            }

            para.Margin = new Thickness(5);
            cell.Blocks.Add(para);
            cell.BorderBrush = Brushes.Black;
            cell.BorderThickness = new Thickness(0.5, 0, 0.5, 0);

            return cell;
        }

        public TableRow CreateDataRow(string sno, string ID, string item, string cqft, string rate,string cg,string sg ,string GST, string total)
        {
            TableRow row = new TableRow();
            row.Cells.Add(CreateTableCell(sno));
            row.Cells.Add(CreateTableCell(ID));
            row.Cells.Add(CreateTableCell(item));
            row.Cells.Add(CreateTableCell(cqft));
            row.Cells.Add(CreateTableCell(rate));
            row.Cells.Add(CreateTableCell(cg));
            row.Cells.Add(CreateTableCell(sg));
            row.Cells.Add(CreateTableCell(GST));
            row.Cells.Add(CreateTableCell(total));
            return row;
        }
    }
}
public class Item
{
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Total { get; set; }
}

