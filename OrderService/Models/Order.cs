﻿using Gateway.Models;

namespace OrderService.Models
{
    public class Order
    {
        public Order(StockTransferOrderModel orderModel)
        {
            this.UserId = orderModel.UserId;
            this.Status = orderModel.Status;
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
