using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kegstand.Unity
{
    public class KegstandSimulationComponent : MonoBehaviour
    {
        public bool AutoRegisterComponentsInScene = true;
        public Simulator Simulator { get; private set; }
        public IReadOnlyList<Stand> Stands => Simulator.Stands;

        public void Awake()
        {
            Simulator = new Simulator();
        }

        private void Start()
        {
            if (AutoRegisterComponentsInScene)
            {
                FindExistingKegstandComponentsInScene();
            }
            
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
                var standBuilder = new StandBase.Builder();
                var pureStand = standBuilder.Build();
                standWrapper.SetWrappedObject(pureStand);      
            }
                
            Simulator.Register(stand);
        }
    }
}