using LionKing.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

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

        #region Read

        /// <summary>
        /// Hämtar animal baserat på animal_id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Animal> GetAnimalById(int id)
        {
            string stmt = "select * from animal where animal_id=@id";
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

        /// <summary>
        /// Hämtar alla våra djur ifrån tabellen Animal
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Animal>> GetAnimals()
        {
            List<Animal> animals = new List<Animal>();
            string stmt = "select * from animal where animal_name is not null";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(stmt);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            while (await reader.ReadAsync())
            {
                animal = new Animal()
                {
                    Animal_id = reader.GetInt32(0),
                    Animal_name = (string)reader["animal_name"]

                };
                animals.Add(animal);
            }
            return animals;
            #endregion
        }

        /// <summary>
        /// Letar efter djur som användaren angett i textbox
        /// </summary>
        /// <param name="findName"></param>
        /// <returns></returns>
        public async Task<Animal> FindAnimalByName(string findName)
        {
            
            string stmt = "select animal_name from animal where animal_name=@name";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("name", findName);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            while (await reader.ReadAsync())
            {
                animal = new Animal()
                {
                    Animal_name = (string)reader["animal_name"]
                };

            }
            return animal;
            
        }



        //Försök till join fråga 
        public async Task<IEnumerable<Animal>> GetAnimalsWithExtraInfo()
        {
            List<Animal> animals = new List<Animal>();
            string stmt = "select species_name, latin_name, class_name FROM species JOIN animal_class ON species.class_id = animal_class.class_id;   ";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(stmt);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            while (await reader.ReadAsync())
            {
                animal = new Animal();

                {
                    Animal_name = (string)reader["animal_name"];
                    Latin_name = (string)reader["latin_name"];

                };
                animals.Add(animal);
            }
            return animals;

        }
    }
}

