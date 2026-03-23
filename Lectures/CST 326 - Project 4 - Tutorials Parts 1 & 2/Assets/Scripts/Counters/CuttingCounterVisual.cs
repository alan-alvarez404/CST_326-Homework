using System;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    private const string CUT = "Cut";
    
    [SerializeField] private CuttingCounter cuttingCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCut += CuttingCOunter_OnCut;
    }

    private void CuttingCOunter_OnCut(object sender, EventArgs e)
    {
        animator.SetTrigger(CUT);
    }

}
