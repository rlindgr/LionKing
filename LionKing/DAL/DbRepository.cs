using LionKing.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
        public async Task<Animal> AddAnimal(string animalName, string speciesName, string className)
        {
            try
            {
                string stmt = "INSERT INTO Animal(animal_name, class_id, species_id) VALUES (@animal_name, @class_id, @species_id) RETURNING animal_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);
                Species species = await GetSpeciesId(speciesName);
                AnimalClass animalClass = await GetClassId(className);
               
                command.Parameters.AddWithValue("@animal_name", ConvertToDBValue<string>(animalName)); // not sure this is the right way to do this?? How does c# cope with unfilled boxes?
                
                command.Parameters.AddWithValue("class_id", animalClass.Class_id);
                command.Parameters.AddWithValue("species_id", species.Species_id);
                int animalId =(int) await command .ExecuteScalarAsync();
                Animal animal = new Animal(animalName, species, animalClass, animalId);
                return animal;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel";
                string errorCode = ex.SqlState;
                switch (errorCode)
                {
                    case PostgresErrorCodes.CharacterNotInRepertoire:
                        errorMessage = "Använd endast bokstäver när du anger namnet på ett djur, en art eller en klass"; //When searching for a class or species if a number has been inserted.(?)
                        break;
                    case PostgresErrorCodes.NullValueNotAllowed: //species txt can't be empty = null.
                        errorMessage = "Du måste ange art när du skapar ett nytt djur"; 
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
        public async Task<Species> AddSpecies(string speciesName, string speciesLatinName, string className)
        {
            try
            {
                
                string stmt = "INSERT INTO Species(species_name, latin_name, class_id) VALUES (@species_name, @latin_name, @class_id) RETURNING species_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                AnimalClass animalClass = await GetClassId(className);

                command.Parameters.AddWithValue("@species_name", speciesName); //NOT NULL in our table. Should set to not null here and in postrgresql
                command.Parameters.AddWithValue("@latin_name", ConvertToDBValue<string>(speciesLatinName)); //Nullable in our table as well. 
                command.Parameters.AddWithValue("@class_id", animalClass.Class_id);

                //Species newSpecies = new Species(speciesName, speciesLatinName);
                //newSpecies.Species_id = (int)await command.ExecuteScalarAsync();
                //return newSpecies; 

                int speciesId = (int)await command.ExecuteScalarAsync();
                Species newSpecies = new Species(speciesName, speciesLatinName, speciesId);
                return newSpecies;
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
        public async Task<AnimalClass> AddClass(string className)
        {
            try
            {
                string stmt = "INSERT INTO Animal_class(class_name) VALUES (@class_name) RETURNING class_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@class_name", className);
               
                int id = (int)await command.ExecuteScalarAsync();
                AnimalClass newClass = new AnimalClass(id, className);
                return newClass; 
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel"; //Unsure of which errors we can expect
                string errorCode = ex.SqlState;
                switch (errorCode)
                {
                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "Denna klass finns redan, klassen som skapas måste vara unik";
                        break;
                    
                }
                throw new Exception(errorMessage);
            }

        }

        /// <summary>
        /// Fetches animal_name based on animal_id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Animal> GetAnimalById(int id)
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
                    Animal_name = (string)reader["animal_name"] // Use method for if the name is null. I don't want to change since im unsure if you are still working on it.
                };

            }

            return animal;
        }

        /// <summary>
        /// Retrives id for and sets name, and latin name for retrived species.
        /// </summary>
        /// <param name="speciesName"></param>
        /// <returns></returns>
        public async Task<Species> GetSpeciesId(string speciesName)
        {
            string stmt = "SELECT * FROM species WHERE species_name = @species_name";
            await using var dataSource = NpgsqlDataSource.Create (_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("@species_name", speciesName);
            await using var reader = await command.ExecuteReaderAsync();
            Species species = new Species();
            while (await reader.ReadAsync())
            {
                species = new Species()
                {
                    Species_id = reader.GetInt32(0),
                    Species_name = (string)reader["species_name"],
                    Latin_name = ConvertFromDBVal<string>(reader["latin_name"]), // is this possible to use to retrive values from other co0lumns perhaps???
                    Class.Class_id = reader.GetInt32(3) // why isn't this possible? Shouldn't i be able to acces class_id via prop AnimalClass Class in species??
                };
            }
            return species;
        }

        /// <summary>
        /// Retrives id and sets name for retrived class. 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<AnimalClass> GetClassId (string className)
        {
            string stmt = "SELECT * FROM animal_class WHERE class_name = @class_name";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("@class_name", className);
            await using var reader = await command.ExecuteReaderAsync();
            AnimalClass animalClass = new AnimalClass();
            while (await reader.ReadAsync()) 
            {
                animalClass = new AnimalClass()
                {
                    Class_id = reader.GetInt32(0),
                    Class_name = (string)reader["class_name"]
                };

            }
            return animalClass;
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

