using System;
using System.Collections.Generic;
using IdentityServer4.Extensions;
using WebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApp.Services
{
    public class ClientService
    {
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<ClientConfig> _clientConfigs;
        private readonly IMongoCollection<Group> _groups;
        private readonly IServiceProvider _services;

        public ClientService(IConfiguration config, IServiceProvider services)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("CLIENTS_AND_GROUPS_DATABASE");

            MongoClient client;
            
            if (databaseUrl != null && !databaseUrl.IsNullOrEmpty())
            {
                client = new MongoClient(databaseUrl);
            }
            else
            {
                client = new MongoClient(config.GetConnectionString("MongoDB"));
            }
            
            var database = client.GetDatabase("ChatChainGroups");
            _clients = database.GetCollection<Client>("Clients");
            _groups = database.GetCollection<Group>("Groups");
            _clientConfigs = database.GetCollection<ClientConfig>("ClientConfigs");

            _services = services;
        }

        public List<Client> Get()
        {
            return _clients.Find(client => true).ToList();
        }
        
        public List<Client> GetFromOwnerId(string id)
        {
            return _clients.Find(client => client.OwnerId == id).ToList();
        }
        
        public Client Get(ObjectId id)
        {
            return _clients.Find(client => client.Id == id).FirstOrDefault();
        }
        
        public Client Get(string clientId)
        {
            return _clients.Find(client => client.ClientId == clientId).FirstOrDefault();
        }

        public void Create(Client client)
        {
            _clients.InsertOne(client);
        }

        public void Update(ObjectId id, Client clientIn)
        {
            _clients.ReplaceOne(client => client.Id == id, clientIn);
        }

        public void Remove(Client clientIn)
        {
            _clients.DeleteOne(client => client.Id == clientIn.Id);
        }

        public ClientConfig GetClientConfig(ObjectId id)
        {
            var client = Get(id);
            return _services.GetRequiredService<ClientConfigService>().Get(client.ClientConfigId);
        }
        
        public List<Group> GetGroups(ObjectId id)
        {
            return _groups.Find(group => group.ClientIds.Contains(id)).ToList();
        }
        
        public void AddGroup(ObjectId clientId, ObjectId groupId, bool addClientToGroup = true)
        {

            var client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            var group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            
            if (client == null || group == null) return;
            
            var groupIds = new List<ObjectId>(client.GroupIds) {group.Id};
            client.GroupIds = groupIds;
            Update(client.Id, client);

            if (!addClientToGroup) return;

            _services.GetRequiredService<GroupService>().AddClient(group.Id, client.Id, false);
        }
        
        public void RemoveGroup(ObjectId clientId, ObjectId groupId, bool removeClientFromGroup = true)
        {

            var client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            var group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            
            if (client == null || group == null) return;
            
            client.GroupIds.Remove(group.Id);
            Update(client.Id, client);

            if (!removeClientFromGroup) return;

            _services.GetRequiredService<GroupService>().RemoveClient(group.Id, client.Id, false);
        }
    }
}