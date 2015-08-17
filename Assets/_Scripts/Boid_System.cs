using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Boid_System : MonoBehaviour
{

    public List<GameObject> go_boids = new List<GameObject>();
    public GameObject boidpref;
    public GameObject predpref;

    public GameObject CohSlider;
    public GameObject SepSlider;
    public GameObject VelSlider;
    public GameObject Distance;

    public float cohspeed;
    public float seperationspeed;
    public float seperateThresh;
    public float AllignThresh;
    public float Velocity_Limit;



    public float Spawn_Timer;
    public float timer;

    public int Boid_Count;
    public int Boid_Min;
    public int Preditor_Count;


    public float Box_Size = 100;



    public bool center;

    void Update()
    {
        Spawning();
        BoidUpdate();
    }






    public void CohSliderValue()
    {
        cohspeed = CohSlider.GetComponent<Slider>().value;
    }

    public void SepSliderValue()
    {
        seperationspeed = SepSlider.GetComponent<Slider>().value;
    }

    public void VelSliderValue()
    {
        Velocity_Limit = VelSlider.GetComponent<Slider>().value;
    }


    public void DisSliderValue()
    {
        seperateThresh = Distance.GetComponent<Slider>().value;
    }




    void Spawning()
    {
        if (timer < Spawn_Timer)
            timer += Time.deltaTime;

        if (go_boids.Count < Boid_Count + Preditor_Count && timer > Spawn_Timer || go_boids.Count < Boid_Min)
        {

            Vector3 StartOffset = new Vector3((Random.Range(-5f, 5f)), (Random.Range(-5f, 5f)), (Random.Range(-5f, 5f)));
            if (Boid_Count > go_boids.Count || go_boids.Count < Boid_Min) 
            {

                go_boids.Add((GameObject)Instantiate(boidpref, gameObject.GetComponent<Transform>().position + StartOffset, Quaternion.identity));
                timer = 0f;
                go_boids[go_boids.Count - 1].GetComponent<Boid_Stats>().Flock = true;
                go_boids[go_boids.Count - 1].GetComponent<Boid_Stats>().Cohesion = true;
                go_boids[go_boids.Count - 1].GetComponent<Boid_Stats>().Seperation = true;
                go_boids[go_boids.Count - 1].GetComponent<Boid_Stats>().Allign = true;
                go_boids[go_boids.Count - 1].GetComponent<Boid_Stats>().Boxed = true;

            }


        }
    }
    void DeSpawn()
    {
        if (go_boids.Count > Boid_Count + Preditor_Count)
        {
            if (Boid_Count < go_boids.Count - Preditor_Count)
            {
                GameObject.Destroy(go_boids[Boid_Count]);
                go_boids.Remove(go_boids[Boid_Count]);
            }
            if (Preditor_Count < go_boids.Count - Boid_Count)
            {
                GameObject.Destroy(go_boids[(Boid_Count - Preditor_Count)]);
                go_boids.Remove(go_boids[(Boid_Count - Preditor_Count)]);
            }

        }


    }




    /// <summary>
    /// Updates the boids.
    /// Excludes Flock Bool False
    /// </summary>
    void BoidUpdate()
    {
        if (go_boids.Count > 1)
            for (int i = 0; i < go_boids.Count; i++)
            {
                if (go_boids[i].GetComponent<Boid_Stats>().Flock && go_boids.Count > 1)
                {

                    if (go_boids[i].GetComponent<Boid_Stats>().Cohesion)
                        go_boids[i].GetComponent<Boid_Stats>().velocity += Cohesion(i);
                    if (go_boids[i].GetComponent<Boid_Stats>().Seperation)
                        go_boids[i].GetComponent<Boid_Stats>().velocity += Seperation(i);
                    if (go_boids[i].GetComponent<Boid_Stats>().Allign)
                        go_boids[i].GetComponent<Boid_Stats>().velocity += Allignment(i);

                    if (go_boids[i].GetComponent<Boid_Stats>().Boxed)
                        go_boids[i].GetComponent<Boid_Stats>().velocity += Boundry_Rekt(i);
                    if (go_boids[i].GetComponent<Boid_Stats>().Sphered)
                        go_boids[i].GetComponent<Boid_Stats>().velocity += Boundry_Sqhere(i);



                    VelocityLimit(i);


                    go_boids[i].GetComponent<LineRenderer>().SetPosition(0, go_boids[i].GetComponent<Transform>().position);
                    go_boids[i].GetComponent<LineRenderer>().SetPosition(1, go_boids[i].GetComponent<Transform>().position + go_boids[i].GetComponent<Boid_Stats>().velocity * 10);
                    //  go_boids[i]



                }
            }

        AveragingVelocity();
    }
    /// <summary>
    /// Averages Velocity of all boids.
    /// 
    /// Excludes Flock Bool false
    /// </summary>
    void AveragingVelocity()
    {
        Vector3 test = Vector3.zero;
        for (int i = 0; i < go_boids.Count; i++)
            if (go_boids[i].GetComponent<Boid_Stats>().Flock)
                test += go_boids[i].GetComponent<Boid_Stats>().velocity;
    }



    /// <summary>
    /// Finds the percieved center of boids by averaging all other boid's pasition
    /// 
    /// Excludes Current boid;
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    Vector3 PercievedCenter(int current)
    {
        Vector3 pc = Vector3.zero;

        //pc_j=(b1.pos + b2.pos + ... + b_j-1.position + b_j+1.pos + ... + b_n.pos) / (n-1) 
        /* for(i = 0; i < go_boids.lenth; i++)
                pc += go_boid[i];*/

        //Adds all the boids' positions exsept the current
        for (int i = 0; i < go_boids.Count; i++)
            if (i != current)
                pc += go_boids[i].GetComponent<Transform>().position;

        // averages the number of boids that where added together
        pc /= (go_boids.Count - 1);
        return pc;
    }

    /// <summary>
    /// Finds the percieved center but unlike previous ones it creates an offset away from Preditor bool True boids
    /// 
    /// Excludes Current boid.
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    Vector3 AntiPredPercievedCenter(int current)
    {
        Vector3 pc = Vector3.zero;

        for (int i = 0; i < go_boids.Count; i++)
        {
            if (i != current && go_boids[i].GetComponent<Boid_Stats>().Preditor != true && Vector3Mag((gameObject.GetComponent<Transform>().position - go_boids[current].GetComponent<Transform>().position)) > AllignThresh + 2)
                pc += go_boids[i].GetComponent<Transform>().position;

        }
        if (gameObject.GetComponent<Boid_System>().center == true)
        {
            pc += gameObject.GetComponent<Transform>().position;
            pc /= go_boids.Count;
        }
        else
            pc /= (go_boids.Count - 1);

        return pc;
    }

    /// <summary>
    /// Does some math to return a value that would be the boids position when moving twards a mass with a speed modifier passed in.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="mod">Default: 100</param>
    /// <returns>Vector3 intended to be new position after Cohesion cal.</returns>
    Vector3 Cohesion(int current)
    {
        return (AntiPredPercievedCenter(current) - go_boids[current].GetComponent<Transform>().position) * cohspeed / 100;


        /*           * 
        * rule 1 pseudocode:
        *  
        * rule1(boid b_j)
        * 
        *          for each boid b
        *                if b != b_j
        *                     pc_j = pc_j + b.position;
        * 
        *          pc_j = pc_j / (n-1);
        *          
        *          return (pc_j - b_j.position / 100);*/





    }
    /// <summary>
    /// Intended to be used once multiple preditors are made.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="mod"></param>
    /// <returns></returns>
    Vector3 PredCohesion(int current, float mod = 1)
    {
        return (PercievedCenter(current) - go_boids[current].GetComponent<Transform>().position) / 100 * mod;
    }
    /// <summary>
    /// Returns a vector that would be inteded to move away from a boid that is too close.
    /// 
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="spacing">Default: 100 </param>
    /// <returns>Returns the new position</returns>
    Vector3 Seperation(int current)
    {
        /** Rule 2: "Boids try to keep a small distance away from other objects (including other boids)."
            *          "If a boid is within a small distance of another move it as far away again as it already is.
            *          "This is done by subtracting from a vector d (the displacement of each boid wich is near by)."         
            * 
            *      rule 2 pseudocode :
            *          
            *      rule2(boid b_j)
            *          
            *         vector d = 0;
            *         
            *          for each boid b
            *                  if b != b_j
            *                          if |b.position - b_j.position| < 100
            *                              d = d - (b.pos - b_j.pos);
            *                              
            *          return c
            *
         */

        Vector3 seperate = Vector3.zero;
        Vector3 test;
        float testf;
        for (int i = 0; i < go_boids.Count; i++)
            if (i != current)
            {
                test = go_boids[i].GetComponent<Transform>().position - go_boids[current].GetComponent<Transform>().position;
                testf = Mathf.Abs((test.x * test.x) + (test.y * test.y) + (test.z * test.z));
                if (testf < seperateThresh && !go_boids[i].GetComponent<Boid_Stats>().Preditor)
                    seperate -= (go_boids[i].GetComponent<Transform>().position - go_boids[current].GetComponent<Transform>().position) * seperationspeed / 100;
                else if (testf < seperateThresh * 5 && go_boids[i].GetComponent<Boid_Stats>().Preditor)
                    seperate -= ((go_boids[i].GetComponent<Transform>().position - go_boids[current].GetComponent<Transform>().position) * seperationspeed / 100) * .50f;
            }
        return seperate;


    }


    /// <summary>
    /// Takes the velocity of all the Boids around current boid and averages it.
    /// If average is more then current boid's Velocity it returns the average. Else it returns <0,0,0>
    /// 
    /// Excluded from average is current boid, boids with Preditor bool set to true, boids with Allign bool Set to false,
    /// and Flock Bool set to false
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    Vector3 Allignment(int current)
    {
        Vector3 allign = Vector3.zero;
        Vector3 test;
        List<int> list = new List<int>();
        float testf;
        for (int i = 0; i < go_boids.Count; i++)
            if (i != current && !go_boids[i].GetComponent<Boid_Stats>().Preditor && go_boids[i].GetComponent<Boid_Stats>().Allign
                 && go_boids[i].GetComponent<Boid_Stats>().Flock)
            {
                test = go_boids[i].GetComponent<Transform>().position - go_boids[current].GetComponent<Transform>().position;
                testf = Mathf.Abs((test.x * test.x) + (test.y * test.y) + (test.z * test.z));
                if (testf < AllignThresh)
                {
                    allign += go_boids[i].GetComponent<Boid_Stats>().velocity;
                    list.Add(i);
                }

            }
        if (list.Count > 0)
            allign /= list.Count;

        // Mag of allign. checks it to Mag of current boids velocity. If its less then it returns allign if its greater it returns zero?
        if (Vector3.Magnitude(allign) < Vector3.Magnitude(go_boids[current].GetComponent<Boid_Stats>().velocity))
            return allign; //- go_boids[current].GetComponent<Boid_Stats>().CheckVelocity();
        else return Vector3.zero;


    }

    /// <summary>
    /// Limits the velocity based on the float Velocity_Limit
    /// </summary>
    /// <param name="current"></param>
    void VelocityLimit(int current)
    {
        if (Velocity_Limit != 0)
        {
            if (Vector3Mag(go_boids[current].GetComponent<Boid_Stats>().velocity) > Velocity_Limit)
                go_boids[current].GetComponent<Boid_Stats>().velocity = (go_boids[current].GetComponent<Boid_Stats>().velocity.normalized * .15f);
        }

        else
            go_boids[current].GetComponent<Boid_Stats>().velocity = new Vector3(0, 0, 0);
    }

    float Vector3Mag(Vector3 vec)
    {
        return Mathf.Sqrt((vec.x * vec.x) + (vec.y * vec.y) + (vec.z * vec.z));
    }

    /// <summary>
    /// Checks to see how far boid is from center(gameobject this script is attached to) and if its passed a certain point it is cent twards the center based on its distance from it.
    /// 
    /// The shape made is a Sphere
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>

    Vector3 Boundry_Sqhere(int current)
    {
        if (Vector3Mag((gameObject.GetComponent<Transform>().position - go_boids[current].GetComponent<Transform>().position)) > Box_Size)
        {
            return (gameObject.GetComponent<Transform>().position - go_boids[current].GetComponent<Transform>().position) / 100;
        }

        return Vector3.zero;

    }

    /// <summary>
    /// Checks to see if boid is outside of a sertain range(The range creats a Cube around the center of the gameObject this script is attached to)
    /// 
    /// if boid is past the boundry it is sent back in based on distance from "wall"
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    Vector3 Boundry_Rekt(int current)
    {
        ////////////////////////////
        // 1       //  1, 1, 1 max
        //  2      // -1, 1, 1
        //   3     // -1,-1, 1 
        //    4    // -1,-1,-1 min
        //     5   //  1,-1,-1
        //      6  //  1, 1,-1
        //       7 //  1,-1, 1
        //        8// -1, 1,-1

        // x isnt higher or lower then bpos modded by box size
        // y
        // z
        ///////////////////////////
        Vector3 cpos = go_boids[current].GetComponent<Transform>().position;
        Vector3 bpos = gameObject.GetComponent<Transform>().position;
        Vector3 velchan = Vector3.zero;

        if (cpos.x > (bpos.x + Box_Size) || cpos.x < (bpos.x - Box_Size))
        {
            if (cpos.x > (bpos.x + Box_Size))
                velchan.x = ((bpos.x + Box_Size) - cpos.x);

            else
                velchan.x = ((bpos.x - Box_Size) - cpos.x);

        }

        if (cpos.y > (bpos.y + Box_Size) || cpos.y < (bpos.y - Box_Size))
        {
            if (cpos.y > (bpos.y + Box_Size))
                velchan.y = ((bpos.y + Box_Size) - cpos.y);

            else
                velchan.y = ((bpos.y - Box_Size) - cpos.y);

        }
        if (cpos.z > (bpos.z + Box_Size) || cpos.z < (bpos.z - Box_Size))
        {
            if (cpos.z > (bpos.z + Box_Size))
                velchan.z = ((bpos.z + Box_Size) - cpos.z);

            else
                velchan.z = ((bpos.z - Box_Size) - cpos.z);
        }




        return velchan;

    }



}







