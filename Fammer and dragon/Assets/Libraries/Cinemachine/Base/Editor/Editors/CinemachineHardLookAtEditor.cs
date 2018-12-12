using UnityEditor;
using UnityEngine;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineHaFDLookAt))]
    public sealed class CinemachineHaFDLookAtEditor : BaseEditor<CinemachineHaFDLookAt>
    {
        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.LookAtTarget == null)
                EditorGUILayout.HelpBox(
                    "HaFD Look At requires a LookAt target.  Change Aim to Do Nothing if you don't want a LookAt target.", 
                    MessageType.Warning);
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUILayout.LabelField(" ", "HaFD Look At has no settings", EditorStyles.miniLabel);
            GUI.enabled = true;
            DrawRemainingPropertiesInInspector();
        }
    }
}
