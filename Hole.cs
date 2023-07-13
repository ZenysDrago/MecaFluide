using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hole : MonoBehaviour
{
    public static float Gravity = 9.81f;
    private Tank tank;
    public float height; // par rapport au reservoir (h reel = h hole + h tank)
    public float speed; // vitesse a la sortie
    public float maxDist;
    public float diameter; 
    public float flowRate; // Qv
    public float volumicMass; // p
    public bool isPlaying = false;
    public bool isPreview = false;
    public ParticleSystem particles;
    
    void Start()
    {
       
    }

    public static float Remap(float val,  float inputStart,  float inputEnd,  float outputStart,  float outputEnd)
    {
        return outputStart + (val - inputStart) * (outputEnd - outputStart) / (inputEnd - inputStart);
    }
        
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPlaying)
        {
            tank.waterHeight = CalculCurrentHeight(tank.timerNotFull) + height;
           
            var main = particles.main;
            float difHeight = (tank.waterHeight - height);
            if (difHeight > 0)
                main.startSpeed = new ParticleSystem.MinMaxCurve(Mathf.Sqrt(2 * Gravity * difHeight));
            else
                particles.Stop();

            
            CalculSpeed();
            CalculTime();
            CalculFlowRate();
           
        }
        
        if (isPreview)
        {
            float scale = Remap(diameter, 1, 50, 0.25f, 1f);
            transform.localScale = new Vector3(0.5f, scale , 1f);
        }
    }

    public void SetTank(Tank _tank)
    {
        tank = _tank;
    }

    public float CalculSpeed()
    {
        float h = tank.waterHeight - height;
        if (h < 0) h = 0;
        speed = Mathf.Sqrt(2 * Gravity * h);
        return speed;
    }

    public float CalculFlowRate()
    {
        float surfaceHole = (Mathf.PI * diameter * diameter) * 0.25f;
        flowRate = speed * surfaceHole;
        return flowRate;
    }

    public float CalculTime()
    {
        float h = tank.waterHeight - height;
        if (h <= 0) { h = 0; isPlaying = false; }
        float surfaceTank = (Mathf.PI * tank.size.x * tank.size.x) * 0.25f;
        float surfaceHole = (Mathf.PI * diameter * diameter) * 0.25f;
        tank.timeEmpty = ((2 * surfaceTank) / (surfaceHole)) * Mathf.Sqrt(h / (2 * Gravity));
        return tank.timeEmpty;
    }

    public float CalculCurrentHeight(float time)
    {
        float surfaceTank = (Mathf.PI * tank.size.x * tank.size.x) * 0.25f;
        float surfaceHole = (Mathf.PI * diameter * diameter) * 0.25f;
        float h = Mathf.Pow((-surfaceHole / (2 *surfaceTank)) * Mathf.Sqrt(2 * Gravity) * time + Mathf.Sqrt(tank.size.y - height), 2);
       
        return h;
    }
}
