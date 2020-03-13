using System;
using System.Collections;
using UnityEngine;

public class LinearRobotUnitBehaviour : RobotUnit
{
    public enum ActivationType { None, Linear, Gaussian, LogaritmNegative};

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

    //funcao de output
    public ActivationType activationResources;
    public ActivationType activationBlocks;

    void Update()
    {

        if( activationResources != ActivationType.None)
        {
            // get sensor data
            resouceAngle = resourcesDetector.GetAngleToClosestResource();

            // testes de limiares de x
            if (resourcesDetector.strength > x_superior || resourcesDetector.strength < x_inferior)
            {
                resourceValue = 0;
            }
            else
            {
                // receber função de output
                switch (activationResources)
                {
                    case ActivationType.Linear:
                        resourceValue = resourcesDetector.GetLinearOuput();
                        break;
                    case ActivationType.Gaussian:
                        resourceValue = resourcesDetector.GetGaussianOutput();
                        break;
                    case ActivationType.LogaritmNegative:
                        resourceValue = resourcesDetector.GetLogaritmicOutput();
                        break;
                }
            }

            // testar limites de y
            if (resourceValue < y_inferior)
            {
                resourceValue = y_inferior;
            }
            else if(resourceValue > y_superior)
            {
                resourceValue = y_superior;
            }

            // Se weightResource = 0, não se aplica
            if(weightResource > 0)
            {
                resourceValue *= weightResource;
            }

            applyForce(resouceAngle, resourceValue); // go towards
        }

        if (activationBlocks != ActivationType.None)
        {
            // get sensor data
            wallAngle = blockDetector.GetAngleToClosestObstacle();

            // testes de limiares de x
            if (blockDetector.strength > x_superior || blockDetector.strength < x_inferior)
            {
                wallValue = 0;
            }
            else
            {
                // receber função de output
                switch (activationBlocks)
                {
                    case ActivationType.Linear:
                        wallValue = blockDetector.GetLinearOuput();
                        break;
                    case ActivationType.Gaussian:
                        wallValue = blockDetector.GetGaussianOutput();
                        break;
                    case ActivationType.LogaritmNegative:
                        wallValue = blockDetector.GetLogaritmicOutput();
                        break;
                }
            }

            // testar limites de y
            if (wallValue < y_inferior)
            {
                wallValue = y_inferior;
            }
            else if (wallValue > y_superior)
            {
                wallValue = y_superior;
            }

            // Se weightResource = 0, não se aplica
            if (weightWall > 0)
            {
                wallValue *= weightWall;
            }

            applyForce(wallAngle + 180, wallValue); // go towards
        }

    }

}