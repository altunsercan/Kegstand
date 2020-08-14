using System;
using System.Collections.Generic;
using Kegstand.Impl;
using UnityEngine;

namespace Kegstand.Unity
{
    public class KegstandSimulationComponent : MonoBehaviour
    {
        public bool AutoRegisterComponentsInScene = true;
        public Simulator Simulator { get; private set; }
        public IReadOnlyList<Stand> Stands => Simulator.Stands;

        private bool initialized = false;
        private IStandDefinitionBuilder standDefBuilder;
        
        public void Awake()
        {
            Simulator = new Simulator();
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

        public void Update()
        {
            Simulator.Update(Time.deltaTime);
        }

        private void FindExistingKegstandComponentsInScene()
        {
            var rootObjects = gameObject.scene.GetRootGameObjects();
            var exploreQueue = new Queue<Transform>(rootObjects.Length);
            foreach (GameObject rootGObj in rootObjects)
            {
                exploreQueue.Enqueue(rootGObj.transform);
            }

            while (exploreQueue.Count>0)
            {
                ExploreGameObjectForStand(exploreQueue.Dequeue(), exploreQueue);
            }
            
            exploreQueue.Clear();
        }

        private void ExploreGameObjectForStand(Transform currentObject, Queue<Transform> exploreQueue)
        {
            AddStandFromGameObjectIfExists(currentObject);
            AppendChildrenToQueue(currentObject.transform, exploreQueue);
        }
        
        private static void AppendChildrenToQueue(Transform parent, Queue<Transform> exploreQueue)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                exploreQueue.Enqueue( parent.GetChild(i));
            }
        }

        private void AddStandFromGameObjectIfExists(Transform gObj)
        {
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
                    var standBuilder = new Impl.StandBase.Builder();
                    pureStand = standBuilder.Build();
                }
                
                standWrapper.SetWrappedObject(pureStand);
            }
                
            Simulator.Register(stand);
        }
    }
}