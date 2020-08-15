using System.Collections.Generic;
using JetBrains.Annotations;
using Kegstand.Impl;
using Kegstand.Unity.Impl;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kegstand.Unity
{
    public class KegstandSimulationComponent : MonoBehaviour
    {
        public bool AutoRegisterComponentsInScene = true;
        public Simulator Simulator { get; private set; }
        public IReadOnlyList<Stand> Stands => Simulator?.Stands;

        private bool initialized;
        private IStandDefinitionBuilder standDefBuilder;
        
        public void Awake()
        {
            Simulator = new Simulator<TimeSpanClock>();
        }

        public void Initialize(IStandDefinitionBuilder builder = null)
        {
            initialized = true;

            if (builder == null)
            {
                builder = new StandDefinitionBuilder(new FlowCalculatorImpl());
            }

            standDefBuilder = builder;
        }

        private void Start()
        {
            if (!initialized)
            {
                Initialize();
            }
            
            if (AutoRegisterComponentsInScene)
            {
                FindExistingKegstandComponentsInScene();
            }
            
        }

        void Update()
        {
            Simulator?.Update(Time.deltaTime);
        }

        private void FindExistingKegstandComponentsInScene()
        {
            var rootObjects = gameObject.scene.GetRootGameObjects();
            Assert.IsNotNull(rootObjects);

            var exploreQueue = new Queue<Transform>(rootObjects.Length);
            foreach (GameObject rootGObj in rootObjects)
            {
                Assert.IsNotNull(rootGObj);
                exploreQueue.Enqueue(rootGObj.transform);
            }

            while (exploreQueue.Count>0)
            {
                Transform transformToExplore = exploreQueue.Dequeue();
                if (transformToExplore == null) { continue; }
                ExploreGameObjectForStand(transformToExplore, exploreQueue);
            }
            
            exploreQueue.Clear();
        }

        private void ExploreGameObjectForStand([NotNull] Transform currentObject, [NotNull] Queue<Transform> exploreQueue)
        {
            AddStandFromGameObjectIfExists(currentObject);
            AppendChildrenToQueue(currentObject.transform, exploreQueue);
        }
        
        private static void AppendChildrenToQueue([NotNull] Transform parent, [NotNull] Queue<Transform> exploreQueue)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                exploreQueue.Enqueue( parent.GetChild(i));
            }
        }

        private void AddStandFromGameObjectIfExists([NotNull] Transform gObj)
        {
            Assert.IsNotNull(standDefBuilder);
            Assert.IsNotNull(Simulator);
            
            if (!(gObj.GetComponent<Stand>() is Stand stand)) return;

            if (stand is IWrapperComponent<Stand> standWrapper)
            {
                Stand pureStand = null;
                if ( stand is IStandDefinitionProvider provider)
                {
                    pureStand = standDefBuilder.BuildWrappers(standWrapper, provider);
                }
                else
                {
                    var standBuilder = new StandBase.Builder();
                    pureStand = standBuilder.Build();
                }
                
                standWrapper.SetWrappedObject(pureStand);
            }
                
            Simulator.Register(stand);
        }
    }
}