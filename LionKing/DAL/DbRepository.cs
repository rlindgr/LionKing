using LionKing.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static System.Net.Mime.MediaTypeNames;

namespace LionKing.DAL
{
    public class DbRepository
    {
        #region Database connection
        private readonly string _connectionString;
        //när vi skapar ett objekt av DbRepository då vill vi också få tillgång till vår connectionString dvs vi använder konstruktorn
        public DbRepository()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<DbRepository>().Build();
            _connectionString = config.GetConnectionString("develop");
        }
        #endregion


        #region Lists
        public List<AnimalClass> animalClasses { get; set; } = new List<AnimalClass>();

        public List<Species> speciesList { get; set; } = new List<Species>();
        public List<Animal> animals { get; set; } = new List<Animal>(); //To be changed to Animals (Listname)

        #endregion




        #region CREATE

        /// <summary>
        /// Adds animal to database, with species and class relations
        /// </summary>
        /// <param name="animal"></param>
        /// <returns></returns>
        public async Task<Animal> AddAnimal(string animalName, Species species)
        {
            try
            {
                string stmt = "INSERT INTO Animal(animal_name, class_id, species_id) VALUES (@animal_name, @class_id, @species_id) RETURNING animal_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);


                command.Parameters.AddWithValue("@animal_name", ConvertToDBValue<string>(animalName)); // not sure this is the right way to do this?? How does c# cope with unfilled boxes?
                command.Parameters.AddWithValue("@class_id", species.Class_id);
                command.Parameters.AddWithValue("@species_id", species.Species_id);


                Animal animal = new Animal();
                animal.Animal_id = (int)await command.ExecuteScalarAsync();
                animal.Animal_name = animalName;
                animal.Species_id = species.Species_id;
                animal.Class_id = species.Class_id;
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
        public async Task<Species> AddSpecies(string speciesName, string speciesLatinName, AnimalClass animalClass)
        {
            try
            {

                string stmt = "INSERT INTO Species(species_name, latin_name, class_id) VALUES (@species_name, @latin_name, @class_id) RETURNING species_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@species_name", speciesName);
                command.Parameters.AddWithValue("@latin_name", ConvertToDBValue<string>(speciesLatinName));
                command.Parameters.AddWithValue("@class_id", animalClass.Class_id);


                Species newSpecies = new Species();
                newSpecies.Species_id = (int)await command.ExecuteScalarAsync();
                newSpecies.Latin_name = speciesLatinName;
                newSpecies.Species_name = speciesName;
                newSpecies.Class_id = animalClass.Class_id;
                return newSpecies;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel";
                string errorCode = ex.SqlState;
                switch (errorCode)
                {
                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "Denna art finns redan, arten som skapas måste vara unik";
                        break;
                    case PostgresErrorCodes.NullValueNotAllowed:
                        errorMessage = "Du måste ange ett namn för arten du skapar, textrutan för namn får ej lämnas tom";
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

                AnimalClass newClass = new AnimalClass();
                newClass.Class_id = (int)await command.ExecuteScalarAsync();
                newClass.Class_name = className;

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

        #endregion

        #region READ

        /// <summary>
        /// Get all animal classes for comboboxes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AnimalClass>> GetAllClasses()
        {

            animalClasses.Clear();
            string stmt = "SELECT * FROM Animal_class";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(stmt);
            await using var reader = await command.ExecuteReaderAsync();
            AnimalClass newClass = new AnimalClass();

            while (await reader.ReadAsync())
            {
                newClass = new AnimalClass()
                {
                    Class_id = reader.GetInt32(0),
                    Class_name = (string)reader.GetString(1)
                };
                animalClasses.Add(newClass);
            }
            return animalClasses;
        }
        /// <summary>
        /// Get all species for comboboxes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Species>> GetAllSpecies()
        {
            speciesList.Clear();
            string stmt = "SELECT * FROM Species";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(stmt);
            await using var reader = await command.ExecuteReaderAsync();
            Species species = new Species();

            while (await reader.ReadAsync())
            {
                species = new Species()
                {
                    Species_id = reader.GetInt32(0),
                    Species_name = (string)reader.GetString(1),
                    Latin_name = ConvertFromDBVal<string>(reader["latin_name"]),
                    Class_id = reader.GetInt32(3)

                };
                speciesList.Add(species);
            }
            return speciesList;
        }
        /// <summary>
        /// Get all animals for comboboxes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Animal>> GetAllAnimals()
        {
            animals.Clear();
            string stmt = "SELECT * FROM Animal";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(stmt);
            await using var reader = await command.ExecuteReaderAsync();
            Animal newAnimals = new Animal();

            while (await reader.ReadAsync())
            {

                newAnimals = new Animal()
                {
                    Animal_id = reader.GetInt32(0),
                    Animal_name = ConvertFromDBVal<string>(reader["animal_name"]),
                    Class_id = reader.GetInt32(2),
                    Species_id = reader.GetInt32(3)
                };

                animals.Add(newAnimals);
            }
            return animals;
        }

        /// <summary>
        /// Retrieves animal by animal_name
        /// </summary>
        /// <param name="fetchAnimal"></param>
        /// <returns></returns>
        public async Task<Animal> GetAnimalByName(string animalName) 
        {
            try
            {
                string stmt = "SELECT * FROM animal WHERE animal_name = @name";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);

                await using var command = dataSource.CreateCommand(stmt);
                command.Parameters.AddWithValue("@name", animalName);
                await using var reader = await command.ExecuteReaderAsync();

                Animal animal = new Animal();
                while (await reader.ReadAsync())
                {
                    animal = new Animal()
                    {
                        Animal_id = reader.GetInt32(0),
                        Animal_name = ConvertFromDBVal<string>(reader["animal_name"]),
                        Class_id = reader.GetInt32(2),
                        Species_id = reader.GetInt32(3)
                    };
                }
                return animal;
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
        /// Retrieves animal_names based on Species_id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Animal>> GetAnimalsBySpeciesId(int id)
        {

            string stmt = "SELECT * FROM animal WHERE species_id = @id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("@id", id);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            while (await reader.ReadAsync())
            {
                animal = new Animal()
                {
                    Animal_id = reader.GetInt32(0),
                    Animal_name = ConvertFromDBVal<string>(reader["animal_name"]), 
                    Class_id = reader.GetInt32(2),
                    Species_id = reader.GetInt32(3)
                };
                animals.Add(animal);

            }
            return animals;
        }
        /// <summary>
       /// Retrives animals based on Class_id.
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        public async Task<IEnumerable<Animal>> GetAnimalsByClassId(int id)
        {

            string stmt = "SELECT * FROM animal WHERE class_id = @id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("@id", id);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            while (await reader.ReadAsync())
            {
                animal = new Animal()
                {
                    Animal_id = reader.GetInt32(0),
                    Animal_name = ConvertFromDBVal<string>(reader["animal_name"]) 
                };
                animals.Add(animal);

            }
            return animals;
        }
        /// <summary>
        /// Retrives Class_name based on Class_id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AnimalClass> GetClassById(int id)
        {

            string stmt = "SELECT * FROM animal_class WHERE class_id = @id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("id", id);
            await using var reader = await command.ExecuteReaderAsync();

            AnimalClass animalClass = new AnimalClass();
            while (await reader.ReadAsync())
            {
                animalClass = new AnimalClass()
                {
                    Class_name = (string)reader["class_name"],
                    Class_id = reader.GetInt32(0)
                };
            }

            return animalClass;
        }
       
        /// <summary>
        /// Retrives Species_name based on Species_id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Species> GetSpeciesById(int id)
        {
            string stmt = "SELECT * FROM species WHERE species_id = @id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("id", id);
            await using var reader = await command.ExecuteReaderAsync();

            Species species = new Species();
            while (await reader.ReadAsync())
            {
                species = new Species()
                {
                    Species_id = reader.GetInt32(0),
                    Species_name = (string)reader["species_name"],
                    Latin_name = (string)reader["latin_name"]
                };

            }
            return species;
        }
 
        /// <summary>
        /// Uses class_id to get all species belonging to that class
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Species>> GetAllSpeciesFromClass(int id)
        {
            //List<Species> speciesList = new List<Species>();

            string stmt = "SELECT * FROM species WHERE class_id = @id";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(stmt);
            command.Parameters.AddWithValue("id", id);
            await using var reader = await command.ExecuteReaderAsync();

            Species species = new Species();
            while (await reader.ReadAsync())
            {
                species = new Species()
                {
                    Species_name = (string)reader["species_name"],
                    Latin_name = (string)reader["latin_name"],
                    Species_id = reader.GetInt32(0)
                };
                speciesList.Add(species);
            }

            return speciesList;
        }

        #endregion

        #region UPDATE
        /// <summary>
        /// Takes new AnimalClass from interface and updates database
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Animal> UpdateClass(Animal oldAnimal, AnimalClass newAnimalClass ) 
        {
            if (newAnimalClass != null)
            {
                try
                {
                    string stmt = "UPDATE animal SET class_id = @class_id WHERE animal_id = @animal_id";
                    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                    await using var command = dataSource.CreateCommand(stmt);

                    command.Parameters.AddWithValue("@class_id", newAnimalClass.Class_id);
                    command.Parameters.AddWithValue("@animal_id", oldAnimal.Animal_id);
                    await using var reader = await command.ExecuteReaderAsync();

                    Animal updatedAnimal = new Animal();
                    updatedAnimal.Class_id = newAnimalClass.Class_id;
                    updatedAnimal.Animal_name = oldAnimal.Animal_name;
                    updatedAnimal.Animal_id = oldAnimal.Animal_id;
                    updatedAnimal.Species_id = oldAnimal.Species_id;

                    return updatedAnimal;
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
            else
            {
                return oldAnimal;

            }
            
           

        }
        public async Task<Animal> UpdateClassBySpecies(Animal oldAnimal, AnimalClass newAnimalClass) 
        {
            if (newAnimalClass != null)
            {
                try
                {
                    string stmt = "UPDATE animal SET class_id = @class_id WHERE species_id = @species_id";
                    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                    await using var command = dataSource.CreateCommand(stmt);

                    command.Parameters.AddWithValue("@class_id", newAnimalClass.Class_id);
                    command.Parameters.AddWithValue("@species_id", oldAnimal.Species_id);
                    await using var reader = await command.ExecuteReaderAsync();

                    Animal updatedAnimal = new Animal();
                    updatedAnimal.Class_id = newAnimalClass.Class_id;
                    updatedAnimal.Animal_id = oldAnimal.Animal_id;
                    updatedAnimal.Animal_name = oldAnimal.Animal_name;
                    updatedAnimal.Species_id = oldAnimal.Species_id;

                    return updatedAnimal;
                }
                catch (PostgresException ex)
                {
                    string errorMessage = "Något gick fel"; //Unsure of which errors we can expect
                    string errorCode = ex.SqlState;
                    switch (errorCode)
                    {
                        case PostgresErrorCodes.UniqueViolation:
                            errorMessage = "Denna klass finns redan, klassen som skapas måste vara unik"; //Ska vi använda denna?
                            break;

                    }
                    throw new Exception(errorMessage);
                }
            }
            else { return oldAnimal; }

        }

        public async Task<Animal> UpdateAnimalName (Animal oldAnimal, string newName) 
        {
            if (newName != null)
            {
                try
                {
                    string stmt = "UPDATE animal SET animal_name=@animal_name WHERE animal_id=@animal_id";
                    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                    await using var command = dataSource.CreateCommand(stmt);

                    command.Parameters.AddWithValue("@animal_name", (string)newName);
                    command.Parameters.AddWithValue("@animal_id", oldAnimal.Animal_id);
                    await using var reader = await command.ExecuteReaderAsync();

                    Animal updatedAnimal = new Animal();
                    updatedAnimal.Animal_name = newName;
                    updatedAnimal.Class_id = oldAnimal.Class_id;
                    updatedAnimal.Species_id = oldAnimal.Species_id;
                    updatedAnimal.Animal_id = oldAnimal.Animal_id;
                    return updatedAnimal;
                }


                catch (PostgresException ex)
                {
                    string errorMessage = "Något gick fel"; //Unsure of which errors we can expect
                    string errorCode = ex.SqlState;
                    //switch (errorCode)
                    //{
                    //    case PostgresErrorCodes.UniqueViolation:
                    //        errorMessage = "Denna klass finns redan, klassen som skapas måste vara unik"; //Ska vi använda denna?
                    //        break;

                    //}

                    throw new Exception(errorMessage);
                }
            }

            else { return oldAnimal; }
        }

        public async Task<Animal> UpdateSpecies (Animal oldAnimal, Species newSpecies)
        {
            if (newSpecies != null)
            {
                try
                {
                    string stmt = "UPDATE animal SET species_id=@species_id WHERE animal_id=@animal_id";
                    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                    await using var command = dataSource.CreateCommand(stmt);

                    command.Parameters.AddWithValue("@species_id", newSpecies.Species_id);
                    command.Parameters.AddWithValue("@animal_id", oldAnimal.Animal_id);
                    await using var reader = await command.ExecuteReaderAsync();

                    Animal updatedAnimal = new Animal();
                    updatedAnimal.Animal_name = oldAnimal.Animal_name;
                    updatedAnimal.Class_id = oldAnimal.Class_id;
                    updatedAnimal.Species_id = newSpecies.Species_id;
                    updatedAnimal.Animal_id = oldAnimal.Animal_id;
                    return updatedAnimal;

                }
                catch (PostgresException ex)
                {
                    string errorMessage = "Något gick fel"; //Unsure of which errors we can expect
                    string errorCode = ex.SqlState;
                    //switch (errorCode)
                    //{
                    //    case PostgresErrorCodes.UniqueViolation:
                    //        errorMessage = "Denna klass finns redan, klassen som skapas måste vara unik"; //Ska vi använda denna?
                    //        break;

                    //}

                    throw new Exception(errorMessage);
                }


            }
            else { return oldAnimal; }
        }





        #endregion

        #region DELETE

        /// <summary>
        /// Delete animal from database
        /// </summary>
        /// <param name="animal"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Animal> DeleteAnimal(Animal animal)
        {
            try
            {
                string stmt = "DELETE FROM animal WHERE animal_id=@animal_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@animal_id", animal.Animal_id);
                await using var reader = await command.ExecuteReaderAsync();

                return animal;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel, djuret har inte raderats vänligen försök igen";
                throw new Exception(errorMessage);
            }

        }

       /// <summary>
       /// Deletes all animals with specific species_id
       /// </summary>
       /// <param name="species"></param>
       /// <exception cref="Exception"></exception>
        public async void DeleteAllAnimalsInSpecies(Species species)
        {
            try
            {

                string stmt = "DELETE FROM animal WHERE species_id=@species_id;";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@species_id", species.Species_id);
                await using var reader = await command.ExecuteReaderAsync();
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel, djuren i den valda djurarten har inte raderats";
                throw new Exception(errorMessage);
            }
           
        }
       /// <summary>
       /// Deletes all animals with specific class_id
       /// </summary>
       /// <param name="animalClass"></param>
       /// <exception cref="Exception"></exception>
        public async void DeleteAllAnimalsInClass(AnimalClass animalClass)
        {
            try
            {

                string stmt = "DELETE FROM animal WHERE class_id=@class_id;";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@class_id", animalClass.Class_id);
                await using var reader = await command.ExecuteReaderAsync();
            
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel, djuren i den valda klassen har inte raderats";
                throw new Exception(errorMessage);
            }

        }

        /// <summary>
        /// Deletes choosen species
        /// </summary>
        /// <param name="species"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Species> DeleteSpecies(Species species)
        {
            try
            {
                string stmt = "DELETE FROM species WHERE species_id=@species_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@species_id", species.Species_id);
                await using var reader = await command.ExecuteReaderAsync(); // Reader when i want to delete? why, there are other alternatives??

                return species;

            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel, djurarten har inte raderats, vänligen försök igen";
                throw new Exception (errorMessage);
            }
        }

        /// <summary>
        /// Deletes choosen animalClass
        /// </summary>
        /// <param name="animalClass"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AnimalClass> DeleteAnimalClass (AnimalClass animalClass)
        {
            try
            {
                string stmt = "DELETE FROM animal_class WHERE class_id=@class_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@class_id", animalClass.Class_id);
                await using var reader = await command.ExecuteReaderAsync(); // Reader when i want to delete? why, there are other alternatives??

                return animalClass;

            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel, djurklassen har inte raderats, vänligen försök igen";
                throw new Exception(errorMessage);
            }
        }

        public async void DeleteAllSpeciesinClass(AnimalClass animalClass)
        {
            try
            {
                string stmt = "DELETE FROM species WHERE class_id=@class_id";
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(stmt);

                command.Parameters.AddWithValue("@class_id", animalClass.Class_id);
                await using var reader = await command.ExecuteReaderAsync(); // Reader when i want to delete? why, there are other alternatives??

            }
            catch (PostgresException ex)
            {
                string errorMessage = "Något gick fel, arterna i den valda klassen har inte raderats, vänligen försök igen";
                throw new Exception(errorMessage);
            }

        }

        #endregion



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
        private static object ConvertToDBValue<T>(object obj)
        {
                if (obj == null || obj == string.Empty)
                {
                    return DBNull.Value;
                }
                return (T)obj;
        }

        public void SortAnimalByNameAcs() 
        {
            animals = animals.OrderBy(animals => animals.Animal_name).ToList();          
        }

        public void SortAnimalByNameDecs()
        {
           SortAnimalByNameAcs();
           animals.Reverse();

        }
    }
}  


