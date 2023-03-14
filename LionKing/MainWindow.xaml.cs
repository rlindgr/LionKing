using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LionKing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // försöker igen jhk
        }
        //public async void AddAnimal(Animal animal)
        //{
        //    try
        //    {
        //        await using var command = dataSource.CreateCommand(stmt);
        //        string stmt = $"INSERT INTO Animal VALUES(@Animal_id, Animal_name)";
        //        await using var command = NpgsqlDataSource.Create(_connectionString);
        //        command.Parameters.AddWithValue("Animal_id", animal.Animal_id);
        //        await command.ExecuteNonQueryAsync();

        //    }
        //    catch (PostgresException ex)
        //    {
        //        string mess = ex.Message;
        //        throw;
        //    }

        //}
    }
}
