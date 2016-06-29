#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ReplaceName : MonoBehaviour
{
    //
    // ReplaceName
    // structs & classes

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // V a r i a b l e s
    //

    public string target = "(0)";
    public string replaceWith = "(x)";

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // P r o p e r t i e s
    //

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U n i t y
    //

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U s e r
    //

    public void Replace(Transform parent)
    {
        GameObject current = null;

        for(int i = 0; i < parent.childCount; i++)
        {
            Replace(parent.GetChild(i));

            current = parent.GetChild(i).gameObject;

            if(current)
            {
                current.name = current.name.Replace(target, replaceWith);

                EditorUtility.SetDirty(current);
            }
        }
    }
}

[CustomEditor(typeof(ReplaceName))]
public class E_ReplaceName : Editor
{
    ReplaceName I;

    void OnEnable()
    {
        if (!I) I = target as ReplaceName;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!I) return;

        if(GUILayout.Button("Replace"))
        {
            I.Replace(I.transform);
        }
    }
}

#endif