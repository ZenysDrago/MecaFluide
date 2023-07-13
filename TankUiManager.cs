using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankUiManager : MonoBehaviour
{
    private bool switcher = true;
    [SerializeField] private Tank tank;

    public GameObject mainCanvas;
    public GameObject holeCanvas;
    public Slider holeHeight;
    public Slider tankHeight;
    public Slider diameterHole;
    public TextMeshProUGUI tankHeightText;
    private Hole holeComp;
    public float newHeight = 0;
    public GameObject backGroundTank;
    public TextMeshProUGUI holeDiametertext;
    public TextMeshProUGUI timeEmptyText;
    public TextMeshProUGUI outputSpeedText;

    public void Start()
    {
        SwitchUI();
        UIUpdate();
        SwitchUI();
    }

    public void CreateHole()
    {
        tank.AddHole(holeComp);
        if (holeComp) Destroy(holeComp.gameObject);
        holeComp = tank.CreateHole();
        holeHeight.value = 0;
    }

    public void Update()
    {
        UIUpdate();
    }

    private void UIUpdate()
    {
        float minutes = Mathf.Floor(tank.timeEmpty / 60f);
        float seconds = (tank.timeEmpty % 60f);
        timeEmptyText.text = string.Format("Empty in {0}min {1}s", minutes.ToString("0"), seconds.ToString("0"));

        if (tank.holes.Count > 0)
        {
            float speed = tank.holes[0].GetComponent<Hole>().speed;
            outputSpeedText.text = string.Format("Output speed {0}m/s", speed.ToString("0.0"));
        }

        if (holeComp)
        {
            Vector3 tankPos   = tank.transform.position;
            Vector3 tankScale = tank.waterLevel.transform.localScale;
            
            newHeight = Mathf.Lerp(tankPos.y - tankScale.y / 2,
                tankPos.y + tankScale.y / 2, holeHeight.value);

            holeComp.transform.position = new Vector3(holeComp.transform.position.x,
                newHeight + tankScale.y/2, holeComp.transform.position.z);
        }

        if (!switcher)
        {
            tank.size.y = Mathf.Round(tankHeight.value * 100f) / 100f;
            tank.waterHeight = tank.size.y;
            tank.maxWaterHeight = tank.waterHeight;
            tankHeightText.text = tank.size.y.ToString();

            holeComp.diameter = Mathf.Round(diameterHole.value * 100f) / 100f;
            holeDiametertext.text = holeComp.diameter.ToString();
        }
    }

    public void SwitchUI()
    {
        mainCanvas.SetActive(!switcher);
        holeCanvas.SetActive(switcher);

        if (switcher)
        {
            holeComp = tank.CreateHole();
            foreach (GameObject holeGO in tank.holes)
            {
                Hole hole = holeGO.GetComponent<Hole>();
                hole.isPlaying = false;
                hole.particles.Stop();
            }
        }
        else
        {
            foreach (GameObject holeGO in tank.holes)
            {
                Hole hole = holeGO.GetComponent<Hole>();
                hole.isPlaying = true;
                hole.particles.Play();
            }

            tank.timerNotFull = 0;
            
            if (holeComp)
                Destroy(holeComp.gameObject);
        }
        
        
        switcher = !switcher;
    }
    
    public void FillTank()
    {
        tank.shouldFillTank = true;
    }
}
