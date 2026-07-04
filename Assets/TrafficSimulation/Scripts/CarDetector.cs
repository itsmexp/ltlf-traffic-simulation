using UnityEngine;

namespace TrafficSimulation {
    [RequireComponent(typeof(Collider))]
    public class CarDetector : MonoBehaviour
    {
        public bool hasCar;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("AutonomousVehicle"))
            {
                hasCar = true;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("AutonomousVehicle"))
            {
                hasCar = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("AutonomousVehicle"))
            {
                hasCar = false;
            }
        }
    }
}
