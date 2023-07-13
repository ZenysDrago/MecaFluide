using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using list = System.Collections.Generic.List<Particle>;
using vector2 = UnityEngine.Vector2;

public class ParticleManager : MonoBehaviour
{
    public list particles = new list();

    #region Parameters

    /*  List of parameters needed  */
    
    public float TIMESTEP = 20f; 
    public float GRAVITY = 0.02f * 0.25f;  // Acceleration of gravity
    public float SPACING = 0.08f;  // Spacing between particles
    public float K = 0.08f / 1000f;  // Pressure factor
    public float K_NEAR = 0.08f / 100f;  // Near pressure factor
    public float BASE_DENSITY = 3.0f;

    // Neighbour radius, if the distance between two particles is less than R, they are neighbours
    public float RADIUS = 0.08f * 1.25f;
    public float VISCOSITY_FACTOR = 0.2f;
    public float MAX_VEL = 0.25f;  // Maximum velocity of particles, used to avoid instability
    
    // Wall constraints
    public float WALL_DAMP = 0.2f;
    public float WALL_POS = 0.08f; 
    
    // Utility variables
    private float density;
    private float densityNear;
    private float dist;
    private float distance;
    private float normalDistance;
    private float relativeDistance;
    private float totalPressure;
    private float velocityDifference;
    private vector2 pressureForce;
    private vector2 particuleToNeighbor;
    private vector2 pressureVector;
    private vector2 normalPtoN;
    private vector2 viscosityForce;
    
    // Base Particle Object
    public GameObject Base_Particle;

    // Spatial Partitioning Grid Variables
    public int gridSizeX = 60;
    public int gridSizeY = 30;
    public list[,] grid;
    public float xMin = -9f;
    public float xMax = 9f;
    public float yMin = 1f;
    public float yMax = 12f;

    #endregion
   
    void Start()
    {
        for (int i = 0; i < 600; i++)
        {
            float x = Random.Range(4f, 7f);
            float y = Random.Range(10f, 45f);
            particles.Add(Instantiate(Base_Particle, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Particle>());
        }

        // Initialize spatial partitioning grid
        grid = new list[gridSizeX, gridSizeY];
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                grid[i, j] = new list();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleGridForParticles();

        // Update particles one by one.
        foreach (Particle p in particles)
        {
            p.UpdateParticle();
        }

        ComputeDensity();

        foreach (Particle p in particles)
        {
            p.CalculatePressure();
        }

        ComputePressure();

        ComputeViscosity();
    }

    #region PhysicCalculation

     private void ComputeViscosity()
    {
        // Viscosity
        foreach (Particle p in particles)
        {
            foreach (Particle n in p.neighbours)
            {
                particuleToNeighbor = n.pos - p.pos;
                distance = Vector2.Distance(p.pos, n.pos);
                normalPtoN = particuleToNeighbor.normalized;
                relativeDistance = distance / RADIUS;
                velocityDifference = Vector2.Dot(p.vel - n.vel, normalPtoN);
                if (velocityDifference > 0)
                {
                    viscosityForce = (1 - relativeDistance) * velocityDifference * VISCOSITY_FACTOR * normalPtoN;
                    p.vel -= viscosityForce * 0.5f;
                    n.vel += viscosityForce * 0.5f;
                }
            }
        }
    }

    private void ComputePressure()
    {
        // Pressure
        foreach (Particle p in particles)
        {
            pressureForce = vector2.zero;

            foreach (Particle n in p.neighbours)
            {
                particuleToNeighbor = n.pos - p.pos;
                distance = Vector2.Distance(p.pos, n.pos);

                normalDistance = 1 - distance / RADIUS;
                totalPressure = (p.press + n.press) * normalDistance * normalDistance + (p.press_near + n.press_near) * Mathf.Pow(normalDistance, 3);
                pressureVector = totalPressure * particuleToNeighbor.normalized;
                n.force += pressureVector;
                pressureForce += pressureVector;
            }
            p.force -= pressureForce;
        }
    }

    private void HandleGridForParticles()
    {
        
        // Assign particles to spatial partitioning grid
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                grid[i, j].Clear();
            }
        }
        foreach (Particle p in particles)
        {
            // Assign grid_x and grid_y using x_min y_min x_max y_max
            p.grid_x = (int)((p.pos.x - xMin) / (xMax - xMin) * gridSizeX);
            p.grid_y = (int)((p.pos.y - yMin) / (yMax - yMin) * gridSizeY);

            // Add particle to grid if it is within bounds
            if (p.grid_x >= 0 && p.grid_x < gridSizeX && p.grid_y >= 0 && p.grid_y < gridSizeY)
            {
                grid[p.grid_x, p.grid_y].Add(p);
            }
        }
    }

    private void ComputeDensity()
    {
        // For each particle
        foreach (Particle p in particles)
        {
            // for each particle in the 9 neighboring cells in the spatial partitioning grid
            for (int i = p.grid_x - 1; i <= p.grid_x + 1; i++)
            {
                for (int j = p.grid_y - 1; j <= p.grid_y + 1; j++)
                {
                    // If the cell is in the grid
                    if (i >= 0 && i < gridSizeX && j >= 0 && j < gridSizeY)
                    {
                        // For each particle in the cell
                        foreach (Particle n in grid[i, j])
                        {
                            // Calculate distance between particles
                            dist = Vector2.Distance(p.pos, n.pos);

                            if (dist < RADIUS)
                            {
                                normalDistance = 1 - dist / RADIUS;
                                p.rho += normalDistance * normalDistance;
                                p.rho_near += normalDistance * normalDistance * normalDistance;
                                n.rho += normalDistance * normalDistance;
                                n.rho_near += normalDistance * normalDistance * normalDistance;

                                // Add n to p's neighbors for later use
                                p.neighbours.Add(n);
                            }
                        }
                    }
                }
            }
        }
    }
    

    #endregion
   
}
