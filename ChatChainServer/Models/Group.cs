using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainServer.Models
{
    /**
     * ---- IMPORTANT ----
     *
     * ALL CHANGES TO THIS FILE MUST BE RECIPROCATED IN THE IdentityServer_WebApp PROJECT
     *
     * ---- IMPORTANT ----
     */
    public class Group
    {
        public ObjectId Id { get; set; }

        [BsonElement("GroupId")]
        public string GroupId { get; set; }
        
        [BsonElement("GroupName")]
        public string GroupName { get; set; }
        
        [BsonElement("GroupDescription")]
        public string GroupDescription { get; set; }
        
        [BsonElement("OwnerId")]
        public string OwnerId { get; set; }
        
        [BsonElement("ClientIds")]
        public List<ObjectId> ClientIds { get; set; } = new List<ObjectId>();
    }
}