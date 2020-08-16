using Kegstand;
using Kegstand.Impl;
using Kegstand.Unity;
using UnityEngine;
using UnityEngine.UI;

public class KegValueBar : MonoBehaviour
{
    [SerializeField] private StandComponent stand;
    [SerializeField] private string id;

    [SerializeField] public Image fillBar;
    
    private Keg keg;
    
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
    }

    void Update()
    {
        // TODO: need to eliminate by reading keg value through simulator.
        fillBar.fillAmount = keg.Amount(NullAmountVisitor.Instance) / keg.MaxAmount;
    }
}
