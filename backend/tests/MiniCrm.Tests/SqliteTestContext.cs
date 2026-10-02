using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniCrm.Domain.Entities;
using MiniCrm.Infrastructure.Data;

namespace MiniCrm.Tests;

internal sealed class SqliteTestContext : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Db { get; }

    public Asesor Asesor { get; }

    public SqliteTestContext()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new AppDbContext(options);
        Db.Database.EnsureCreated();

        Asesor = new Asesor { Nombre = "Laura Fernández" };
        Db.Asesores.Add(Asesor);
        Db.SaveChanges();
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
