using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraButton : MonoBehaviour
{
    bool fpc = false;
    public GameObject BoidSystem;
    public GameObject ButtonText;
    public void onButton()
    {
       if(!fpc)
       {
           int i = Random.Range(0, BoidSystem.GetComponent<NewBoidsSystem>().go_boids.Count);
           Transform trans = BoidSystem.GetComponent<NewBoidsSystem>().go_boids[i].GetComponent<Transform>();
           gameObject.GetComponent<Transform>().parent = null;
           gameObject.GetComponent<Transform>().parent = trans;
           gameObject.GetComponent<Transform>().rotation = new Quaternion(0, 0, 0, 0);
           gameObject.GetComponent<Transform>().localPosition = new Vector3(0,.5f,.5f);
           ButtonText.GetComponent<Text>().text = "Unfallow Boid";
    
           fpc = true;
           print("SHould be parented");
       }
       else  if(fpc)
       {
           gameObject.GetComponent<Transform>().parent = null;
           gameObject.GetComponent<Transform>().position = new Vector3(0,0,-50);
           gameObject.GetComponent<Transform>().rotation = new Quaternion(0, 0, 0, 0);
           ButtonText.GetComponent<Text>().text = "Fallow Boid";
           fpc = false;
           print("SHould be unparented");
       }
    }




}
