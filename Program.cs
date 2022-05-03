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
            IEnumerable<Category> categories = connection.Query<Category>("SELECT [Id], [Title] From [Category]");

            foreach (Category category in categories)
            {
                System.Console.WriteLine($"{category.Id} - {category.Title}");
            }
        }
    }
}
