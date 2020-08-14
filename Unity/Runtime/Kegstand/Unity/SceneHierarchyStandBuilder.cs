using System;
using Kegstand.Impl;

namespace Kegstand.Unity
{
    public class SceneHierarchyStandBuilder
    {
        private readonly StandBase.Builder standBuilder = new StandBase.Builder();
        
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