﻿using System.Data;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.Core;
using SimpleMigrations;
using SimpleMigrations.VersionProvider;
using Smoother.IoC.Dapper.Repository.UnitOfWork.Data;
using Smoother.IoC.Dapper.Repository.UnitOfWork.SQLite;
using Smoother.IoC.Dapper.Repository.UnitOfWork.UoW;

namespace Smoother.IoC.Dapper.FastCRUD.Repository.UnitOfWork.Tests.TestClasses.Migrations
{
    public class MigrateDb
    {
        public ISession Connection { get; }
        public MigrateDb()
        {
            var migrationsAssembly = Assembly.GetExecutingAssembly();
            var versionProvider = new SqliteVersionProvider();
            var factory = A.Fake<IDbFactory>();
            Connection = new SqliteSession(factory, "Data Source=:memory:;Version=3;New=True;");
            A.CallTo(() => factory.CreateUnitOwWork<IUnitOfWork>(A<IDbFactory>._, A<IDbConnection>._))
                .ReturnsLazily(CreateUnitOrWork);
            var migrator = new SimpleMigrator(migrationsAssembly, Connection, versionProvider);
            migrator.Load();
            migrator.MigrateToLatest();
        }

        private IUnitOfWork CreateUnitOrWork(IFakeObjectCall arg)
        {
            return new Dapper.Repository.UnitOfWork.Data.UnitOfWork((IDbFactory) arg.FakedObject, Connection);
        }
    }
}