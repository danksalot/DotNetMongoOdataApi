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
    public class SalesOrderDetailsController : ODataController
    {
        private IMongoCollection<SalesOrderDetail> collection;

        public SalesOrderDetailsController()
        {
            string connectionString = @"mongodb://trial1mongo:sllOtIFhiriZcbDYLKxPW5M8ptIErrBrBttczRQ798bgs4x5uz1UUz5fe9EedG0Er3Iujk5PeQNb7fYvmjgezw==@trial1mongo.documents.azure.com:10250/?ssl=true&sslverifycertificate=false";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var mongoDb = mongoClient.GetDatabase("AdventureWorks");
            collection = mongoDb.GetCollection<SalesOrderDetail>("SalesOrderDetails");
        }

        // GET: odata/SalesOrderDetails
        [EnableQuery]
        public IQueryable<SalesOrderDetail> GetSalesOrderDetails()
        {
            return collection.AsQueryable<SalesOrderDetail>();
        }

        // GET: odata/SalesOrderDetails(5)
        [EnableQuery]
        public SingleResult<SalesOrderDetail> GetSalesOrderDetail([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<SalesOrderDetail>().Where(entity => entity.SalesOrderDetailID == key));
        }

        // PUT: odata/SalesOrderDetails(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SalesOrderDetail> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<SalesOrderDetail>.Filter.Eq(a => a.SalesOrderDetailID, key);
            SalesOrderDetail entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<SalesOrderDetail>.Filter.Eq(a => a._id, entity._id);
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

        // POST: odata/SalesOrderDetails
        public async Task<IHttpActionResult> Post(SalesOrderDetail entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await collection.InsertOneAsync(entity);

            return Created(entity);
        }

        // PATCH: odata/SalesOrderDetails(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SalesOrderDetail> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<SalesOrderDetail>.Filter.Eq(a => a.SalesOrderDetailID, key);
            SalesOrderDetail entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<SalesOrderDetail>.Filter.Eq(a => a._id, entity._id);
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

        // DELETE: odata/SalesOrderDetails(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var filter = Builders<SalesOrderDetail>.Filter.Eq(a => a.SalesOrderDetailID, key);
            SalesOrderDetail entity = collection.Find(filter).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            var result = await collection.DeleteOneAsync(filter);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SalesOrderDetails(5)/Product
        [EnableQuery]
        public SingleResult<Product> GetProduct([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<SalesOrderDetail>().Where(m => m.SalesOrderID == key).Select(m => m.Product));
        }

        // GET: odata/SalesOrderDetails(5)/SalesOrderHeader
        [EnableQuery]
        public SingleResult<SalesOrderHeader> GetSalesOrderHeader([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<SalesOrderDetail>().Where(m => m.SalesOrderID == key).Select(m => m.SalesOrderHeader));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool EntityExists(int key)
        {
            return collection.AsQueryable<SalesOrderDetail>().Count(e => e.SalesOrderDetailID == key) > 0;
        }
    }
}
