using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticleUiManager : MonoBehaviour
{
    public Slider viscositySlider;

    private ParticleManager particleManager;

    void Start()
    {
        particleManager = FindObjectOfType<ParticleManager>();
    }

    void Update()
    {
        particleManager.VISCOSITY_FACTOR = viscositySlider.value;
    }
}
