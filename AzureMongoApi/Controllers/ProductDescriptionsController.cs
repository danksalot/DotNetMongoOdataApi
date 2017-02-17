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
    public class ProductDescriptionsController : ODataController
    {
        private IMongoCollection<ProductDescription> collection;

        public ProductDescriptionsController()
        {
            string connectionString = @"mongodb://trial1mongo:sllOtIFhiriZcbDYLKxPW5M8ptIErrBrBttczRQ798bgs4x5uz1UUz5fe9EedG0Er3Iujk5PeQNb7fYvmjgezw==@trial1mongo.documents.azure.com:10250/?ssl=true&sslverifycertificate=false";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var mongoDb = mongoClient.GetDatabase("AdventureWorks");
            collection = mongoDb.GetCollection<ProductDescription>("ProductDescriptions");
        }

        // GET: odata/ProductDescriptions
        [EnableQuery]
        public IQueryable<ProductDescription> GetProductDescriptions()
        {
            return collection.AsQueryable<ProductDescription>();
        }

        // GET: odata/ProductDescriptions(5)
        [EnableQuery]
        public SingleResult<ProductDescription> GetProductDescription([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<ProductDescription>().Where(entity => entity.ProductDescriptionID == key));
        }

        // PUT: odata/ProductDescriptions(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ProductDescription> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<ProductDescription>.Filter.Eq(a => a.ProductDescriptionID, key);
            ProductDescription entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<ProductDescription>.Filter.Eq(a => a._id, entity._id);
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

        // POST: odata/ProductDescriptions
        public async Task<IHttpActionResult> Post(ProductDescription entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await collection.InsertOneAsync(entity);

            return Created(entity);
        }

        // PATCH: odata/ProductDescriptions(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ProductDescription> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<ProductDescription>.Filter.Eq(a => a.ProductDescriptionID, key);
            ProductDescription entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<ProductDescription>.Filter.Eq(a => a._id, entity._id);
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

        // DELETE: odata/ProductDescriptions(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var filter = Builders<ProductDescription>.Filter.Eq(a => a.ProductDescriptionID, key);
            ProductDescription entity = collection.Find(filter).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            var result = await collection.DeleteOneAsync(filter);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ProductDescriptions(5)/ProductModelProductDescriptions
        [EnableQuery]
        public IQueryable<ProductModelProductDescription> GetProductModelProductDescriptions([FromODataUri] int key)
        {
            return collection.AsQueryable<ProductDescription>().Where(m => m.ProductDescriptionID == key).SelectMany(m => m.ProductModelProductDescriptions);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool EntityExists(int key)
        {
            return collection.AsQueryable<ProductDescription>().Count(e => e.ProductDescriptionID == key) > 0;
        }
    }
}
