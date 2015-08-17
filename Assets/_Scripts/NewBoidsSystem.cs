using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;




public class NewBoidsSystem : MonoBehaviour
{

    /// <summary>
    /// Prefix Code:
    /// go_ = GameObject;
    /// b_ = Boid; 
    /// p_ = Preditor ;
    /// c_ = Current;
    /// s_ = Speed;
    /// d_ = Distance;
    /// l_ = limiting;
    /// t_ = Timer;
    /// c_ = Current;
    /// r_ = return variable;
    /// 
    /// 
    /// </summary>
    public List<GameObject> go_boids = new List<GameObject>();
    public GameObject b_prefab;

    public GameObject CohSlider;
    public GameObject SepSlider;
    public GameObject VelSlider;
    public GameObject DisSlider;

    public float s_coh;
    public float s_sep;
    public float d_sep;
    public float d_allign;
    public float d_coh;
    public float l_vel;

    public float t_spawn;
    public float t_time;

    public int b_count;
    public int p_count;
    public int b_min;

    public float boundry;


    // Update is called once per frame
    void Update()
    {
        
        Spawning();
        SliderUpdates();
        BoidUpdate();
    }
    void BoidUpdate()
    {
        if (go_boids.Count > 1)
            foreach(GameObject b in go_boids)
                if (b.GetComponent<Boid_Stats>().Flock)
                {

                    if (b.GetComponent<Boid_Stats>().Cohesion)
                        b.GetComponent<Boid_Stats>().velocity += Cohesion(b);
                    if (b.GetComponent<Boid_Stats>().Seperation)
                       b.GetComponent<Boid_Stats>().velocity += Seperation(b);
                    if (b.GetComponent<Boid_Stats>().Allign)
                        b.GetComponent<Boid_Stats>().velocity += Allignment(b);
                    if (b.GetComponent<Boid_Stats>().Boxed)
                       b.GetComponent<Boid_Stats>().velocity += BoundryRekt(b);
                



                    VelocityLimit(b);


                    b.GetComponent<LineRenderer>().SetPosition(0, b.GetComponent<Transform>().position);
                    b.GetComponent<LineRenderer>().SetPosition(1, b.GetComponent<Transform>().position + b.GetComponent<Boid_Stats>().velocity * 10);
                    //  go_boids[i]



                }
      }

    void SliderUpdates()
    {
        s_coh = CohSlider.GetComponent<Slider>().value;
        s_sep = SepSlider.GetComponent<Slider>().value;
        l_vel = VelSlider.GetComponent<Slider>().value;
        d_sep = DisSlider.GetComponent<Slider>().value;
    }



    void Spawning()
    {
        if (t_time < t_spawn)
            t_time += Time.deltaTime;

        if (go_boids.Count < b_count + p_count && t_time > t_spawn || go_boids.Count < b_min)
        {

            Vector3 StartOffset = new Vector3((Random.Range(-5f, 5f)), (Random.Range(-5f, 5f)), (Random.Range(-5f, 5f)));
            if (b_count > go_boids.Count || go_boids.Count < b_min)
            {

                go_boids.Add((GameObject)Instantiate(b_prefab, gameObject.GetComponent<Transform>().position + StartOffset, Quaternion.identity));
                t_time = 0f;


            }


        }
    }

