using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.DaoInterface
{
    public interface IPostalCode
    {
        public bool ValidatePostalCode(string postalCode, string city, string state, out string error);
        public void Test();
    }
}
