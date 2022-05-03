using Dapper;
using DataAccess.Models;
using Microsoft.Data.SqlClient;

namespace DataAccess;

public class Program
{
    static void Main(string[] args)
    {
        const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;TrustServerCertificate=Yes;";
        var category = new Category()
        {
            Id = Guid.NewGuid(),
            Title = "Amazon AWS",
            Url = "amazon",
            Description = "Categoria destinada a serviços do AWS",
            Order = 8,
            Summary = "AWS Cloud",
            Featured = false
        };

        var insertSql = @"INSERT INTO
                [Category] 
            VALUES(
                @Id,
                @Title,
                @Url,
                @Summary,
                @Order,
                @Description,
                @Featured)";

        using (var connection = new SqlConnection(connectionString))
        {
            int affectedRows = connection.Execute(insertSql, new
            {
                Id = category.Id,
                Title = category.Title,
                Url = category.Url,
                Summary = category.Summary,
                Order = category.Order,
                Description = category.Description,
                Featured = category.Featured,
            });

            Console.WriteLine($"{affectedRows} linhas inseridas.");

            // IEnumerable<dynamic>
            IEnumerable<Category> categories = connection.Query<Category>("SELECT [Id], [Title] From [Category]");

            foreach (Category item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }
    }
}
