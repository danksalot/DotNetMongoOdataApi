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
    public class CustomerAddressesController : ODataController
    {
        private IMongoCollection<CustomerAddress> collection;

        public CustomerAddressesController()
        {
            string connectionString = @"mongodb://trial1mongo:sllOtIFhiriZcbDYLKxPW5M8ptIErrBrBttczRQ798bgs4x5uz1UUz5fe9EedG0Er3Iujk5PeQNb7fYvmjgezw==@trial1mongo.documents.azure.com:10250/?ssl=true&sslverifycertificate=false";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var mongoDb = mongoClient.GetDatabase("AdventureWorks");
            this.collection = mongoDb.GetCollection<CustomerAddress>("CustomerAddresses");
        }

        // GET: odata/CustomerAddresses
        [EnableQuery]
        public IQueryable<CustomerAddress> GetCustomerAddresses()
        {
            return collection.AsQueryable<CustomerAddress>();
        }

        // GET: odata/CustomerAddresses(5)
        [EnableQuery]
        public SingleResult<CustomerAddress> GetCustomerAddress([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<CustomerAddress>().Where(address => address.AddressID == key));
        }

        // PUT: odata/CustomerAddresses(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<CustomerAddress> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<CustomerAddress>.Filter.Eq(a => a.AddressID, key);
            CustomerAddress entity = collection.Find(filter).FirstOrDefault();
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
                await collection.ReplaceOneAsync(filter, entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerAddressExists(key))
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

        // POST: odata/CustomerAddresses
        public async Task<IHttpActionResult> Post(CustomerAddress entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            await collection.InsertOneAsync(entity);

            return Created(entity);
        }

        // PATCH: odata/CustomerAddresses(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CustomerAddress> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filter = Builders<CustomerAddress>.Filter.Eq(a => a.AddressID, key);
            CustomerAddress entity = collection.Find(filter).FirstOrDefault();
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
                await collection.ReplaceOneAsync(filter, entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerAddressExists(key))
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

        // DELETE: odata/CustomerAddresses(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var filter = Builders<CustomerAddress>.Filter.Eq(a => a.AddressID, key);
            CustomerAddress entity = collection.Find(filter).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            var result = await collection.DeleteOneAsync(filter);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/CustomerAddresses(5)/Address
        [EnableQuery]
        public SingleResult<Address> GetAddress([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<CustomerAddress>().Where(m => m.CustomerID == key).Select(m => m.Address));
        }

        // GET: odata/CustomerAddresses(5)/Customer
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(collection.AsQueryable<CustomerAddress>().Where(m => m.CustomerID == key).Select(m => m.Customer));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool CustomerAddressExists(int key)
        {
            return collection.AsQueryable<CustomerAddress>().Count(e => e.CustomerID == key) > 0;
        }
    }
}
