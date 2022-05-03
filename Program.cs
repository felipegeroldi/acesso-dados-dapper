using System.Data;
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
            // CreateManyCategory(connection);
            // ListCategories(connection);
            // ExecuteProcedure(connection);
            // ExecuteReadProcedure(connection);
            // ExecuteScalar(connection);
            // ReadView(connection);
            OneToOne(connection);
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

    static void ExecuteProcedure(SqlConnection connection)
    {
        var procedure = "[spDeleteStudent]";
        var parameters = new
        {
            StudentId = "d1e3263a-bb72-4daf-9af2-f64f03470575"
        };

        int affectedRows = connection.Execute(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure);

        Console.WriteLine($"{affectedRows} linhas afetadas.");
    }

    static void ExecuteReadProcedure(SqlConnection connection)
    {
        var procedure = "[spGetCoursesByCategory]";
        var parameters = new
        {
            CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142"
        };

        var courses = connection.Query(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure);

        foreach (var item in courses)
        {
            Console.WriteLine(item.Id);
        }
    }

    static void ExecuteScalar(SqlConnection connection)
    {
        var category = new Category()
        {
            Title = "Amazon AWS",
            Url = "amazon",
            Description = "Categoria destinada a serviços do AWS",
            Order = 8,
            Summary = "AWS Cloud",
            Featured = false
        };

        var insertSql = @"INSERT INTO
                [Category] 
            OUTPUT inserted.[Id]
            VALUES(
                NEWID(),
                @Title,
                @Url,
                @Summary,
                @Order,
                @Description,
                @Featured)";

        var id = connection.ExecuteScalar<Guid>(insertSql, new
        {
            Title = category.Title,
            Url = category.Url,
            Summary = category.Summary,
            Order = category.Order,
            Description = category.Description,
            Featured = category.Featured,
        });

        Console.WriteLine($"A categoria inserida foi: {id}");
    }

    static void ReadView(SqlConnection connection)
    {
        var sql = "SELECT * FROM [vwCourses]";
        var courses = connection.Query(sql);

        foreach (var item in courses)
        {
            Console.WriteLine($"{item.Id} - {item.Title}");
        }
    }

    static void OneToOne(SqlConnection connection)
    {
        var sql = @"
            SELECT
                *
            FROM
                [CareerItem]
            INNER JOIN
                [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

        var items = connection.Query<CareerItem, Course, CareerItem>(
            sql,
            (careerItem, course) =>
            {
                careerItem.Course = course;
                return careerItem;
            }, splitOn: "Id");

        foreach (var item in items)
        {
            Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
        }
    }
}
