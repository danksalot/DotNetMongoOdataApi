namespace AzureMongoApi.Models
{
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesLT.ProductModelProductDescription")]
    public partial class ProductModelProductDescription
    {
        public ObjectId _id { get; set; }

        public int ProductModelID { get; set; }

        public int ProductDescriptionID { get; set; }

        public string Culture { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ProductDescription ProductDescription { get; set; }

        public virtual ProductModel ProductModel { get; set; }
    }
}
