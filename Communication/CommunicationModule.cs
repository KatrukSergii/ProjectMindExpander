using Autofac;
using Communication.ParserStrategy;
using Shared;

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

            builder.RegisterType<DraftTimesheetParserStrategy>().Keyed<ITimesheetParserStrategy>(TimesheetType.Draft);
            builder.RegisterType<ApprovedTimesheetParserStrategy>().Keyed<ITimesheetParserStrategy>(TimesheetType.Approved);
        }
    }
}
