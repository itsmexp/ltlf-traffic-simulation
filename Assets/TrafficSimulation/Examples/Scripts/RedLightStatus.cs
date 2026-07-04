// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class RedLightStatus : MonoBehaviour
{

    public int lightGroupId;  // Belong to traffic light 1 or 2?
    public Intersection intersection;
    
    Light pointLight;

    void Start(){
        pointLight = this.transform.GetChild(0).GetComponent<Light>();
        SetTrafficLightColor();
    }

    // Update is called once per frame
    void Update(){
        SetTrafficLightColor();
    }

    void SetTrafficLightColor(){
        // Colori ad alto contrasto per daltonismo
        Color stopColor = new Color(1f, 0.2f, 0.1f);   // Rosso acceso/Vermiglio
        Color goColor = new Color(0.1f, 0.7f, 1f);     // Azzurro/Ciano brillante

        if (intersection.useThinkEngine) {
            if (lightGroupId == 1) {
                pointLight.color = intersection.isEWRed ? stopColor : goColor;
            } else if (lightGroupId == 2) {
                pointLight.color = intersection.isNSRed ? stopColor : goColor;
            }
        } else {
            if(lightGroupId == intersection.currentRedLightsGroup)
                pointLight.color = stopColor;
            else
                pointLight.color = goColor;
        }
    }
}
