using System.Collections;
using System.Collections.Generic;
using Kegstand;
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
        keg = stand.GetKeg(id);

        enabled = fillBar != null;
    }

    void Update()
    {
        fillBar.fillAmount = keg.Amount / keg.MaxAmount;
    }
}
