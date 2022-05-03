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
            // CreateCategory(connection);
            // UpdateCategory(connection);
            CreateManyCategory(connection);
            ListCategories(connection);
        }
    }

    static void ListCategories(SqlConnection connection)
    {
        IEnumerable<Category> categories = connection.Query<Category>("SELECT [Id], [Title] From [Category]");
        foreach (Category item in categories)
        {
            Console.WriteLine($"{item.Id} - {item.Title}");
        }
    }

    static void CreateCategory(SqlConnection connection)
    {
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
    }

    static void CreateManyCategory(SqlConnection connection)
    {
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

        var category2 = new Category()
        {
            Id = Guid.NewGuid(),
            Title = "Categoria Nova",
            Url = "categoria-nova",
            Description = "Categoria nova",
            Order = 9,
            Summary = "Categoria",
            Featured = true
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

        int affectedRows = connection.Execute(insertSql, new[] {
            new {
                Id = category.Id,
                Title = category.Title,
                Url = category.Url,
                Summary = category.Summary,
                Order = category.Order,
                Description = category.Description,
                Featured = category.Featured,
            },
            new {
                Id = category2.Id,
                Title = category2.Title,
                Url = category2.Url,
                Summary = category2.Summary,
                Order = category2.Order,
                Description = category2.Description,
                Featured = category2.Featured,
            }
        });

        Console.WriteLine($"{affectedRows} linhas inseridas.");
    }

    static void UpdateCategory(SqlConnection connection)
    {
        var updateCategory = "UPDATE [Category] SET [Title]=@title WHERE [Id]=@id";

        int affectedRows = connection.Execute(updateCategory, new
        {
            id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
            title = "FrontEnd 2021",
        });

        Console.WriteLine($"{affectedRows} registros atualizados.");
    }
}
