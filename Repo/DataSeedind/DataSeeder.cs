using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using Microsoft.Extensions.Hosting;

namespace Repo.DataSeedind
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly string _env;

        public DataSeeder(ApplicationDbContext context, string env)
        {
            _context = context;
            _env = env;
        }


        public async Task SeedAsync()
        {
            if (! _context.ContractTemplates.Any())
            {
                await SeedContractTemplatesAsync();
            }        
        }
            
               



        public async Task SeedContractTemplatesAsync()
        {
        

            var filePath = Path.Combine(_env, "Data", "ContractTemplate" , "Template.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seeding file not found at path: {filePath}");

          
            var jsonData = await File.ReadAllTextAsync(filePath);
            var templates = JsonSerializer.Deserialize<List<ContractTemplate>>(jsonData) ?? new List<ContractTemplate>();

        
            await _context.ContractTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();
        }
    }
}
