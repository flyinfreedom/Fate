//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fate.Backoffice.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDetail = new HashSet<OrderDetail>();
        }
    
        public string OrderId { get; set; }
        public System.DateTime Datetime { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string IPAddress { get; set; }
        public string InvoiceHandle { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string VATNumber { get; set; }
        public string CompanyName { get; set; }
        public string Cooperation { get; set; }
        public string CooperationID { get; set; }
        public string PayMethod { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> Refund { get; set; }
        public string PayStatus { get; set; }
        public string InvoiceStatus { get; set; }
        public Nullable<System.DateTime> PayTime { get; set; }
        public bool IsPayed { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string RecipientCode { get; set; }
        public string TxId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
