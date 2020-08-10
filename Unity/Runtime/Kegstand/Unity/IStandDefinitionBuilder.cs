namespace Kegstand.Unity
{
    public interface IStandDefinitionBuilder
    {
        Stand BuildWrappers(IWrapperComponent<Stand> standWrapper, IStandDefinitionProvider provider);
    }

    public class StandDefinitionBuilder : IStandDefinitionBuilder
    {
        public StandDefinitionBuilder()
        {
            
        }
        
        public Stand BuildWrappers(IWrapperComponent<Stand> standWrapper, IStandDefinitionProvider provider)
        {
            var definition = provider.GetStandDefinition();

            foreach (KegEntry kegEntry in definition.Kegs)
            {
                var keg = kegEntry.Keg;

                if (keg is IWrapperComponent<Keg> kegWrapper)
                {
                    var kegBuilder = new KegBase.Builder<KegBase>();
                    KegBase pureKeg = kegBuilder.Build();
                    kegWrapper.SetWrappedObject(pureKeg);
                }
            }
            
            
            var standBuilder = new StandBase.Builder();
            standBuilder.CopyDefinition( definition );
            return standBuilder.Build();
        }
    }
}