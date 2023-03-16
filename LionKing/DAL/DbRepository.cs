using LionKing.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
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
                    //Animal_name = (string)reader["animal_name"]

                    // If retrived value is null, transforms into null value in C#
                    Animal_name = ConvertFromDBVal<string>(reader["animal_name"])
                };

            }

            return animal;
        }

        /// <summary>
        /// Adds animal to database, with species and class relations
        /// </summary>
        /// <param name="animal"></param>
        /// <returns></returns>
        public async Task<Animal> AddAnimal(Animal animal)
        {
            try
            {
                string stmt = "INSERT INTO Animal(animal_name, class_id, species_id) VALUES (@animal_name, @class_id, @species_id) RETURNING animal_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                //command.Parameters.AddWithValue("animal_name", (object) animal.Animal_name ?? DBNull.Value);
                command.Parameters.AddWithValue("@animal_name", ConvertToDBValue<string>(animal.Animal_name)); // not sure this is the right way to do this?? Or should it be "animal_name" instead of animal.Animal_name

                command.Parameters.AddWithValue("class_id", animal.Species.Class.Class_id); // Added species as a prop in animal and class as a prop in species
                command.Parameters.AddWithValue("species_id", animal.Species.Species_id);
                animal.Animal_id =(int) await command .ExecuteScalarAsync();
                return animal;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel";
                string errorCode = ex.SqlState;
                switch (errorCode)
                {
                    //case PostgresErrorCodes.:
                    //    errorMessage = "";
                    //    break;
                    default:
                        break;
                }
                throw new Exception(errorMessage);
            }
        }
        /// <summary>
        /// Adds species to database and returns species_id
        /// </summary>
        /// <param name="species"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Animal> AddSpecies(Species species)
        {
            try
            {
                string stmt = "INSERT INTO Species(species_name, latin_name, class_id) VALUES (@species_name, @latin_name, @class_id) RETURNING species_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@species_name", species.Species_name); //NOT NULL in our table. Should set to not null here and in postrgresql
                command.Parameters.AddWithValue("@latin_name", ConvertToDBValue<string>(species.Latin_name)); //Nullable in our table as well. 
                command.Parameters.AddWithValue("@class_id", species.Class_id); 

              
                species.Species_id = (int)await command.ExecuteScalarAsync();
                return species; // i cant figure out why i get this error?
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel"; //Unsure of which errors we can expect
                string errorCode = ex.SqlState;
                switch (errorCode)
                {
                    //case PostgresErrorCodes.:
                    //    errorMessage = "";
                    //    break;
                    default:
                        break;
                }
                throw new Exception(errorMessage);
            }
        }

       /// <summary>
       /// Adds class to database and returns class_id
       /// </summary>
       /// <param name="animalClass"></param>
       /// <returns></returns>
       /// <exception cref="Exception"></exception>
        public async Task<Animal> AddClass(AnimalClass animalClass)
        {
            try
            {
                string stmt = "INSERT INTO Species(class_name) VALUES (@class_name) RETURNING class_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

               
                command.Parameters.AddWithValue("@class_name", ConvertToDBValue<string>(animalClass.Class_name)); //Nullable in our table as well. 
                


                animalClass.Class_id = (int)await command.ExecuteScalarAsync();
                return animalClass; // i cant figure out why i get this error?
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel"; //Unsure of which errors we can expect
                string errorCode = ex.SqlState;
                switch (errorCode)
                {
                    //case PostgresErrorCodes.:
                    //    errorMessage = "";
                    //    break;
                    default:
                        break;
                }
                throw new Exception(errorMessage);
            }
        }




        /// <summary>
        /// Transforms null valu from database to null value in c#. If not null, returns object. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T? ConvertFromDBVal<T>(object obj) // T is an arbitraty object, can be anything (string, int....)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default; // if database is null return null, otherwise return object. Default value is null!
            }
            return (T)obj;
        }

        /// <summary>
        /// Transforms inserted null valu to null valu in database. If not null, returns obejct. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static object ConvertToDBValue<T> (object obj)
        {
            if (obj == null || obj == string.Empty)
            {
                return DBNull.Value;
            }
            return (T)obj;
        }
    }
}

