namespace Kegstand.Unity
{
    public interface IWrapperComponent<T>
    {
        void SetWrappedObject(T wrappedObject);
    }
}