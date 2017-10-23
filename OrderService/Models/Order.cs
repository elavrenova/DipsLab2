using DipsLab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Order
    {
        public Order(OrderModel orderModel)
        {
            this.UserId = orderModel.UserId;
            this.Email = orderModel.Email;
        }

        public long Id { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
    }
}
