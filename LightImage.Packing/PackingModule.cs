using Autofac;

namespace LightImage.Packing
{
    public class PackingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<WeightedPackingService>().As<IWeightedPackingService>().SingleInstance();
            builder.RegisterType<PackingService>().As<IPackingService>().SingleInstance();
        }
    }
}