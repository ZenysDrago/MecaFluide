using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonUI : MonoBehaviour
{
    
    public void SwitchScene()
    {
        Scene cur = SceneManager.GetActiveScene();
        if(cur.name == "WaterTank")
            SceneManager.LoadScene("Scenes/Particles");
        else
            SceneManager.LoadScene("Scenes/WaterTank");

    }
}
