using System;
using Kegstand;
using Kegstand.Unity;
using UnityEngine;
using UnityEngine.UI;

public class KegValueBar : MonoBehaviour, IObserver<float>
{
    public Keg Keg => keg;
    
    [SerializeField] private KegstandSimulationComponent simulation;
    [SerializeField] private StandComponent stand;
    [SerializeField] private string id;

    [SerializeField] public Image fillBar;

    private bool initialized;
    private Keg keg;
    private IDisposable listener;
    
    public void Start()
    {
        if (stand.IsInitialized)
        {
            InitializeKegBar();
        }
        else
        {
            stand.Initialized += OnDelayedInitialized;
        }
    }

    private void OnDelayedInitialized(StandComponent standComponent)
    {
        standComponent.Initialized -= OnDelayedInitialized;
        InitializeKegBar();
    }

    private void InitializeKegBar()
    {
        keg = stand.GetKeg(id);

        enabled = fillBar != null;
        initialized = true;

        if (enabled)
        {
            TryStartListening();
        }
    }

    private void OnEnable()
    {
        ValidateOrDisableComponent(()=>simulation == null, $"Cannot activate {nameof(KegValueBar)} without setting simulation first.");
        ValidateOrDisableComponent(()=>stand == null, $"Cannot activate {nameof(KegValueBar)} without setting stand first.");
        ValidateOrDisableComponent(()=>fillBar== null, $"Cannot activate {nameof(KegValueBar)} without setting fill bar first.");

        TryStartListening();
        
        void ValidateOrDisableComponent(Func<bool> check, string errorMessage)
        {
            if (check())
            {
                Debug.LogError(errorMessage);
                enabled = false;
            }    
        }
    }

    private void OnDisable()
    {
        StopListening();
    }

    private void TryStartListening()
    {
        if (initialized && enabled)
        {
            listener = simulation.Simulator.ObserveKegFill(keg, this);
        }
    }
    
    private void StopListening()
    {
        listener?.Dispose();   
    }

    void IObserver<float>.OnCompleted() => StopListening();
    void IObserver<float>.OnError(Exception error) { }

    void IObserver<float>.OnNext(float value)
    {
        fillBar.fillAmount = value / keg.MaxAmount;
    }
}
