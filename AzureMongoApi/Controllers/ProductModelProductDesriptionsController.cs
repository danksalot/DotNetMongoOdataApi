using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using AzureMongoApi.Models;
using MongoDB.Driver;
using System.Security.Authentication;
using MongoDB.Bson;

namespace AzureMongoApi.Controllers
{
    public class ProductModelProductDescriptionsController : ODataController
    {
        private IMongoCollection<ProductModelProductDescription> collection;

        public ProductModelProductDescriptionsController()
        {
            string connectionString = @"mongodb://trial1mongo:sllOtIFhiriZcbDYLKxPW5M8ptIErrBrBttczRQ798bgs4x5uz1UUz5fe9EedG0Er3Iujk5PeQNb7fYvmjgezw==@trial1mongo.documents.azure.com:10250/?ssl=true&sslverifycertificate=false";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var mongoDb = mongoClient.GetDatabase("AdventureWorks");
            collection = mongoDb.GetCollection<ProductModelProductDescription>("ProductModelProductDescriptions");
        }

        // GET: odata/ProductModelProductDescriptions
        [EnableQuery]
        public IQueryable<ProductModelProductDescription> GetProductModelProductDescriptions()
        {
            return collection.AsQueryable<ProductModelProductDescription>();
        }

        // GET: odata/ProductModelProductDescriptions(5)
        [EnableQuery]
        public SingleResult<ProductModelProductDescription> GetProductModelProductDescription([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<ProductModelProductDescription>().Where(productModelProductDescription => productModelProductDescription.ProductModelID == key));
        }

        // PUT: odata/ProductModelProductDescriptions(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ProductModelProductDescription> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<ProductModelProductDescription>.Filter.Eq(a => a.ProductModelID, key);
            ProductModelProductDescription entity = collection.Find(filter).FirstOrDefault();
            ObjectId objectId;
            if (entity == null)
            {
                return NotFound();
            }
            else
            {
                objectId = entity._id;
            }

            patch.Put(entity);
            entity._id = objectId;

            try
            {
                filter = Builders<ProductModelProductDescription>.Filter.Eq(a => a._id, entity._id);
                await collection.ReplaceOneAsync(filter, entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        // POST: odata/ProductModelProductDescriptions
        public async Task<IHttpActionResult> Post(ProductModelProductDescription entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await collection.InsertOneAsync(entity);

            return Created(entity);
        }

        // PATCH: odata/ProductModelProductDescriptions(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ProductModelProductDescription> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<ProductModelProductDescription>.Filter.Eq(a => a.ProductModelID, key);
            ProductModelProductDescription entity = collection.Find(filter).FirstOrDefault();
            ObjectId objectId;
            if (entity == null)
            {
                return NotFound();
            }
            else
            {
                objectId = entity._id;
            }

            patch.Patch(entity);
            entity._id = objectId;

            try
            {
                filter = Builders<ProductModelProductDescription>.Filter.Eq(a => a._id, entity._id);
                await collection.ReplaceOneAsync(filter, entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        // DELETE: odata/ProductModelProductDescriptions(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var filter = Builders<ProductModelProductDescription>.Filter.Eq(a => a.ProductModelID, key);
            ProductModelProductDescription entity = collection.Find(filter).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            var result = await collection.DeleteOneAsync(filter);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ProductModelProductDescriptions(5)/ProductDescription
        [EnableQuery]
        public SingleResult<ProductDescription> GetProductDescription([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<ProductModelProductDescription>().Where(m => m.ProductModelID == key).Select(m => m.ProductDescription));
        }

        // GET: odata/ProductModelProductDescriptions(5)/ProductModel
        [EnableQuery]
        public SingleResult<ProductModel> GetProductModel([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<ProductModelProductDescription>().Where(m => m.ProductModelID == key).Select(m => m.ProductModel));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool EntityExists(int key)
        {
            return collection.AsQueryable<ProductModelProductDescription>().Count(e => e.ProductModelID == key) > 0;
        }
    }
}
