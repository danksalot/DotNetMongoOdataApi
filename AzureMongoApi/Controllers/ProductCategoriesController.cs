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
    public class ProductCategoriesController : ODataController
    {
        private IMongoCollection<ProductCategory> collection;

        public ProductCategoriesController()
        {
            string connectionString = @"mongodb://trial1mongo:sllOtIFhiriZcbDYLKxPW5M8ptIErrBrBttczRQ798bgs4x5uz1UUz5fe9EedG0Er3Iujk5PeQNb7fYvmjgezw==@trial1mongo.documents.azure.com:10250/?ssl=true&sslverifycertificate=false";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var mongoDb = mongoClient.GetDatabase("AdventureWorks");
            collection = mongoDb.GetCollection<ProductCategory>("ProductCategories");
        }

        // GET: odata/ProductCategories
        [EnableQuery]
        public IQueryable<ProductCategory> GetProductCategories()
        {
            return collection.AsQueryable<ProductCategory>();
        }

        // GET: odata/ProductCategories(5)
        [EnableQuery]
        public SingleResult<ProductCategory> GetProductCategory([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<ProductCategory>().Where(entity => entity.ProductCategoryID == key));
        }

        // PUT: odata/ProductCategories(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ProductCategory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<ProductCategory>.Filter.Eq(a => a.ProductCategoryID, key);
            ProductCategory entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<ProductCategory>.Filter.Eq(a => a._id, entity._id);
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

        // POST: odata/ProductCategories
        public async Task<IHttpActionResult> Post(ProductCategory entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await collection.InsertOneAsync(entity);

            return Created(entity);
        }

        // PATCH: odata/ProductCategories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ProductCategory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<ProductCategory>.Filter.Eq(a => a.ProductCategoryID, key);
            ProductCategory entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<ProductCategory>.Filter.Eq(a => a._id, entity._id);
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

        // DELETE: odata/ProductCategories(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var filter = Builders<ProductCategory>.Filter.Eq(a => a.ProductCategoryID, key);
            ProductCategory entity = collection.Find(filter).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            var result = await collection.DeleteOneAsync(filter);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ProductCategories(5)/ProductCategory1
        [EnableQuery]
        public IQueryable<ProductCategory> GetProductCategory1([FromODataUri] int key)
        {
            return collection.AsQueryable<ProductCategory>().Where(m => m.ProductCategoryID == key).SelectMany(m => m.ProductCategory1);
        }

        // GET: odata/ProductCategories(5)/ProductCategory2
        [EnableQuery]
        public SingleResult<ProductCategory> GetProductCategory2([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<ProductCategory>().Where(m => m.ProductCategoryID == key).Select(m => m.ProductCategory2));
        }

        // GET: odata/ProductCategories(5)/Products
        [EnableQuery]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return collection.AsQueryable<ProductCategory>().Where(m => m.ProductCategoryID == key).SelectMany(m => m.Products);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool EntityExists(int key)
        {
            return collection.AsQueryable<ProductCategory>().Count(e => e.ProductCategoryID == key) > 0;
        }
    }
}
