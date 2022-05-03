using Dapper;
using DataAccess.Models;
using Microsoft.Data.SqlClient;

namespace DataAccess;

public class Program
{
    static void Main(string[] args)
    {
        const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;TrustServerCertificate=Yes;";

        using (var connection = new SqlConnection(connectionString))
        {
            // IEnumerable<dynamic>
            // Pode se utilizar alias na consulta para auxiliar no mapeamento
            IEnumerable<Category> categories = connection.Query<Category>("SELECT [Id] as [Codigo], [Title] as [Titulo] From [Category]");

            foreach (Category category in categories)
            {
                System.Console.WriteLine($"{category.Codigo} - {category.Titulo}");
            }
        }
    }
}
