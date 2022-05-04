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
            // OneToOne(connection);
            // OneToMany(connection);
            // QueryMultiple(connection);
            Like(connection);
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

    static void OneToMany(SqlConnection connection)
    {
        var sql = @"SELECT 
                [Career].[Id],
                [Career].[Title],
                [CareerItem].[CareerId],
                [CareerItem].[Title]
            FROM 
                [Career] 
            INNER JOIN 
                [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
            ORDER BY
                [Career].[Title]";

        var careers = new List<Career>();

        var items = connection.Query<Career, CareerItem, Career>(
            sql,
            (career, careerItem) =>
            {
                var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                if (car == null)
                {
                    car = career;
                    car.Items.Add(careerItem);
                    careers.Add(car);
                }
                else
                {
                    car.Items.Add(careerItem);
                }

                return career;
            }, splitOn: "CareerId"
        );

        foreach (var career in careers)
        {
            System.Console.WriteLine(career.Title);
            foreach (var item in career.Items)
            {
                Console.WriteLine($" - {item.Title}");
            }
        }
    }

    static void QueryMultiple(SqlConnection connection)
    {
        var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";

        using (var multi = connection.QueryMultiple(query))
        {
            var categories = multi.Read<Category>();
            var courses = multi.Read<Course>();

            foreach (var item in categories)
            {
                Console.WriteLine(item.Title);
            }

            foreach (var item in courses)
            {
                Console.WriteLine(item.Title);
            }
        }
    }

    static void SelectIn(SqlConnection connection)
    {
        var query = "SELECT * FROM [Career] WHERE [Id] IN @Id";

        var careers = connection.Query<Career>(query, new
        {
            Id = new[] {
                "01ae8a85-b4e8-4194-a0f1-1c6190af54cb",
                "e6730d1c-6870-4df3-ae68-438624e04c72"
            }
        });

        foreach (var item in careers)
        {
            System.Console.WriteLine(item.Title);
        }
    }

    static void Like(SqlConnection connection)
    {
        var term = "api";
        var query = "SELECT * FROM [Course] WHERE [Title] LIKE @exp";

        var careers = connection.Query<Course>(query, new
        {
            exp = $"%{term}%"
        });

        foreach (var item in careers)
        {
            System.Console.WriteLine(item.Title);
        }
    }
}
