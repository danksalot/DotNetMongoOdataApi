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
using System.Configuration;
using System.Security.Authentication;
using MongoDB.Bson;

namespace AzureMongoApi.Controllers
{
    public class AddressesController : ODataController
    {
        private IMongoCollection<Address> collection;

        public AddressesController()
        {
            string connectionString = @"mongodb://trial1mongo:sllOtIFhiriZcbDYLKxPW5M8ptIErrBrBttczRQ798bgs4x5uz1UUz5fe9EedG0Er3Iujk5PeQNb7fYvmjgezw==@trial1mongo.documents.azure.com:10250/?ssl=true&sslverifycertificate=false";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var mongoDb = mongoClient.GetDatabase("AdventureWorks");
            collection = mongoDb.GetCollection<Address>("Addresses");
        }

        // GET: odata/Addresses
        [EnableQuery]
        public IQueryable<Address> GetAddresses()
        {
            return collection.AsQueryable<Address>();
        }

        // GET: odata/Addresses(5)
        [EnableQuery]
        public SingleResult<Address> GetAddress([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<Address>().Where(entity => entity.AddressID == key));
        }

        // PUT: odata/Addresses(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Address> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<Address>.Filter.Eq(a => a.AddressID, key);
            Address entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<Address>.Filter.Eq(a => a._id, entity._id);
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

        // POST: odata/Addresses
        public async Task<IHttpActionResult> Post(Address entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await collection.InsertOneAsync(entity);

            return Created(entity);
        }

        // PATCH: odata/Addresses(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Address> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<Address>.Filter.Eq(a => a.AddressID, key);
            Address entity = collection.Find(filter).FirstOrDefault();
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
                filter = Builders<Address>.Filter.Eq(a => a._id, entity._id);
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

        // DELETE: odata/Addresses(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var filter = Builders<Address>.Filter.Eq(a => a.AddressID, key);
            Address entity = collection.Find(filter).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            var result = await collection.DeleteOneAsync(filter);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Addresses(5)/CustomerAddresses
        [EnableQuery]
        public IQueryable<CustomerAddress> GetCustomerAddresses([FromODataUri] int key)
        {
            return collection.AsQueryable<Address>().Where(m => m.AddressID == key).SelectMany(m => m.CustomerAddresses);
        }

        // GET: odata/Addresses(5)/SalesOrderHeaders
        [EnableQuery]
        public IQueryable<SalesOrderHeader> GetSalesOrderHeaders([FromODataUri] int key)
        {
            return collection.AsQueryable<Address>().Where(m => m.AddressID == key).SelectMany(m => m.SalesOrderHeaders);
        }

        // GET: odata/Addresses(5)/SalesOrderHeaders1
        [EnableQuery]
        public IQueryable<SalesOrderHeader> GetSalesOrderHeaders1([FromODataUri] int key)
        {
            return collection.AsQueryable<Address>().Where(m => m.AddressID == key).SelectMany(m => m.SalesOrderHeaders1);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool EntityExists(int key)
        {
            return collection.AsQueryable<Address>().Count(e => e.AddressID == key) > 0;
        }
    }
}
