using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Kegstand.Unity
{
    public class StandComponentWrapperTests : ComponentWrapperTestFixture<StandComponent, Stand>
    {
        private KegstandSimulationComponent simulationComp;
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
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
        public IEnumerable ShouldAssociateWithKegComponentsInSiblings()
        {
            GameObject gameObj = new GameObject();

            StandComponent standComp = gameObj.AddComponent<StandComponent>();
            standComp.AutoAddSiblingComponents = true;
            
            KegComponent keg1Comp = gameObj.AddComponent<KegComponent>();
            keg1Comp.Id = "keg1";
            
            KegComponent keg2Comp = gameObj.AddComponent<KegComponent>();
            keg2Comp.Id = "keg2";

            yield return new EnterPlayMode();
            
            Assert.That( standComp.Kegs, Has.Member(keg1Comp).And.Member(keg2Comp));
        }

    }
}