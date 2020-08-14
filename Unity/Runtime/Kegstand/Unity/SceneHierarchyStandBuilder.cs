using System;

namespace Kegstand.Unity
{
    public class SceneHierarchyStandBuilder
    {
        private readonly Impl.StandBase.Builder standBuilder = new Impl.StandBase.Builder();
        
        private KegstandSimulationComponent simulationComponent;

        public SceneHierarchyStandBuilder(KegstandSimulationComponent simulationComponent)
        {
            this.simulationComponent = simulationComponent;
        }
        
        public void AddKegComponent(KegComponent kegComponent)
        {
            throw new NotImplementedException();
        }

        public Stand Build()
        {
            throw new NotImplementedException();
        }
    }
}