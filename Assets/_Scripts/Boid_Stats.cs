using UnityEngine; 
using System.Collections;

public class Boid_Stats : MonoBehaviour
{
    public bool Flock;
    public bool Preditor;     
    
    public bool Cohesion;
    public bool Seperation;
    public bool Allign;
    public bool Boxed;
    public bool Sphered;

    public float Velocity_Rng_Min = -5.0f;
    public float Velocity_Rng_Max = 5.0f;

    public Vector3 velocity;



    // Use this for initialization
    void Start()
    {
        if(Flock)
        velocity = new Vector3(Random.Range(Velocity_Rng_Min, Velocity_Rng_Max), Random.Range(Velocity_Rng_Min, Velocity_Rng_Max), Random.Range(Velocity_Rng_Min, Velocity_Rng_Max));
    }

    // Update is called once per frame

    /// <summary>
    /// Updates the Position and Rotation of the boid based on its Velocity
    /// 
    /// Velocity Dampinor to give less rugged movments
    /// </summary>
    void Update()
    {          
            gameObject.GetComponent<Transform>().position += velocity;

            gameObject.GetComponent<Transform>().forward += new Vector3(velocity.normalized.x + .00001f, velocity.normalized.y + .00001f, velocity.normalized.z + .00001f);
            gameObject.GetComponent<Boid_Stats>().velocity = (gameObject.GetComponent<Boid_Stats>().velocity.normalized * .5f);
        
    }


   
}
