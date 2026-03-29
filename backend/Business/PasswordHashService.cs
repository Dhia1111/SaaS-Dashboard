using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IPasswordHashService
    {

        public string Hash(string password);
        public bool Verify(string hashedPassword, string providedPassword);
    }


}
