using System;

namespace Kegstand
{
    public interface Clock
    {
        void Update(float deltaSeconds);
        ref TimeSpan GetCurrentTimePassed();
    }
}