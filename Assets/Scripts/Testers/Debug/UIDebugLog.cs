using UnityEngine;
using UnityEngine.UI;

public class UIDebugLog : MonoBehaviour
{
    //
    // UIDebugLog
    // structs & classes

    [System.Serializable]
    public class Label
    {
        public string name;
        public string prefix;
        public Text label;

        public void Update(string text)
        {
            if (!label) return;

            label.text = prefix + text;
        }

        public void Clear()
        {
            if (!label) return;

            label.text = prefix;
        }
    }

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // V a r i a b l e s
    //

    [SerializeField]
    private Label[] label;

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // P r o p e r t i e s
    //

    public Label this[string labelName]
    {
        get
        {
            foreach (Label l in label)
                if (l.name.Equals(labelName)) return l;

            return default(Label);
        }
    }

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U n i t y
    //

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U s e r
    //

    public void ClearText(string labelName)
    {
        this[labelName].Clear();
    }
}
