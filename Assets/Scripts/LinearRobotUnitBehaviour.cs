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

    void Update()
    {

        // get sensor data
        resouceAngle = resourcesDetector.GetAngleToClosestResource();

        resourceValue = weightResource * resourcesDetector.GetLogaritmicOutput();

        // get sensor data
        wallAngle = blockDetector.GetAngleToClosestObstacle();

        wallValue = weightWall * blockDetector.GetLinearOuput();

        if(wallValue > resourceValue)
        {
            // apply to the wall
            applyForce(wallAngle+180, wallValue); // move away
        }
        else
        {
            // apply to the ball
            applyForce(resouceAngle, resourceValue); // go towards
        }

    }

}