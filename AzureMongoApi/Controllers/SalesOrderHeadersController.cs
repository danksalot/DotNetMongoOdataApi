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
    public class SalesOrderHeadersController : ODataController
    {
        private IMongoCollection<SalesOrderHeader> collection;

        public SalesOrderHeadersController()
        {
            string connectionString = @"mongodb://trial1mongo:sllOtIFhiriZcbDYLKxPW5M8ptIErrBrBttczRQ798bgs4x5uz1UUz5fe9EedG0Er3Iujk5PeQNb7fYvmjgezw==@trial1mongo.documents.azure.com:10250/?ssl=true&sslverifycertificate=false";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var mongoDb = mongoClient.GetDatabase("AdventureWorks");
            collection = mongoDb.GetCollection<SalesOrderHeader>("SalesOrderHeaders");
        }

        // GET: odata/SalesOrderHeaders
        [EnableQuery]
        public IQueryable<SalesOrderHeader> GetSalesOrderHeaders()
        {
            return collection.AsQueryable<SalesOrderHeader>();
        }

        // GET: odata/SalesOrderHeaders(5)
        [EnableQuery]
        public SingleResult<SalesOrderHeader> GetSalesOrderHeader([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<SalesOrderHeader>().Where(entity => entity.SalesOrderID == key));
        }

        // PUT: odata/SalesOrderHeaders(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SalesOrderHeader> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<SalesOrderHeader>.Filter.Eq(a => a.SalesOrderID, key);
            SalesOrderHeader entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<SalesOrderHeader>.Filter.Eq(a => a._id, entity._id);
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

        // POST: odata/SalesOrderHeaders
        public async Task<IHttpActionResult> Post(SalesOrderHeader entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await collection.InsertOneAsync(entity);

            return Created(entity);
        }

        // PATCH: odata/SalesOrderHeaders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SalesOrderHeader> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<SalesOrderHeader>.Filter.Eq(a => a.SalesOrderID, key);
            SalesOrderHeader entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<SalesOrderHeader>.Filter.Eq(a => a._id, entity._id);
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

        // DELETE: odata/SalesOrderHeaders(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var filter = Builders<SalesOrderHeader>.Filter.Eq(a => a.SalesOrderID, key);
            SalesOrderHeader entity = collection.Find(filter).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            var result = await collection.DeleteOneAsync(filter);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SalesOrderHeaders(5)/Address
        [EnableQuery]
        public SingleResult<Address> GetAddress([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<SalesOrderHeader>().Where(m => m.SalesOrderID == key).Select(m => m.Address));
        }

        // GET: odata/SalesOrderHeaders(5)/Address1
        [EnableQuery]
        public SingleResult<Address> GetAddress1([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<SalesOrderHeader>().Where(m => m.SalesOrderID == key).Select(m => m.Address));
        }

        // GET: odata/SalesOrderHeaders(5)/Customer
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<SalesOrderHeader>().Where(m => m.SalesOrderID == key).Select(m => m.Customer));
        }

        // GET: odata/SalesOrderHeaders(5)/SalesOrderDetails
        [EnableQuery]
        public IQueryable<SalesOrderDetail> GetSalesOrderDetails([FromODataUri] int key)
        {
            return collection.AsQueryable<SalesOrderHeader>().Where(m => m.SalesOrderID == key).SelectMany(m => m.SalesOrderDetails);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool EntityExists(int key)
        {
            return collection.AsQueryable<SalesOrderHeader>().Count(e => e.SalesOrderID == key) > 0;
        }
    }
}
