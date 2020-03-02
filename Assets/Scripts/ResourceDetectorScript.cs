using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDetectorScript : MonoBehaviour
{

    public float angleOfSensors = 10f;
    public float rangeOfSensors = 10f;
    protected Vector3 initialTransformUp;
    protected Vector3 initialTransformFwd;
    public float strength;
    public float angle;
    public int numObjects;
    public bool debug_mode;

    // Função Gaussiana
    public float sigma = 0.25f;
    public float micro = 0.12f;

    // Limiares X
    public float x_superior = 0.75f;
    public float x_inferior = 0.25f;

    // Limiares Y
    public float y_superior = 0.05f;
    public float y_inferior = 0.6f;

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
        if (strength > x_superior || strength < x_inferior || strength < y_inferior)
        {
            return y_inferior;
        }
        else if(strength > y_superior)
        {
            return y_superior;
        }

        return strength;
    }

    public virtual float GetGaussianOutput()
    {
        if (strength > x_superior || strength < x_inferior)
        {
            return 0.0f + y_inferior;
        }

        float v1 = strength - micro;
        float v2 = 0.5f * ((v1 * v1) / (sigma * sigma));
        float a = 1.0f / (sigma * (float)Math.Sqrt(2 * Math.PI));
        float gaussian = a * (float)Math.Exp(-v2);

        if(gaussian < y_inferior)
        {
            return y_inferior;
        }
        else if(gaussian > y_superior)
        {
            return y_superior;
        }
        
        return gaussian;
    }

    public virtual float GetLogaritmicOutput()
    {
        if (strength > x_superior || strength < x_inferior)
        {
            return 0.0f + y_inferior;
        }

        float log = (float)-Math.Log(strength);

        if (log < y_inferior)
        {
            return y_inferior;
        }
        else if (log > y_superior)
        {
            return y_superior;
        }

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