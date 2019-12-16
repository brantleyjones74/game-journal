using game_journal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace game_journal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpClientFactory _clientFactory;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GameGenre> GameGenres { get; set; }
        public DbSet<GamePlatform> GamePlatforms { get; set; }

        //protected override async void OnModelCreating(ModelBuilder modelBuilder)
        //{

            //base.OnModelCreating(modelBuilder);

            //// Seed data for platforms
            //var platformRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api-v3.igdb.com/platforms?fields=id,name");
            //var platformClient = _clientFactory.CreateClient("igdb");
            //var platformResponse = await platformClient.SendAsync(platformRequest);
            //var platformAsJson = await platformResponse.Content.ReadAsStringAsync();
            //var deserializedPlatform = JsonConvert.DeserializeObject<List<Platform>>(platformAsJson);

            //foreach (Platform platform in deserializedPlatform)
            //{
                //Platform newPlatform = new Platform
                //{
                    //ApiPlatformId = platform.ApiPlatformId,
                    //Name = platform.Name
                //};
                //modelBuilder.Entity<Platform>().HasData(newPlatform);
            //}
        //}
    }
}