using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Kegstand.Unity
{
    [TestFixture]
    public abstract class ComponentWrapperTestFixture<TComponent, TWrapped> 
        where TComponent: MonoBehaviour, TWrapped, IWrapperComponent<TWrapped>
        where TWrapped: class
    {
        public static IEnumerable<MethodInfo> WrappedMethodsSource()
        {
            var members = typeof(TWrapped).GetMembers();

            foreach (MemberInfo memberInfo in members)
            {
                if (memberInfo.MemberType == MemberTypes.Method)
                {
                    yield return (MethodInfo) memberInfo;
                }
            }
        }
        
        public static IEnumerable<PropertyInfo> WrappedPropertySource()
        {
            var members = typeof(TWrapped).GetMembers();

            foreach (MemberInfo memberInfo in members)
            {
                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    yield return (PropertyInfo) memberInfo;
                }
            }
        }
        
        private WaitForEndOfFrame waitEndOfFrame = new WaitForEndOfFrame();
        private object[] emptyParams = new object[0]; 
            
        private TWrapped wrappedObject;
        private TComponent wrapperComponent;
        [UnitySetUp]
        public IEnumerator Setup()
        {
            wrappedObject = Substitute.For<TWrapped>();
            
            GameObject gameObject = new GameObject("wrapperObj", typeof(TComponent));
            wrapperComponent = gameObject.GetComponent<TComponent>();

            if (wrapperComponent != null)
            {
                wrapperComponent.SetWrappedObject(wrappedObject);
            }

            yield return waitEndOfFrame;
        }
        

        [UnityTest]
        public IEnumerator TestMethodWrapping([ValueSource(nameof(WrappedMethodsSource))] MethodInfo methodInfo)
        {
            Assert.NotNull(methodInfo, "Invalid value source");

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            object[] parameters = new object[parameterInfos.Length];
            
            for (var index = 0; index < parameterInfos.Length; index++)
            {
                ParameterInfo parameterInfo = parameterInfos[index];
                parameters[index] = MakeSubstituteType(parameterInfo.ParameterType);
            }
            
            methodInfo.Invoke(wrapperComponent, parameters);
            methodInfo.Invoke(wrappedObject.ReceivedWithAnyArgs(1), parameters);
            
            yield return waitEndOfFrame;
        }
        
        [UnityTest]
        public IEnumerator TestPropertyWrapping([ValueSource(nameof(WrappedPropertySource))] PropertyInfo propertyInfo)
        {
            Assert.NotNull(propertyInfo, "Invalid value source");

            MethodInfo getMethod = propertyInfo.GetMethod;
            if (getMethod != null)
            {
                getMethod.Invoke(wrapperComponent, emptyParams);
                getMethod.Invoke(wrappedObject.ReceivedWithAnyArgs(1), emptyParams);
            }
            
            MethodInfo setMethod = propertyInfo.SetMethod;
            if (setMethod != null)
            {
                var substitute = MakeSubstituteType(propertyInfo.PropertyType);
            
                setMethod.Invoke(wrapperComponent, emptyParams);
                setMethod.Invoke(wrappedObject.ReceivedWithAnyArgs(1), emptyParams);
            }
            
            yield return waitEndOfFrame;
        }

        private object MakeSubstituteType(Type type)
        {
            if (type == null)
            {
                return null;
            }
            
            TypeInfo typeInfo = type.GetTypeInfo();
            if (typeInfo == null)
            {
                return null;
            }
            
            if (typeInfo.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return Substitute.For(new Type[]{type}, emptyParams);
            }
        }
    
        
    }

    public class KegstandSimulatorComponentWrapperTests : ComponentWrapperTestFixture<KegComponent, Keg>
    {
        
    }
}