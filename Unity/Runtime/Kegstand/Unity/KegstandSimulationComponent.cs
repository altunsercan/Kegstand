using System.Collections.Generic;
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
            foreach (GameObject rootGObj in rootObjects)
            {
                AddStandFromGameObject(rootGObj);
            }
        }

        private void AddStandFromGameObject(GameObject gObj)
        {
            if (gObj.GetComponent<Stand>() is Stand stand)
            {
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
}