namespace AzureMongoApi.Models
{
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesLT.SalesOrderDetail")]
    public partial class SalesOrderDetail
    {
        public ObjectId _id { get; set; }

        public int SalesOrderID { get; set; }

        public int SalesOrderDetailID { get; set; }

        public short OrderQty { get; set; }

        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal UnitPriceDiscount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal LineTotal { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual SalesOrderHeader SalesOrderHeader { get; set; }
    }
}