     /// <param name="c_boid"></param>
        /// <returns>Returns a value that is 1% closer the the percieved center of mass of the flock</returns>
    Vector3 Cohesion(GameObject c_boid)
    {
        if (predignorPercievedCenter(c_boid) != Vector3.zero)
            return ((predignorPercievedCenter(c_boid) - c_boid.GetComponent<Transform>().position)) * s_coh / 100;

        else return Vector3.zero;
    }
    /// <param name="c_boid"></param>
    /// <returns>Returns a value that is 1% further away from a boid(s) that c_boid has gotten too close to</returns>
    Vector3 Seperation(GameObject c_boid)
    {
        Vector3 r_vec = Vector3.zero;

        foreach (GameObject b in go_boids)
            if (Mathf.Abs(Vec3Mag(b.GetComponent<Transform>().position - c_boid.GetComponent<Transform>().position)) < d_sep)
                r_vec -= (b.GetComponent<Transform>().position - c_boid.GetComponent<Transform>().position);


        return r_vec * s_sep / 100;

    }
    /// <param name="c_boid"></param>
    /// <returns>Returns a value that is in the same Velocity as the nearby boids</returns>
    Vector3 Allignment(GameObject c_boid)
    {
        Vector3 r_vec = Vector3.zero;

        int n = 0;
        foreach (GameObject b in go_boids)
            if (b != c_boid && !b.GetComponent<Boid_Stats>().Preditor)
                if ((Mathf.Abs(Vec3Mag(b.GetComponent<Transform>().position - c_boid.GetComponent<Transform>().position)) < d_allign))
                {
                    r_vec += b.GetComponent<Boid_Stats>().velocity;
                    n++;
                }

        if (n > 0)
            r_vec /= n;
        else r_vec = new Vector3(0, 0, 0);


        return r_vec;
    }

    /// <summary>
    /// Checks to see if a boid has gotten outside of a set distance from x,y,z points and if it has it returns a corrected path
    /// </summary>
    /// <param name="c_boid"></param>
    /// <returns></returns>

    Vector3 BoundryRekt(GameObject c_boid)
    {

        Vector3 r_vec = Vector3.zero;

        if (c_boid.GetComponent<Transform>().position.x > (gameObject.GetComponent<Transform>().position.x + boundry))
        {
            r_vec.x = (gameObject.GetComponent<Transform>().position.x + boundry) - c_boid.GetComponent<Transform>().position.x;
          
        }

        else if (c_boid.GetComponent<Transform>().position.x < (gameObject.GetComponent<Transform>().position.x - boundry))
        {
            r_vec.x = (gameObject.GetComponent<Transform>().position.x - boundry) - c_boid.GetComponent<Transform>().position.x;
            
        }

        if (c_boid.GetComponent<Transform>().position.y > (gameObject.GetComponent<Transform>().position.y + boundry))
        {
            r_vec.y = (gameObject.GetComponent<Transform>().position.y + boundry) - c_boid.GetComponent<Transform>().position.y;
           
        }

        else if (c_boid.GetComponent<Transform>().position.y < (gameObject.GetComponent<Transform>().position.y - boundry))
        {
            r_vec.y = (gameObject.GetComponent<Transform>().position.y - boundry) - c_boid.GetComponent<Transform>().position.y;
           
        }

        if (c_boid.GetComponent<Transform>().position.z > (gameObject.GetComponent<Transform>().position.z + boundry))
        {
            r_vec.z = (gameObject.GetComponent<Transform>().position.z + boundry) - c_boid.GetComponent<Transform>().position.z;
           
        }
        else if (c_boid.GetComponent<Transform>().position.z < (gameObject.GetComponent<Transform>().position.z - boundry))
        {
            r_vec.z = (gameObject.GetComponent<Transform>().position.z - boundry) - c_boid.GetComponent<Transform>().position.z;
        }

        //if ((Mathf.Abs((gameObject.GetComponent<Transform>().position.x - (c_boid.GetComponent<Transform>().position.x))) > boundry))
        //{
        //    r_vec.x = Vec3NormalizeX(gameObject.GetComponent<Transform>().position - c_boid.GetComponent<Transform>().position) * (c_boid.GetComponent<Transform>().position.x - gameObject.GetComponent<Transform>().position.x);
        //    print(r_vec.x);
        //}
        //if ((Mathf.Abs((gameObject.GetComponent<Transform>().position.y - (c_boid.GetComponent<Transform>().position.y))) > boundry))
        //{
        //    r_vec.y = Vec3NormalizeY(gameObject.GetComponent<Transform>().position - c_boid.GetComponent<Transform>().position) * (c_boid.GetComponent<Transform>().position.y - gameObject.GetComponent<Transform>().position.y);
        //    print(r_vec.y);
        //}
        //if ((Mathf.Abs((gameObject.GetComponent<Transform>().position.z - (c_boid.GetComponent<Transform>().position.z))) > boundry))
        //{
        //    r_vec.z = Vec3NormalizeZ(gameObject.GetComponent<Transform>().position - c_boid.GetComponent<Transform>().position) * (c_boid.GetComponent<Transform>().position.z - gameObject.GetComponent<Transform>().position.z);
        //    print(r_vec.z);
        //}


        return r_vec/100;

    }


