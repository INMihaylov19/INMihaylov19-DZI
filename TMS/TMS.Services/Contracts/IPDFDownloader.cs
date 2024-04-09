using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Services.Contracts
{
    public interface IPDFDownloader
    {
        Task<byte[]> DownloadPDF(string html);
    }
}
