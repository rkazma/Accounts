using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.DTOModels
{
    public class CustomerInfoDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public decimal Balance { get; set; }
    }
}
