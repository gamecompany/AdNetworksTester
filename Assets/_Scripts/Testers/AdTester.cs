using UnityEngine;
using UnityEngine.UI;

public class AdTester : MonoBehaviour
{
    //
    // UIDebugLog
    // structs & classes

    // I had to make a singleton because most of the adNetworks are not Mono
    private static AdTester instance;

    [System.Serializable]
    public class Label
    {
        public string name;
        public Text label;

        public void Update(string text)
        {
            if (!label) return;

            label.text = text;
        }

        public void Clear()
        {
            if (!label) return;

            label.text = "";
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

    public static AdTester I
    {
        get { return instance; }
    }

    public static bool Exists
    {
        get { return instance; }
    }

    public Label this[string labelName]
    {
        get
        {
            labelName = labelName.ToLower();
            labelName = labelName.Replace(" ", "");
            labelName = labelName.Replace("_", "");

            string currentLabelName = "";

            foreach (Label l in label)
            {
                currentLabelName = l.name.ToLower();
                currentLabelName = currentLabelName.Replace(" ", "");
                currentLabelName = currentLabelName.Replace("_", "");

                Debug.LogFormat("label: {0}, current: {1}", labelName, currentLabelName);

                if (currentLabelName == labelName) return l;
            }

            return default(Label);
        }
    }

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U n i t y
    //

    void Awake()
    {
        instance = this;
    }

    // • • • • • • • • • • • • • • • • • • • • //

    //
    // U s e r
    //

    public void ClearText(string labelName)
    {
        this[labelName].Clear();
    }

    public static string RandomAdPlacement()
    {
        int randomID = Random.Range(0, 2);

        string id = "";
        
        switch (randomID)
        {
            case 0: id = "Ad3MoreGame"; break;
            case 1: id = "Ad4Trailers"; break;
        }

        return id;
    }

    public static string RandomRewardedAdPlacement()
    {
        int randomID = Random.Range(0, 2);

        string id = "";

        switch (randomID)
        {
            case 0: id = "Ad1Launch";  break;
            case 1: id = "Ad5Death"; break;
        }

        return id;
    }
}
