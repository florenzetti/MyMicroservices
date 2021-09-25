using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace MyMicroservices.Customers.UnitTest
{
    public abstract class DbContextUnitTest<TContext> : IDisposable
        where TContext : DbContext
    {
        private readonly DbConnection _connection;

        public DbContextOptions ContextOptions { get; }

        public DbContextUnitTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            ContextOptions = new DbContextOptionsBuilder<TContext>()
                    .UseSqlite(_connection)
                    .Options;
        }

        public virtual void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
