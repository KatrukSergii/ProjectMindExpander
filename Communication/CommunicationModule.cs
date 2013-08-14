using Autofac;

namespace Communication
{


    public class CommunicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(CommunicationModule).Assembly)
                        .AsImplementedInterfaces()
                        .AsSelf()
                        .As(q => q.BaseType);
        }
    }
}
