using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Entites.Consts
{
    public class MyStatus
    {
        public const string StatusPending = "Pending";//قيد الانتظار
        public const string StatusApproved = "Approved";//موافقه
        public const string StatusInProcess = "Processing";//تعالج
        public const string StatusShipped = "Shipped";//تم شحنها
        public const string StatusCancelled = "Cancelled";//تم الالغاء
        public const string StatusRefunded = "Refunded";//تم ردها

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";
    }
}
