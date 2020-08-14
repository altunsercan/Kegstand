namespace Kegstand.Unity
{
    public interface IStandDefinitionBuilder
    {
        Stand BuildWrappers(IWrapperComponent<Stand> standWrapper, IStandDefinitionProvider provider);
    }
}