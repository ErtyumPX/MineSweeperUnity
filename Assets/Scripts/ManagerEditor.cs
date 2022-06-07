using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manager))]
public class ManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Manager generator = (Manager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            generator.Initialize();
        }
    }
}
