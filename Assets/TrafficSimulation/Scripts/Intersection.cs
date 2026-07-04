// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using System.Collections.Generic;
using UnityEngine;

namespace TrafficSimulation{
    public enum IntersectionType{
        STOP,
        TRAFFIC_LIGHT
    }

    public class Intersection : MonoBehaviour
    {   
        public IntersectionType intersectionType;
        public int id;  

        //For stop only
        public List<Segment> prioritySegments;

        //For traffic lights only
        public float lightsDuration = 8;
        public float orangeLightDuration = 2;
        public List<Segment> lightsNbr1;
        public List<Segment> lightsNbr2;

        private List<GameObject> vehiclesQueue;
        private List<GameObject> vehiclesInIntersection;
        private TrafficSystem trafficSystem;
        
        [HideInInspector] public int currentRedLightsGroup = 1;

        public bool useThinkEngine = true;

        void Awake() {
            if (useThinkEngine && intersectionType == IntersectionType.TRAFFIC_LIGHT) {
                isNSRed = startNSRed;
                isEWRed = startEWRed;
            }
        }

        void Start(){
            vehiclesQueue = new List<GameObject>();
            vehiclesInIntersection = new List<GameObject>();
            if(intersectionType == IntersectionType.TRAFFIC_LIGHT && !useThinkEngine)
                InvokeRepeating("SwitchLights", lightsDuration, lightsDuration);

            if (useThinkEngine && intersectionType == IntersectionType.TRAFFIC_LIGHT) {
                AssignSegmentsAutomatically();
                StartCoroutine(TriggerBrainAtStart());
            }
        }

        System.Collections.IEnumerator TriggerBrainAtStart() {
            // Aspettiamo un attimo per essere certi che il TemporalBrain abbia terminato il suo Awake/Start interno
            yield return new WaitForSeconds(0.1f);
            Component brain = GetComponent("ThinkEngine.TemporalBrain");
            if (brain != null) {
                var triggerMethod = brain.GetType().GetMethod("TriggerExecution");
                if (triggerMethod != null) {
                    triggerMethod.Invoke(brain, null);
                }
            }
        }

        public void AssignSegmentsAutomatically() {
            List<Segment> allSegments = new List<Segment>();
            
            if ((lightsNbr1 == null || lightsNbr1.Count == 0) && (lightsNbr2 == null || lightsNbr2.Count == 0)) {
                Segment[] sceneSegments = FindObjectsOfType<Segment>();
                foreach (Segment s in sceneSegments) {
                    if (s.waypoints == null || s.waypoints.Count < 2) continue;
                    Vector3 endPos = s.waypoints[s.waypoints.Count - 1].transform.position;
                    if (Vector3.Distance(endPos, transform.position) < detectionRadius * 2f) {
                        allSegments.Add(s);
                    }
                }
            } else {
                if (lightsNbr1 != null) allSegments.AddRange(lightsNbr1);
                if (lightsNbr2 != null) allSegments.AddRange(lightsNbr2);
            }

            foreach (Segment seg in allSegments) {
                if (seg.waypoints == null || seg.waypoints.Count < 2) continue;
                
                // Usiamo solo l'ultimo tratto della strada per avere la direzione esatta di ingresso
                Vector3 lastPos = seg.waypoints[seg.waypoints.Count - 1].transform.position;
                Vector3 secondLastPos = seg.waypoints[seg.waypoints.Count - 2].transform.position;
                Vector3 dir = (lastPos - secondLastPos).normalized;

                float dotN = Vector3.Dot(dir, Vector3.forward);
                float dotS = Vector3.Dot(dir, Vector3.back);
                float dotE = Vector3.Dot(dir, Vector3.right);
                float dotW = Vector3.Dot(dir, Vector3.left);

                float max = Mathf.Max(dotN, Mathf.Max(dotS, Mathf.Max(dotE, dotW)));

                if (max == dotN) segment_SN = seg; // Puntato verso Nord (viene da Sud)
                else if (max == dotS) segment_NS = seg; // Puntato verso Sud
                else if (max == dotE) segment_WE = seg; // Puntato verso Est
                else if (max == dotW) segment_EW = seg; // Puntato verso Ovest
            }
        }

