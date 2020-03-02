using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDetectorScript : MonoBehaviour
{

    public float angleOfSensors = 10f;
    public float rangeOfSensors = 10f;
    protected Vector3 initialTransformUp;
    protected Vector3 initialTransformFwd;
    public float strength;
    public float angleToClosestObj;
    public int numObjects;
    public bool debugMode;

    // Função Gaussiana
    public float sigma_block = 0.12f;
    public float micro_block = 0.5f;

    // Limiares X
    public float x_block_superior = 0.75f;
    public float x_block_inferior = 0.25f;

    // Limiares Y
    public float y_block_superior = 0.2f;
    public float y_block_inferior = 0.02f;

    // Start is called before the first frame update
    void Start()
    {

        initialTransformUp = this.transform.up;
        initialTransformFwd = this.transform.forward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ObjectInfo wall;
        wall = GetClosestWall();
        if (wall != null)
        {
            angleToClosestObj = wall.angle;
            strength = 1.0f / (wall.distance + 1.0f);
        }
        else
        { // no object detected
            strength = 0;
            angleToClosestObj = 0;
        }

    }

    public float GetAngleToClosestObstacle()
    {
        return angleToClosestObj;
    }

    public float GetLinearOuput()
    {
        if (strength > x_block_superior || strength < x_block_inferior || strength < y_block_inferior)
        {
            Debug.Log("INFERIOR, " + y_block_inferior);
            return y_block_inferior;
        }
        else if (strength > y_block_superior)
        {
            return y_block_superior;
        }

        return strength;
    }

    public virtual float GetGaussianOutput()
    {
        if (strength > x_block_superior || strength < x_block_inferior)
        {
            return y_block_inferior;
        }

        float v1 = strength - micro_block;
        float v2 = 0.5f * ((v1 * v1) / (sigma_block * sigma_block));
        float a = 1.0f / (sigma_block * (float)Math.Sqrt(2 * Math.PI));
        float gaussian = a * (float)Math.Exp(-v2);

        if (gaussian < y_block_inferior)
        {
            return y_block_inferior;
        }
        else if (gaussian > y_block_superior)
        {
            return y_block_superior;
        }

        return gaussian;
    }

    public virtual float GetLogaritmicOutput()
    {
        if (strength > x_block_superior || strength < x_block_inferior)
        {
            return y_block_inferior;
        }

        float log = (float)-Math.Log(strength);

        if (log < y_block_inferior)
        {
            return y_block_inferior;
        }
        else if (log > y_block_superior)
        {
            return y_block_superior;
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
                    if (debugMode)
                    {
                        Debug.DrawRay(this.transform.position, Quaternion.AngleAxis((-angleOfSensors * i), initialTransformUp) * initialTransformFwd * hit.distance, Color.yellow);
                    }
                    ObjectInfo info = new ObjectInfo(hit.distance, angleOfSensors * i + 90);
                    objectsInformation.Add(info);
                }
            }
        }

        objectsInformation.Sort();

        return objectsInformation;
    }

    public ObjectInfo GetClosestWall()
    {
        ObjectInfo[] a = (ObjectInfo[])GetVisibleObjects("Wall").ToArray();
        if (a.Length == 0)
        {
            return null;
        }
        return a[a.Length - 1];
    }

    private void LateUpdate()
    {
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.transform.parent.rotation.z * -1.0f);

    }
}
