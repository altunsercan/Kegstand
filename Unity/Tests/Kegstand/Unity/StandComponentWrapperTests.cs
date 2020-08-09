using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Kegstand.Unity
{
    public class StandComponentWrapperTests : ComponentWrapperTestFixture<StandComponent, Stand>
    {
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            var sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (var index = sceneObjects.Length - 1; index >= 1; index--) // Don't destroy first object which is test runner
            {
                GameObject obj = sceneObjects[index];

                GameObject.Destroy(obj);
            }

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator ShouldCreateStandDefinitionFromComponentsInSiblings()
        {
            GameObject gameObj = new GameObject();
            
            StandComponent standComp = gameObj.AddComponent<StandComponent>();
            standComp.AutoAddSiblingComponents = true;
            
            KegComponent keg1Comp = gameObj.AddComponent<KegComponent>();
            keg1Comp.Id = "keg1";
            
            KegComponent keg2Comp = gameObj.AddComponent<KegComponent>();
            keg2Comp.Id = "keg2";
            
            var definition = standComp.GetStandDefinition();
            
            Assert.That(definition.Kegs,
                Has.Some.Matches<KegEntry>(entry=>entry.Keg == keg1Comp && entry.Key == keg1Comp.Id)
                .And.Some.Matches<KegEntry>(entry=>entry.Keg == keg2Comp && entry.Key == keg2Comp.Id));
            
            yield return null;
        }

    }
}