// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using UnityEngine;
using UnityEditor;

namespace TrafficSimulation {
    [CustomEditor(typeof(Intersection))]
    public class IntersectionEditor : Editor
    {
        private Intersection intersection;

        void OnEnable(){
            intersection = target as Intersection;
        }

        public override void OnInspectorGUI(){
            intersection.intersectionType = (IntersectionType) EditorGUILayout.EnumPopup("Intersection type", intersection.intersectionType);

            EditorGUI.BeginDisabledGroup(intersection.intersectionType != IntersectionType.STOP);

            EditorGUILayout.LabelField("Stop", EditorStyles.boldLabel);
            SerializedProperty sPrioritySegments = serializedObject.FindProperty("prioritySegments");
            EditorGUILayout.PropertyField(sPrioritySegments, new GUIContent("Priority Segments"), true);
            serializedObject.ApplyModifiedProperties();

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(intersection.intersectionType != IntersectionType.TRAFFIC_LIGHT);

            EditorGUILayout.LabelField("Traffic Lights", EditorStyles.boldLabel);
            intersection.lightsDuration = EditorGUILayout.FloatField("Light Duration (in s.)", intersection.lightsDuration);
            intersection.orangeLightDuration = EditorGUILayout.FloatField("Orange Light Duration (in s.)", intersection.orangeLightDuration);
            SerializedProperty sLightsNbr1 = serializedObject.FindProperty("lightsNbr1");
            SerializedProperty sLightsNbr2 = serializedObject.FindProperty("lightsNbr2");
            EditorGUILayout.PropertyField(sLightsNbr1, new GUIContent("Lights #1 (first to be red)"), true);
            EditorGUILayout.PropertyField(sLightsNbr2, new GUIContent("Lights #2"), true);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ThinkEngine Radar", EditorStyles.boldLabel);
            intersection.useThinkEngine = EditorGUILayout.Toggle("Use ThinkEngine", intersection.useThinkEngine);
            
            if (intersection.useThinkEngine) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Startup State", EditorStyles.boldLabel);
                intersection.startNSRed = EditorGUILayout.Toggle("Start NS Red", intersection.startNSRed);
                intersection.startEWRed = EditorGUILayout.Toggle("Start EW Red", intersection.startEWRed);

                EditorGUILayout.Space();
                intersection.detectionRadius = EditorGUILayout.FloatField("Detection Radius", intersection.detectionRadius);
                
                if (GUILayout.Button("Auto-Find Segments")) {
                    intersection.AssignSegmentsAutomatically();
                    EditorUtility.SetDirty(intersection);
                }

                SerializedProperty sSegmentNS = serializedObject.FindProperty("segment_NS");
                SerializedProperty sSegmentSN = serializedObject.FindProperty("segment_SN");
                SerializedProperty sSegmentEW = serializedObject.FindProperty("segment_EW");
                SerializedProperty sSegmentWE = serializedObject.FindProperty("segment_WE");
                EditorGUILayout.PropertyField(sSegmentNS, new GUIContent("Segment NS"), true);
                EditorGUILayout.PropertyField(sSegmentSN, new GUIContent("Segment SN"), true);
                EditorGUILayout.PropertyField(sSegmentEW, new GUIContent("Segment EW"), true);
                EditorGUILayout.PropertyField(sSegmentWE, new GUIContent("Segment WE"), true);
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
