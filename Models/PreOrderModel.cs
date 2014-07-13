using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace ShopCar.Models
{
    public class PreOrderModel
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("app_ser")]
        public int app_ser { get; set; }

        [BsonElement("user_app_ser")]
        public int user_app_ser { get; set; }

        [BsonElement("user_id")]
        public string user_id { get; set; }

        [BsonElement("user_name")]
        public string user_name { get; set; }

        [BsonElement("pro_class_name")]
        public string pro_class_name { get; set; }

        [BsonElement("prod_desc")]
        public string prod_desc { get; set; }

        [BsonElement("prod_feature")]
        public string prod_feature { get; set; }

        [BsonElement("prod_name")]
        public string prod_name { get; set; }

        [BsonElement("prod_no")]
        public string prod_no { get; set; }

        [BsonElement("prod_price")]
        public int prod_price { get; set; }

        [BsonElement("prod_special_price")]
        public int prod_special_price { get; set; }

        [BsonElement("create_date")]
        public DateTime create_date { get; set; }

        [BsonElement("update_date")]
        public DateTime update_date { get; set; }

    }

}