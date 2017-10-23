using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DipsLab2.Models
{
    public class OrderModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
    }
}
