using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionKing.DAL
{
    public class DbRepository
    {

        private readonly string _connectionString;
        //när vi skapar ett objekt av DbRepository då vill vi också få tillgång till vår connectionString dvs vi använder konstruktorn
        public DbRepository()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<DbRepository>().Build();
            _connectionString = config.GetConnectionString("develop");
        }


        public async Task<Animal> GetAnimal(int id)
        {

            string stmt = "select * from animal where animal_id = @id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("id", id);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            while (await reader.ReadAsync())
            {
                animal = new Animal()
                {
                    Animal_id = reader.GetInt32(0),
                    Animal_name = (string)reader["animal_name"]
                };

            }

            return animal;
        }
    }
}

