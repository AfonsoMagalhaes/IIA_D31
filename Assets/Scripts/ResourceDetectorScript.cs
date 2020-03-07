using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDetectorScript : MonoBehaviour
{

    public float angleOfSensors = 5f;
    public float rangeOfSensors = 30f;
    protected Vector3 initialTransformUp;
    protected Vector3 initialTransformFwd;
    public float strength;
    public float angle;
    public int numObjects;
    public bool debug_mode;

    // Função Gaussiana
    public float sigma_resource;
    public float micro_resource;

    // Start is called before the first frame update
    void Start()
    {
        initialTransformUp = this.transform.up;
        initialTransformFwd = this.transform.forward;
    }

    // FixedUpdate is called at fixed intervals of time
    void FixedUpdate()
    {
        ObjectInfo pickup;
        pickup = GetClosestPickup();
        if (pickup != null)
        {
            angle = pickup.angle;
            strength = 1.0f / (pickup.distance + 1.0f);
        }
        else
        {
            strength = 0;
            angle = 0;
        }
        
    }

    public float GetAngleToClosestResource()
    {
        return angle;
    }

    public float GetLinearOuput()
    {
        return strength;
    }

    public virtual float GetGaussianOutput()
    {
        float v1 = strength - micro_resource;
        float v2 = 0.5f * ((v1 * v1) / (sigma_resource * sigma_resource));
        float a = 1.0f / (sigma_resource * (float)Math.Sqrt(2 * Math.PI));
        float gaussian = a * (float)Math.Exp(-v2);
        return gaussian;
    }

    public virtual float GetLogaritmicOutput()
    {
        float log = (float)-Math.Log(strength);
        return log;
    }



    public List<ObjectInfo> GetVisibleObjects(string objectTag)
    {
        RaycastHit hit;
        List<ObjectInfo> objectsInformation = new List<ObjectInfo>();

        for (int i = 0; i * angleOfSensors <= 360f; i++)
        {
            if (Physics.Raycast(this.transform.position, Quaternion.AngleAxis(-angleOfSensors * i, initialTransformUp) * initialTransformFwd, out hit, rangeOfSensors))
            {

                if (hit.transform.gameObject.CompareTag(objectTag))
                {
                    if (debug_mode)
                    {
                        Debug.DrawRay(this.transform.position, Quaternion.AngleAxis((-angleOfSensors * i), initialTransformUp) * initialTransformFwd * hit.distance, Color.red);
                    }
                    ObjectInfo info = new ObjectInfo(hit.distance, angleOfSensors * i + 90);
                    objectsInformation.Add(info);
                }
            }
        }

        objectsInformation.Sort();

        return objectsInformation;
    }

    public ObjectInfo[] GetVisiblePickups()
    {
        return (ObjectInfo[]) GetVisibleObjects("Pickup").ToArray();
    }

    public ObjectInfo GetClosestPickup()
    {
        ObjectInfo [] a = (ObjectInfo[])GetVisibleObjects("Pickup").ToArray();
        if(a.Length == 0)
        {
            return null;
        }
        return a[a.Length-1];
    }


    private void LateUpdate()
    {
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.transform.parent.rotation.z * -1.0f);
    }
}