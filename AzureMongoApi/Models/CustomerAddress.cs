namespace AzureMongoApi.Models
{
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesLT.CustomerAddress")]
    public partial class CustomerAddress
    {
        public ObjectId _id { get; set; }

        public int CustomerID { get; set; }

        public int AddressID { get; set; }

        public string AddressType { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Address Address { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
