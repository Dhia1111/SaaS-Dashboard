using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IGenralHashService
    {
        string Sha256(string input);

        string Sha256Base64(string input);

        string HmacSha256(string input, string secret);
    }
}
