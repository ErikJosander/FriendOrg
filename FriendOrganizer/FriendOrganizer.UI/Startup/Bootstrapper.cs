using Autofac;
using FriendOrganizer.DataAccess;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<FriendOrganizerDbContext>().AsSelf();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();


            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            //Specifies that a type from a scanned assembly is registered as providing all
            //of its implemented interfaces.
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            //When an IFriendDataService interface is needed the builder will convert it to a FriendDataService Class
            builder.RegisterType<FriendDataService>().As<IFriendDataService>();

            return builder.Build();
        }
    }
}
