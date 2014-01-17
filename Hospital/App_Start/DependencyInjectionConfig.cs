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
                    Url = "https://lark.ravenhq.com/databases/AppHarbor_c18f9245-031f-4e74-9a0e-df70bbff4e47",
                    ApiKey = "0cfba5fc-d526-4a64-af88-a442c5bf2234"
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