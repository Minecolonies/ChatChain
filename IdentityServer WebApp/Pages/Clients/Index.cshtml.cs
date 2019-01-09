using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;
        
        public IndexModel(UserManager<IdentityUser> userManager, ConfigurationDbContext is4Context, GroupsDbContext groupsContext)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            _groupsContext = groupsContext;
        }

        public IList<Client> Clients { get; set; }

        public async Task OnGetAsync()
        {
            Clients = new List<Client>();

            foreach (var client in await _is4Context.Clients.ToListAsync())
            {
                var groupClient = await _groupsContext.Clients.FirstAsync(c => c.ClientId == client.Id);

                if (groupClient.OwnerId == _userManager.GetUserAsync(User).Result.Id)
                {
                    Clients.Add(client);
                }
            }
        }
        //d10d988b-e7f4-4c40-bf86-c1beeaded8b3; 38; Test Client 1; 12/24/2018 19:07:28
    }
}