/*
         
    * 3 vectors( length equal to go.transform.pos)
    * 3 vectors will be what cals how the boids will move. 
         
         
    * End math for finding new position for the boids.
    * // b.velocity = b.velocity + v1+v2+v3
    * // b.posistion = b.position + b.velocity
         
         
        
          
         
    * ><><><><><><><><><><><><><>><<>FINDING THE CENTER OF MASS><><><><><><><><><><><><><><><
    *   b = boid
    *   n = total number of boids
    *  _n = scaler
    *  _j = current or of current 
    * *****************************************************************
   @ * "Center of mass (c) is the average position of all boids."
   @ * 
   @ * c = (b1.pos + b2.pos + .... + b_n.pos + ect) / n 
    * *****************************************************************
   @ * "Perceived Center of Mass (pc) is the center of all other boids."
   @ * 
   @ * pc_j=(b1.pos + b2.pos + ... + b_j-1.position + b_j+1.pos + ... + b_n.pos) / (n-1) 
    * **********************************************************************************
    * 
    *><><><><><><><><><><><><><><>><><>><><RULES><><><><><><><><><><><><><><><><><><><><
    *
    * 
    @* Rule 1(v1): "Boids try to fly towards the centre of mass of neighbouring boids."
    @*          "to move 1% towards c: (pc-j - b_j.pos) / 100"
    @* 
    @* rule 1 pseudocode:
    @*  
    @* rule1(boid b_j)
    @* 
    @*          for each boid b
    @*                if b != b_j
    @*                     pc_j = pc_j + b.position;
    @* 
    @*          pc_j = pc_j / (n-1);
    @*          
    @*          return (pc_j - b_j.position / 100);
    * 
    * 
    * 
    * 
    * 
    * 
   @ * Rule 2: "Boids try to keep a small distance away from other objects (including other boids)."
   @ *          "If a boid is within a small distance of another move it as far away again as it already is.
   @ *          "This is done by subtracting from a vector d (the displacement of each boid wich is near by)."         
   @ * 
   @ *      rule 2 pseudocode :
   @ *          
   @ *      rule2(boid b_j)
   @ *          
   @ *         vector d = 0;
   @ *         
   @ *          for each boid b
   @ *                  if b != b_j
   @ *                          if |b.position - b_j.position| < 100
   @ *                              c = c - (b.pos - b_j.pos);
   @ *                              
   @ *          return c
    *          
    * 
    * 
    * 
    * 
    *  Rule 3: "Boids try to match velocity with near boids."
    *          "Simluar to Rule 1" "We average the velocities" "We calculated a Percieved Velocity (pv)"
    *          "Add small portion to current boids velocity"
    *          
    *  Rule 3 Pseudocode
    *  
    *  rule3(boid b_j)
    *  
    *  vector pv_J
    *  
    *     for each boid b
    *              if b!= b_j
    *                     pv_j = pv_j + b.velocity;
    *                     
    *  ruturn (pv_j - b_j.velocity) / 8;
    */
