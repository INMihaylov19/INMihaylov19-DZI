using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Services.Contracts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web;
using iTextSharp.text.html.simpleparser;

namespace TMS.Services.Implementations
{
    public class PDFDownloader : IPDFDownloader
    {
        [Obsolete]
        public async Task<byte[]> DownloadPDF(string html)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                HTMLWorker htmlWorker = new HTMLWorker(document);
                using (StringReader sr = new StringReader(html))
                {
                    htmlWorker.Parse(sr);
                }
                document.Close();
                bytes = ms.ToArray();
            }
            return bytes;
        }
    }
}
