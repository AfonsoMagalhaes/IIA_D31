using System;
using System.Collections;
using UnityEngine;

public class LinearRobotUnitBehaviour : RobotUnit
{
    public float weightResource;
    public float resourceValue;
    public float resouceAngle;

    public float weightWall;
    public float wallValue;
    public float wallAngle;

    // Limiares X
    public float x_superior;
    public float x_inferior;

    // Limiares Y
    public float y_superior;
    public float y_inferior;

    void Update()
    {

        // get sensor data
        wallAngle = blockDetector.GetAngleToClosestObstacle();
        wallValue = weightWall * GetResourceOrBlockValue(blockDetector.GetLinearOuput(), blockDetector.GetLinearOuput());
        
        // apply to the wall
        applyForce(wallAngle + 180, wallValue); // move away

        // get sensor data
        resouceAngle = resourcesDetector.GetAngleToClosestResource();
        resourceValue = weightResource * GetResourceOrBlockValue(resourcesDetector.GetLinearOuput(), resourcesDetector.GetLinearOuput());
        
        // apply to the ball
        applyForce(resouceAngle, resourceValue); // go towards

    }

    public float GetResourceOrBlockValue(float strength, float output)
    {
        if (strength > x_superior || strength < x_inferior || output < y_inferior)
        {
            return y_inferior;
        }
        else if (output > y_superior)
        {
            return y_superior;
        }
        return output;
    }

}