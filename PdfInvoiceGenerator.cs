using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS
{
    public static class PdfInvoiceGenerator
    {
        public static void GeneratePdf(string outputPath, BillModel bill)
        {
            var doc = new Document();
            doc.Info.Title = $"Invoice {bill.BillID}";
            doc.Info.Author = "Your Company";

            var style = doc.Styles["Normal"];

            style.Font.Size = 10;

            Section sec = doc.AddSection();
            sec.PageSetup.PageFormat = PageFormat.A4;

            var header = sec.AddParagraph("My Company Pvt. Ltd.");
            header.Format.Alignment = ParagraphAlignment.Center;
            header.Format.Font.Size = 16;
            header.Format.Font.Bold = true;

            sec.AddParagraph($"Bill No: {bill.BillID}");
            sec.AddParagraph($"Date: {bill.Date}");
            sec.AddParagraph($"Customer: {bill.CName}");
            sec.AddParagraph($"Phone: {bill.CPhone}");
            sec.AddParagraph();

            // Table for items
            var table = sec.AddTable();
            table.Borders.Width = 0.75;
            table.AddColumn(Unit.FromCentimeter(1));
            table.AddColumn(Unit.FromCentimeter(6));
            table.AddColumn(Unit.FromCentimeter(2));
            table.AddColumn(Unit.FromCentimeter(2));
            table.AddColumn(Unit.FromCentimeter(2));
            table.AddColumn(Unit.FromCentimeter(3));

            var hdr = table.AddRow();
            hdr.Cells[0].AddParagraph("No");
            hdr.Cells[1].AddParagraph("Item");
            hdr.Cells[2].AddParagraph("Qty");
            hdr.Cells[3].AddParagraph("Rate");
            hdr.Cells[4].AddParagraph("GST");
            hdr.Cells[5].AddParagraph("Amount");

            int i = 1;
            foreach (var it in bill.Items)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(i.ToString());
                row.Cells[1].AddParagraph(it.Name);
                row.Cells[2].AddParagraph(it.CQFT.ToString("0.##"));
                row.Cells[3].AddParagraph(it.Rate.ToString("0.00"));
                row.Cells[4].AddParagraph(it.GST.ToString("0.##"));
                row.Cells[5].AddParagraph(it.Amount.ToString("0.00"));
                i++;
            }

            var totalPar = sec.AddParagraph();
            totalPar.Format.Alignment = ParagraphAlignment.Right;
            totalPar.AddText($"Total: ₹{bill.Total:0.00}");

            PdfSharp.Fonts.GlobalFontSettings.FontResolver = new BS.SimpleFontResolver();

            var renderer = new PdfDocumentRenderer() { 
                Document = doc
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(outputPath);

        }

    }
}