        void SwitchLights(){

            if(currentRedLightsGroup == 1) currentRedLightsGroup = 2;
            else if(currentRedLightsGroup == 2) currentRedLightsGroup = 1;            
            
            //Wait few seconds after light transition before making the other car move (= orange light)
            Invoke("MoveVehiclesQueue", orangeLightDuration);
        }

        void OnTriggerEnter(Collider _other) {
            //Check if vehicle is already in the list if yes abort
            //Also abort if we just started the scene (if vehicles inside colliders at start)
            if(IsAlreadyInIntersection(_other.gameObject) || Time.timeSinceLevelLoad < .5f) return;

            if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
                TriggerStop(_other.gameObject);
            else if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
                TriggerLight(_other.gameObject);
        }

        void OnTriggerExit(Collider _other) {
            if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
                ExitStop(_other.gameObject);
            else if(_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
                ExitLight(_other.gameObject);
        }

        void TriggerStop(GameObject _vehicle){
            VehicleAI vehicleAI = _vehicle.GetComponent<VehicleAI>();
            
            //Depending on the waypoint threshold, the car can be either on the target segment or on the past segment
            int vehicleSegment = vehicleAI.GetSegmentVehicleIsIn();

            if(!IsPrioritySegment(vehicleSegment)){
                if(vehiclesQueue.Count > 0 || vehiclesInIntersection.Count > 0){
                    vehicleAI.vehicleStatus = Status.STOP;
                    vehiclesQueue.Add(_vehicle);
                }
                else{
                    vehiclesInIntersection.Add(_vehicle);
                    vehicleAI.vehicleStatus = Status.SLOW_DOWN;
                }
            }
            else{
                vehicleAI.vehicleStatus = Status.SLOW_DOWN;
                vehiclesInIntersection.Add(_vehicle);
            }
        }

        void ExitStop(GameObject _vehicle){

            _vehicle.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
            vehiclesInIntersection.Remove(_vehicle);
            vehiclesQueue.Remove(_vehicle);

            if(vehiclesQueue.Count > 0 && vehiclesInIntersection.Count == 0){
                vehiclesQueue[0].GetComponent<VehicleAI>().vehicleStatus = Status.GO;
            }
        }

        void TriggerLight(GameObject _vehicle){
            VehicleAI vehicleAI = _vehicle.GetComponent<VehicleAI>();
            int vehicleSegment = vehicleAI.GetSegmentVehicleIsIn();

            if(IsRedLightSegment(vehicleSegment)){
                vehicleAI.vehicleStatus = Status.STOP;
                vehiclesQueue.Add(_vehicle);
            }
            else{
                vehicleAI.vehicleStatus = Status.GO;
            }
        }

        void ExitLight(GameObject _vehicle){
            _vehicle.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
        }

        bool IsRedLightSegment(int _vehicleSegment){
            if (useThinkEngine) {
                if (isNSRed) {
                    if (segment_NS != null && segment_NS.id == _vehicleSegment) return true;
                    if (segment_SN != null && segment_SN.id == _vehicleSegment) return true;
                }
                if (isEWRed) {
                    if (segment_EW != null && segment_EW.id == _vehicleSegment) return true;
                    if (segment_WE != null && segment_WE.id == _vehicleSegment) return true;
                }
                return false;
            }

            if(currentRedLightsGroup == 1){
                foreach(Segment segment in lightsNbr1){
                    if(segment.id == _vehicleSegment)
                        return true;
                }
            }
            else{
                foreach(Segment segment in lightsNbr2){
                    if(segment.id == _vehicleSegment)
                        return true;
                }
            }
            return false;
        }

        void MoveVehiclesQueue(){
            //Move all vehicles in queue
            List<GameObject> nVehiclesQueue = new List<GameObject>(vehiclesQueue);
            foreach(GameObject vehicle in vehiclesQueue){
                int vehicleSegment = vehicle.GetComponent<VehicleAI>().GetSegmentVehicleIsIn();
                if(!IsRedLightSegment(vehicleSegment)){
                    vehicle.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
                    nVehiclesQueue.Remove(vehicle);
                }
            }
            vehiclesQueue = nVehiclesQueue;
        }

        // --- ThinkEngine Integration ---
        
        [Header("Startup State")]
        public bool startNSRed = false;
        public bool startEWRed = true;

        [Header("ThinkEngine Radar")]
        public float detectionRadius = 15f;
        public Segment segment_NS;
        public Segment segment_SN;
        public Segment segment_EW;
        public Segment segment_WE;

        [HideInInspector] public bool isNSRed = true;
        [HideInInspector] public bool isEWRed = true;

        [HideInInspector] public bool detected_car_NS;
        [HideInInspector] public bool detected_car_SN;
        [HideInInspector] public bool detected_car_EW;
        [HideInInspector] public bool detected_car_WE;

        void Update() {
            if (useThinkEngine && intersectionType == IntersectionType.TRAFFIC_LIGHT) {
                detected_car_NS = false;
                detected_car_SN = false;
                detected_car_EW = false;
                detected_car_WE = false;

                Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
                foreach (Collider col in colliders) {
                    if (col.CompareTag("AutonomousVehicle")) {
                        VehicleAI vehicle = col.GetComponentInParent<VehicleAI>();
                        if (vehicle != null) {
                            int segId = vehicle.GetSegmentVehicleIsIn();
                            if (segment_NS != null && segId == segment_NS.id) detected_car_NS = true;
                            if (segment_SN != null && segId == segment_SN.id) detected_car_SN = true;
                            if (segment_EW != null && segId == segment_EW.id) detected_car_EW = true;
                            if (segment_WE != null && segId == segment_WE.id) detected_car_WE = true;
                        }
                    }
                }
            }
        }

        void OnDrawGizmosSelected() {
            if (useThinkEngine && intersectionType == IntersectionType.TRAFFIC_LIGHT) {
                Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
                Gizmos.DrawSphere(transform.position, detectionRadius);
            }
        }

        public void NS_green() {
            if (isNSRed) {
                isNSRed = false;
                Invoke("MoveVehiclesQueue", orangeLightDuration);
            }
        }

        public void NS_red() {
            isNSRed = true;
        }

        public void EW_green() {
            if (isEWRed) {
                isEWRed = false;
                Invoke("MoveVehiclesQueue", orangeLightDuration);
            }
        }

        public void EW_red() {
            isEWRed = true;
        }

        // -------------------------------

        bool IsPrioritySegment(int _vehicleSegment){
            foreach(Segment s in prioritySegments){
                if(_vehicleSegment == s.id)
                    return true;
            }
            return false;
        }

        bool IsAlreadyInIntersection(GameObject _target){
            foreach(GameObject vehicle in vehiclesInIntersection){
                if(vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
            }
            foreach(GameObject vehicle in vehiclesQueue){
                if(vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
            }

            return false;
        } 


        private List<GameObject> memVehiclesQueue = new List<GameObject>();
        private List<GameObject> memVehiclesInIntersection = new List<GameObject>();

        public void SaveIntersectionStatus(){
            memVehiclesQueue = vehiclesQueue;
            memVehiclesInIntersection = vehiclesInIntersection;
        }

        public void ResumeIntersectionStatus(){
            foreach(GameObject v in vehiclesInIntersection){
                foreach(GameObject v2 in memVehiclesInIntersection){
                    if(v.GetInstanceID() == v2.GetInstanceID()){
                        v.GetComponent<VehicleAI>().vehicleStatus = v2.GetComponent<VehicleAI>().vehicleStatus;
                        break;
                    }
                }
            }
            foreach(GameObject v in vehiclesQueue){
                foreach(GameObject v2 in memVehiclesQueue){
                    if(v.GetInstanceID() == v2.GetInstanceID()){
                        v.GetComponent<VehicleAI>().vehicleStatus = v2.GetComponent<VehicleAI>().vehicleStatus;
                        break;
                    }
                }
            }
        }
    }
}
