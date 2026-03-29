using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IEmailTemplateHandler
    {
        Task<string> CreateTemplate(string code);
    }

}
