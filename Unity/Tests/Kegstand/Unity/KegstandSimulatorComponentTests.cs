using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Kegstand.Unity
{
    public class KegstandSimulatorComponentTests
    {
        private KegstandSimulationComponent simulationComp;

        private void MakeSimulator(bool inactive = false)
        {
            GameObject simulatorGobj = new GameObject("Simulator");
            if (inactive)
            {
                simulatorGobj.SetActive(false);
            }
            simulationComp = simulatorGobj.AddComponent<KegstandSimulationComponent>();
        }
        
        private StandComponent MakeStandObject(string name = "Stand")
        {
            GameObject standGameObj = new GameObject(name);
            return standGameObj.AddComponent<StandComponent>();
        }

        private KegComponent MakeKegObject(string name = "Keg")
        {
            GameObject kegGameObj = new GameObject(name);
            return kegGameObj.AddComponent<KegComponent>();
        }
        
        private void MakeNestedStand(int nestLevel, params int[] standInLevel)
        {
            GameObject currentNest = new GameObject();
            int currentNestLevel = 0;
            while (currentNestLevel<=nestLevel)
            {
                currentNestLevel++;
                GameObject tempGobj = null;
                if (standInLevel.Contains(currentNestLevel))
                {
                    var standObj = MakeStandObject();
                    tempGobj = standObj.gameObject;
                }
                else
                {
                    tempGobj = new GameObject();
                }
                    
                tempGobj.transform.SetParent(currentNest.transform);
                currentNest = tempGobj;
            }
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new ExitPlayMode();
            
            simulationComp = null;
            
            var sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (var index = sceneObjects.Length - 1; index >= 1; index--) // Don't destroy first object which is test runner
            {
                GameObject obj = sceneObjects[index];

                GameObject.Destroy(obj);
            }

            yield return null;
        }
        

        [UnityTest]
        public IEnumerator ShouldCreateSimulationOnStart()
        {
            yield return null;

            MakeSimulator(inactive: true);
            
            Assert.IsNull(simulationComp.Simulator);
            simulationComp.Awake();
            Assert.IsNotNull(simulationComp.Simulator);
        }


        [UnityTest]
        public IEnumerator ShouldAutoDiscoverExistingComponentsInScene()
        {
            yield return null;
            
            MakeSimulator();
            simulationComp.AutoRegisterComponentsInScene = true;

            var scene = SceneManager.GetActiveScene();
            
            StandComponent stand = MakeStandObject();
            MakeNestedStand(3, 1, 3);
            MakeNestedStand(5, 5);
            
            yield return new EnterPlayMode();
            
            Assert.AreEqual( 4, simulationComp.Stands.Count);
        }

    }
}