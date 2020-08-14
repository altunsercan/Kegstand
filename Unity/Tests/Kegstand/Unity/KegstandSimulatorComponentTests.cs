using System.Collections;
using System.Linq;
using Kegstand.Unity.Impl;
using NSubstitute;
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

        private KegComponent MakeKeg(StandComponent standComponent, string name = "Keg")
        {
            var keg = standComponent.gameObject.AddComponent<KegComponent>();
            keg.Id = name;
            return keg;
        }
        
        
        private TapComponent MakeTap(StandComponent standComponent, string name = "Tap")
        {
            var tap = standComponent.gameObject.AddComponent<TapComponent>();
            tap.Id = name;
            return tap;
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

            var standDefBuilder = Substitute.For<IStandDefinitionBuilder>();
            
            MakeSimulator();
            simulationComp.AutoRegisterComponentsInScene = true;
            simulationComp.Initialize(standDefBuilder);
            
            var scene = SceneManager.GetActiveScene();
            
            StandComponent stand = MakeStandObject();
            MakeNestedStand(3, 1, 3);
            MakeNestedStand(5, 5);
            
            yield return new EnterPlayMode();
            
            Assert.AreEqual( 4, simulationComp.Stands.Count);
            standDefBuilder.Received(4)
                .BuildWrappers(Arg.Any<IWrapperComponent<Stand>>(), Arg.Any<IStandDefinitionProvider>());
        }

        [UnityTest]
        public IEnumerator StandDefinitionBuilderTest()
        {
            // Given
            yield return null;
            IStandDefinitionBuilder defBuilder = new StandDefinitionBuilder(null);
            
            StandComponent stand = MakeStandObject();
            stand.AutoAddSiblingComponents = true;
            MakeKeg(stand, "keg1");
            MakeKeg(stand, "keg2");
            MakeTap(stand, "tap1");
            MakeTap(stand, "tap2");

            // When
            var pureStand = defBuilder.BuildWrappers(standWrapper: stand, provider: stand);
            
            // Then
            Assert.AreEqual(2, pureStand.Kegs.Count);
            Assert.NotNull(pureStand.Kegs[0].Keg.TapList);
            Assert.NotNull(pureStand.Kegs[1].Keg.TapList);
            
            Assert.AreEqual(2, pureStand.Taps.Count);
            Assert.DoesNotThrow(()=> { var flow = pureStand.Taps[0].Tap.FlowAmount; });
            Assert.DoesNotThrow(()=> { var flow = pureStand.Taps[1].Tap.FlowAmount; });
        }

    }
}