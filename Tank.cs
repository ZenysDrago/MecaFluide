using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tank : MonoBehaviour
{
   
    [SerializeField] private float heightBaseTank;
    [SerializeField] public float waterHeight;
    [SerializeField] public float maxWaterHeight;
    [SerializeField] public float timeEmpty;
    [SerializeField] private GameObject prefabHole;
    
    public List<GameObject> holes;
    public GameObject waterLevel;
    public GameObject waterTop;
    public Vector2 size;
    public float timerNotFull = 0;

    public bool shouldFillTank;
    
    // Start is called before the first frame update
    void Start()
    {
        waterLevel.transform.localScale = new Vector3(waterLevel.transform.localScale.x, size.y, waterLevel.transform.localScale.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (holes.Count > 1)
        {
            Destroy(holes[0]);
            holes.RemoveAt(0);
        }

        if (shouldFillTank)
        {
            timerNotFull = 0;
            waterHeight = maxWaterHeight;
            shouldFillTank = false;
        }
        timerNotFull += Time.fixedDeltaTime;

        waterLevel.transform.localScale  = new Vector3(waterLevel.transform.localScale.x, waterHeight, waterLevel.transform.localScale.z);
        waterTop.transform.localPosition = new Vector3(waterTop.transform.localPosition.x, waterLevel.transform.localScale.y - waterTop.transform.localScale.y, waterTop.transform.localPosition.z);  
    }
 
    public void AddHole(Hole holePreview)
    {
        Vector3 pos = holePreview.transform.position;
        GameObject hole = Instantiate(prefabHole);
        hole.transform.position = new Vector3(waterLevel.transform.position.x + waterLevel.transform.localScale.x * 0.5f, pos.y, pos.z);
        holes.Add(hole);

        Hole holeComp = hole.GetComponent<Hole>();
        holeComp.height = pos.y - heightBaseTank;
        holeComp.transform.localScale = holePreview.transform.localScale;
        holeComp.particles = hole.GetComponentInChildren<ParticleSystem>();
        Vector3 maxScale = holeComp.particles.gameObject.transform.localScale;
        float scale = Hole.Remap(holePreview.diameter, 1, 50, 0.15f, maxScale.x);
        holeComp.particles.gameObject.transform.localScale = new Vector3(scale , scale, scale);
        holeComp.SetTank(this);
    }

    public Hole CreateHole()
    {
        GameObject hole = Instantiate(prefabHole);
        hole.transform.position = new Vector3(waterLevel.transform.position.x + waterLevel.transform.localScale.x * 0.5f, 0, 0);
        Hole holeComp = hole.GetComponent<Hole>();
        holeComp.isPreview = true;
        return holeComp;
    }
}
