using Dapper;
using Microsoft.Data.SqlClient;
using Dapper;
using BaltaDataAccess.Models;
using System.Data;

public class Program
{
    private static void Main(string [] args)
    {
        const string? connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=Dan@8257;Trusted_Connection=False;TrustServerCertificate=True;";

        //var connection = new SqlConnection(connectionString);

        #region SOMENTE ADO.NET
        //using (var connection = new SqlConnection(connectionString))
        //{
            //Console.WriteLine("Conectado");
            //connection.Open();

            //using(var command = new SqlCommand())
            //{
            //    command.Connection = connection;
            //    command.CommandType = System.Data.CommandType.Text;
            //    command.CommandText = "SELECT [Id], [Title] FROM [Category]";

            //    var reader = command.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
            //    }
            //}
        //}
        #endregion

        using(var connection = new SqlConnection(connectionString))
        {
            //CreateCategory(connection);
            //CreateManyCategories(connection);
            //UpdateCategory(connection);
            //DeleteCategory(connection);
            //GetCategory(connection);
            //ListCategories(connection);
            //ExecuteProcedure(connection);
            //ExecuteReadProcedure(connection);
            //ExecuteScalar(connection);
            //ReadView(connection);
            //OneToOne(connection);
            //OneToMany(connection);
            //QueryMultiple(connection);
            //SelectIn(connection);
            //Like(connection, "api");
            Transaction(connection);
        }
    }

    static void ListCategories(SqlConnection connection)
    {
        var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
        foreach (var item in categories)
        {
            Console.WriteLine($"{item.Id} - {item.Title}");
        }
    }

    static void GetCategory(SqlConnection connection)
    {
        var category = connection.QueryFirstOrDefault<Category>("SELECT TOP 1 [Id], [Title] FROM [Category] WHERE [Id] = @id",
            new
            {
                id = new Guid("B4C5AF73-7E02-4FF7-951C-F69EE1729CAC"),
            });
        Console.WriteLine($"{category.Id} - {category.Title}");
    }

    static void CreateCategory(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços do AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;

        var insertSql = $@"INSERT INTO
            [Category]
        VALUES (
            @Id,
            @Title,
            @Url,
            @Summary,
            @Order,
            @Description,
            @Featured)";

        var rows = connection.Execute(insertSql, new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        });
        Console.WriteLine($"{rows} linhas inseridas.");
    }

    static void UpdateCategory(SqlConnection connection)
    {
        var updateQuery = "UPDATE [CATEGORY] SET [Title] = @title WHERE [Id] = @id";
        var rows = connection.Execute(updateQuery, new
        {
            id = new Guid("AF3407AA-11AE-4621-A2EF-2028B85507C4"),
            title = "Frontend 2021"
        });

        Console.WriteLine($"{rows} registros atualizados.");
    }

    static void DeleteCategory(SqlConnection connection)
    {
        var deleteQuery = "DELETE FROM [CATEGORY] WHERE [Id] = @id";
        var rows = connection.Execute(deleteQuery, new
        {
            id = new Guid("FE2D687A-F3CD-4FB2-83B1-9B9605A886EF"),
        });

        Console.WriteLine($"{rows} registros atualizados.");
    }

    static void CreateManyCategories(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços do AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;

        var category2 = new Category();
        category2.Id = Guid.NewGuid();
        category2.Title = "Categoria nova";
        category2.Url = "categoria-nova";
        category2.Description = "Categoria nova";
        category2.Order = 9;
        category2.Summary = "Categoria";
        category2.Featured = true;

        var insertSql = $@"INSERT INTO
            [Category]
        VALUES (
            @Id,
            @Title,
            @Url,
            @Summary,
            @Order,
            @Description,
            @Featured)";

        var rows = connection.Execute(insertSql, new []
        {
            new {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            },
            new {
                category2.Id,
                category2.Title,
                category2.Url,
                category2.Summary,
                category2.Order,
                category2.Description,
                category2.Featured
            }
        });
        Console.WriteLine($"{rows} linhas inseridas.");
    }

    static void ExecuteProcedure(SqlConnection connection)
    {
        var procedure = "[spDeleteStudent]";
        var pars = new { StudentId = "90069d47-3797-4cbc-8962-48854ffed994" };

        var affectedRows = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);
        Console.WriteLine($"{affectedRows} linhas afetadas");
    }

    static void ExecuteReadProcedure(SqlConnection connection)
    {
        var procedure = "[spGetCoursesByCategory]";
        var pars = new { CategoryId = "09CE0B7B-CFCA-497B-92C0-3290AD9D5142" };

        var courses = connection.Query(procedure, pars, commandType: CommandType.StoredProcedure);

        foreach ( var item in courses )
        {
            Console.WriteLine(item.Title);
        }
    }

    static void ExecuteScalar(SqlConnection connection)
    {
        var category = new Category();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços do AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;

        var insertSql = $@"INSERT INTO
            [Category]
        OUTPUT inserted.[Id]
        VALUES (
            NEWID(),
            @Title,
            @Url,
            @Summary,
            @Order,
            @Description,
            @Featured)
            SELECT SCOPE_IDENTITY()";

        var id = connection.ExecuteScalar<Guid>(insertSql, new
        {
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        });
        Console.WriteLine($"A categoria inserida foi: {id}.");
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
        var sql = @"SELECT * FROM [CareerItem] INNER JOIN [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

        var items = connection.Query<CareerItem, Course, CareerItem>(
            sql,
            (careerItem, course) => {
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
        var sql = @"
            SELECT 
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
            (career, item) => {
                var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();

                if (car == null)
                {
                    car = career;
                    car.Items.Add(item);
                    careers.Add(car);
                }
                else
                {
                    car.Items.Add(item);
                }
                return career;
            }, splitOn: "CareerId");

        foreach (var career in careers)
        {
            Console.WriteLine($"{career.Title}");
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
        var query = "SELECT * FROM [Career] where [Id] in @Id";

        var items = connection.Query<Career>(query, new
        {
            Id = new []
            {
                "E6730D1C-6870-4DF3-AE68-438624E04C72",
                "92D7E864-BEA5-4812-80CC-C2F4E94DB1AF"
            }
        });

        foreach (var item in items)
        {
            Console.WriteLine(item.Title);
        }
    }

    static void Like(SqlConnection connection, string term)
    {
        var query = "SELECT * FROM [Course] WHERE [Title] LIKE @exp";

        var items = connection.Query<Course>(query, new
        {
            exp = $"%{term}%"
        });

        foreach (var item in items)
        {
            Console.WriteLine(item.Title);
        }
    }

    static void Transaction(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "não quero salvar";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços do AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;

        var insertSql = $@"INSERT INTO
            [Category]
        VALUES (
            @Id,
            @Title,
            @Url,
            @Summary,
            @Order,
            @Description,
            @Featured)";

        connection.Open();

        using (var transaction = connection.BeginTransaction())
        {
            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            }, transaction);

            transaction.Commit();
            //transaction.Rollback();

            Console.WriteLine($"{rows} linhas inseridas.");
        }
    }
}