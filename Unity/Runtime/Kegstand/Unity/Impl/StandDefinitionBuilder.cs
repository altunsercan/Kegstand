using System.Collections.Generic;
using JetBrains.Annotations;
using Kegstand.Impl;
using UnityEngine.Assertions;

namespace Kegstand.Unity.Impl
{
    public class StandDefinitionBuilder : IStandDefinitionBuilder
    {
        private readonly FlowCalculator flowCalculator;

        public StandDefinitionBuilder(FlowCalculator flowCalculator)
        {
            this.flowCalculator = flowCalculator;
        }
        
        public Stand BuildWrappers(IWrapperComponent<Stand> standWrapper, IStandDefinitionProvider provider)
        {
            Assert.IsNotNull(standWrapper);
            Assert.IsNotNull(provider);
            
            StandDefinition definition = provider.GetStandDefinition();
            if (definition == null)
            {
                return null;
            }

            BuildKegWrappers(definition.Kegs);
            
            BuildTapWrappers(definition.Taps);

            var standBuilder = new StandBase.Builder();
            standBuilder.CopyDefinition( definition );
            return standBuilder.Build();
        }

        private static void BuildTapWrappers([NotNull] List<TapEntry> tapList)
        {
            foreach (TapEntry tapEntry in tapList)
            {
                Tap tap = tapEntry.Tap;
                if (tap is IWrapperComponent<Tap> tapWrapper)
                {
                    var pureTap = new TapBase();
                    pureTap.SetFlow(tap.FlowAmount);
                    tapWrapper.SetWrappedObject(pureTap);
                }

                if (tap is TapComponent tapComponent && tapComponent.ConnectedKegs != null)
                {
                    foreach (KegComponent keg in tapComponent.ConnectedKegs)
                    {
                        keg.AddTap(tap);
                    }
                }
            }
        }

        private void BuildKegWrappers([NotNull] List<KegEntry> kegList)
        {
            foreach (KegEntry kegEntry in kegList)
            {
                Keg keg = kegEntry.Keg;

                if (keg is IWrapperComponent<Keg> kegWrapper)
                {
                    var kegBuilder = new KegBase.Builder<KegBase>();
                    kegBuilder.Max(keg.MaxAmount);
                    kegBuilder.Min(keg.MinAmount);
                    kegBuilder.StartWith(keg.Amount(NullAmountVisitor.Instance));
                    kegBuilder.WithCalculator(flowCalculator);
                    KegBase pureKeg = kegBuilder.Build();
                    kegWrapper.SetWrappedObject(pureKeg);
                }
            }
        }
    }
}