using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using vector2 = UnityEngine.Vector2;
using list = System.Collections.Generic.List<Particle>;

public class Particle : MonoBehaviour
{
    ParticleManager manager;

    // Physics variables
    public vector2 pos;
    public vector2 previous_pos;
    public vector2 visual_pos;
    public float rho = 0.0f;
    public float rho_near = 0.0f;
    public float press = 0.0f;
    public float press_near = 0.0f;
    public list neighbours = new list();
    public vector2 vel = vector2.zero;
    public vector2 force;
    public float velocity = 0.0f;

    // Spatial partitioning position in grid
    public int grid_x;
    public int grid_y;

    void Start()
    {
        manager = FindObjectOfType<ParticleManager>();

        // Set initial position
        pos = transform.position;
        previous_pos = pos;
        visual_pos = pos;
        force = new vector2(0f, -manager.GRAVITY);
    }

    // Update is called once per frame
    public void UpdateParticle()
    {
        // Reset previous position
        previous_pos = pos;

        // Apply force using Newton's second law and with mass = 1
        vel += force * Time.deltaTime * manager.TIMESTEP;

        // Move particle with velocity
        pos += vel * Time.deltaTime * manager.TIMESTEP;

        // Update visual position
        visual_pos = pos;
        transform.position = visual_pos;

        // Reset force to gravity
        force = new vector2(0, -manager.GRAVITY);

        // Define velocity using Euler integration
        vel = (pos - previous_pos) / Time.deltaTime / manager.TIMESTEP;

        // Calculate velocity
        velocity = vel.magnitude;

        // Set to MAX_VEL if velocity is greater than MAX_VEL
        if (velocity > manager.MAX_VEL)
        {
            vel = vel.normalized * manager.MAX_VEL;
        }

        // Reset density
        rho = 0.0f;
        rho_near = 0.0f;

        // Reset neighbors
        neighbours = new list();
    }

    public void CalculatePressure()
    {
        press = manager.K * (rho - manager.BASE_DENSITY);
        press_near = manager.K_NEAR * rho_near;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Calculate the normal vector of the collision
        vector2 normal = collision.contacts[0].normal;

        // Calculate the velocity of the particle in the normal direction
        float vel_normal = Vector2.Dot(vel, normal);

        // If the velocity is positive, the particle is moving away from the wall
        if (vel_normal > 0)
        {
            return;
        }

        // Calculate the velocity of the particle in the tangent direction
        vector2 vel_tangent = vel - normal * vel_normal;

        // Calculate the new velocity of the particle
        vel = vel_tangent - normal * vel_normal * manager.WALL_DAMP;

        // Move the particle out of the wall
        pos = collision.contacts[0].point + normal * manager.WALL_POS;
    }

}
