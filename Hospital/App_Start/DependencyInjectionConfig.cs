using Autofac.Integration.Mvc;
using Autofac;
using Raven.Client.Document;
using Raven.Client;
using Raven.Client.Extensions;
using AspNet.Identity.RavenDB.Stores;
using Hospital.Models;
using Microsoft.AspNet.Identity;
using System.Reflection;

namespace Hospital
{
    public static class DependencyInjectionConfig
    {
        const string RavenDatabaseName = "hospital";

        public static AutofacDependencyResolver InitDependencyInjection()
        {
            var builder = new ContainerBuilder();

            builder.Register(c =>
            {
                var store = new DocumentStore
                {
                    ConnectionStringName = "RavenDbConnectionString"
                }.Initialize();

                store.DatabaseCommands.EnsureDatabaseExists(RavenDatabaseName);

                return store;
            }).As<IDocumentStore>().SingleInstance();

            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerHttpRequest();
            builder.Register(c => c.Resolve<IDocumentStore>().OpenSession()).As<IDocumentSession>().InstancePerHttpRequest();

            builder.Register(c => new RavenUserStore<ApplicationUser>(c.Resolve<IAsyncDocumentSession>(), false))
                                .As<IUserStore<ApplicationUser>>().InstancePerHttpRequest();

            builder.RegisterType<UserManager<ApplicationUser>>().InstancePerHttpRequest();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            return new AutofacDependencyResolver(builder.Build());
        }
    }
}