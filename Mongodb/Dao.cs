using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopCar.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;


namespace ShopCar.Mongodb
{
    public class Dao : IDisposable
    {
        private MongoServer mongoServer = null;
        private bool disposed = false;

        // To do: update the connection string with the DNS name
        // or IP address of your server. 
        //For example, "mongodb://testlinux.cloudapp.net"
        //private string connectionString = "mongodb://<vm-dns-name>";
        //private string connectionString = new MongoConnectionStringBuilder(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
        private string _connectionString = ConfigurationManager.ConnectionStrings["MongoDBConnectionString"].ConnectionString;

        // This sample uses a database named "Tasks" and a 
        //collection named "TasksList".  The database and collection 
        //will be automatically created if they don't already exist.
        private string dbName = "ShopCar";
        private string collectionName = "PreOrder";

        // Default constructor.    
        public Dao()
        {
        }


        // Get DB Collection
        private MongoCollection<PreOrderModel> GetPreOrderCollection()
        {
            MongoServer server = MongoServer.Create(_connectionString);
            MongoDatabase database = server[dbName];
            MongoCollection<PreOrderModel> todoPreOrderCollection = database.GetCollection<PreOrderModel>(collectionName);
            return todoPreOrderCollection;
        }


        private MongoCollection<PreOrderModel> GetPreOrderCollectionForEdit()
        {
            MongoServer server = MongoServer.Create(_connectionString);
            MongoDatabase database = server[dbName];
            MongoCollection<PreOrderModel> todoPreOrderCollection = database.GetCollection<PreOrderModel>(collectionName);
            return todoPreOrderCollection;
        }


        // Gets all PreOrder items from the MongoDB server.        
        public List<PreOrderModel> GetAllPreOder()
        {
            try
            {
                MongoCollection<PreOrderModel> collection = GetPreOrderCollection();
                return collection.FindAll().ToList<PreOrderModel>();
            }
            catch (MongoConnectionException)
            {
                return new List<PreOrderModel>();
            }
        }


        // Creates a Task and inserts it into the collection in MongoDB.
        public void CreatePreOrder(PreOrderModel preOrder)
        {
            MongoCollection<PreOrderModel> collection = GetPreOrderCollectionForEdit();
            try
            {
                collection.Insert(preOrder, SafeMode.True);
            }
            catch (MongoCommandException ex)
            {
                string msg = ex.Message;
            }
        }



        # region IDisposable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (mongoServer != null)
                    {
                        this.mongoServer.Disconnect();
                    }
                }
            }

            this.disposed = true;
        }

        # endregion

    }
}