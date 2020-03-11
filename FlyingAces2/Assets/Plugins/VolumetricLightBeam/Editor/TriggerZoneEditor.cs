﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VLB
{
    [CustomEditor(typeof(TriggerZone))]
    [CanEditMultipleObjects]
    public class TriggerZoneEditor : EditorCommon
    {
        SerializedProperty setIsTrigger, rangeMultiplier;
        TargetList<VolumetricLightBeam> m_Targets;

        protected override void OnEnable()
        {
            base.OnEnable();

            setIsTrigger = FindProperty((TriggerZone x) => x.setIsTrigger);
            rangeMultiplier = FindProperty((TriggerZone x) => x.rangeMultiplier);

            m_Targets = new TargetList<VolumetricLightBeam>(targets);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(setIsTrigger, new GUIContent("Set Is Trigger", "Define if the Collider will be created as a convex trigger (not physical, most common behavior) or as a regular collider (physical)."));
            EditorGUILayout.PropertyField(rangeMultiplier, new GUIContent("Range Multiplier", "Change the length of the Collider.\nFor example, set 2.0 to make the Collider 2x longer than the beam."));

            if (HeaderFoldableBegin("Infos"))
            {
                EditorGUILayout.HelpBox("Generate a Collider with the same shape than the beam", MessageType.Info);

                if(m_Targets.HasAtLeastOneTargetWith((VolumetricLightBeam beam) => { return beam.trackChangesDuringPlaytime; }))
                {
                    EditorGUILayout.HelpBox("The TriggerZone collider cannot be changed in realtime.\nIf you animate a property which change the shape of the beam, the collider shape won't fit anymore.", MessageType.Warning);
                }
            }
            HeaderFoldableEnd();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
