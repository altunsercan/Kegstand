namespace Kegstand.Unity
{
    public interface IStandDefinitionBuilder
    {
        Stand BuildWrappers(IWrapperComponent<Stand> standWrapper, IStandDefinitionProvider provider);
    }

    public class StandDefinitionBuilder : IStandDefinitionBuilder
    {
        private readonly FlowCalculator flowCalculator;

        public StandDefinitionBuilder(FlowCalculator flowCalculator)
        {
            this.flowCalculator = flowCalculator;
        }
        
        public Stand BuildWrappers(IWrapperComponent<Stand> standWrapper, IStandDefinitionProvider provider)
        {
            var definition = provider.GetStandDefinition();

            if (definition == null)
            {
                return null;
            }

            foreach (KegEntry kegEntry in definition.Kegs)
            {
                var keg = kegEntry.Keg;

                if (keg is IWrapperComponent<Keg> kegWrapper)
                {
                    var kegBuilder = new KegBase.Builder<KegBase>();
                    // TODO Initialize keg
                    kegBuilder.Max(keg.MaxAmount);
                    kegBuilder.Min(keg.MinAmount);
                    kegBuilder.StartWith(keg.Amount);
                    kegBuilder.WithCalculator(flowCalculator);
                    KegBase pureKeg = kegBuilder.Build();
                    kegWrapper.SetWrappedObject(pureKeg);
                }
            }
            
            foreach (TapEntry tapEntry in definition.Taps)
            {
                var tap = tapEntry.Tap;
                if (tap is IWrapperComponent<Tap> tapWrapper)
                {
                    var pureTap = new TapBase();
                    pureTap.SetFlow(tap.FlowAmount);
                    tapWrapper.SetWrappedObject(pureTap);
                }

                if (tap is TapComponent tapComponent)
                {
                    foreach (KegComponent keg in tapComponent.ConnectedKegs)
                    {
                        keg.AddTap(tap);
                    }
                }
            }

            var standBuilder = new StandBase.Builder();
            standBuilder.CopyDefinition( definition );
            return standBuilder.Build();
        }
    }
}