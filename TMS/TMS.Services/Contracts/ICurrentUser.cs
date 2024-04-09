using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Services.Contracts
{
    public interface ICurrentUser
    {
        public Guid? Id { get; }
    }
}
