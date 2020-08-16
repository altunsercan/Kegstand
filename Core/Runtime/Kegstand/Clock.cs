using JetBrains.Annotations;

namespace Kegstand
{
    public interface Clock<TTimeValue>
    {
        void Update(float deltaSeconds);
        [NotNull]
        ref TTimeValue GetCurrentTimePassed();
    }
}