    /// <summary>
    /// reduces the velocity if it exseds a set value
    /// </summary>
    /// <param name="c_boid"></param>
    void VelocityLimit(GameObject c_boid)
    {
        if (l_vel > 0)
        {
            if (Vec3Mag(c_boid.GetComponent<Boid_Stats>().velocity) > l_vel)
                c_boid.GetComponent<Boid_Stats>().velocity = Vec3Normalize(c_boid.GetComponent<Boid_Stats>().velocity) * (l_vel/10);
        }


        else c_boid.GetComponent<Boid_Stats>().velocity = Vector3.zero;
    }


    /// <summary>
    /// Checks the percieved center without any special factors
    /// 
    /// averages all the boids in the list
    /// 
    /// </summary>
    /// <param name="c_boid"></param>
    /// <returns></returns>
    Vector3 PercievedCenter(GameObject c_boid)
    {
        Vector3 r_vec = Vector3.zero;

        foreach (GameObject b in go_boids)
            if (b != c_boid)
                r_vec += b.GetComponent<Transform>().position;


        return r_vec / (go_boids.Count - 1);
    }
    /// <summary>
    /// Simular to PercievedCenter exsept that it checks for Preditors and ignors them from the averaging of the positions
    /// </summary>
    /// <param name="c_boid"></param>
    /// <returns></returns>
    Vector3 predignorPercievedCenter(GameObject c_boid)
    {
        Vector3 r_vec = Vector3.zero;
        int n = 0;
        foreach (GameObject b in go_boids)
            if (b != c_boid && b.GetComponent<Boid_Stats>().Preditor != true && Mathf.Abs(Vec3Mag(b.GetComponent<Transform>().position - c_boid.GetComponent<Transform>().position)) < d_allign)
            {
                r_vec += b.GetComponent<Transform>().position;
                n++;
            }

        if (n > 0)
            r_vec /= n;
        else r_vec = Vector3.zero;

        return r_vec;
    }


    /// <summary>
    /// gets the magintude of a vector passed in
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public float Vec3Mag(Vector3 vec)
    {
        
        return Mathf.Sqrt((vec.x * vec.x) + (vec.y * vec.y) + (vec.z + vec.x));
    }
    /// <summary>
    /// normalizes a vector
    /// XYZ normals return just the normal of a assosiated function
    /// 
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public Vector3 Vec3Normalize(Vector3 vec)
    {


        if ((Vec3Mag(vec) != 0))
           vec = (vec) / (Vec3Mag(vec));

        else vec = Vector3.zero;

        return vec;
    }
    public float Vec3NormalizeX(Vector3 vec)
    {
        if ((Vec3Mag(vec) != 0))
           vec = (vec) / (Vec3Mag(vec));
        return vec.x;
    }
    public float Vec3NormalizeY(Vector3 vec)
    {
        if ((Vec3Mag(vec) != 0))
           vec = (vec) / (Vec3Mag(vec));
        return vec.y;
    }
    public float Vec3NormalizeZ(Vector3 vec)
    {
        if ((Vec3Mag(vec) != 0))
           vec = (vec) / (Vec3Mag(vec));

        return vec.z;
    }
}
