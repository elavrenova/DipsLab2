using DipsLab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Order
    {
        public Order(StockTransferOrderModel orderModel)
        {
            this.UserId = orderModel.UserId;
            this.Status = orderModel.OrderStatus;
            this.StockId = orderModel.StockId;
            this.TransferId = orderModel.TransferId;
            this.Value = orderModel.Value;
        }

        public Order()
        {
            
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }
        public double Value { get; set; }
        public int Status { get; set; }
        public int TransferId { get; set; }
    }
}
