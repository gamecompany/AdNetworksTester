using UnityEngine;
using System.Collections;

/// <summary>
///This is a class to wrap the gui texture button class. It serves as a lazy way to integrate button press logic. Typically this would be done through delegates and independent scripts but this doesn't need to be too complicated for what's being done here. Clarity over complexity.
/// </summary>
public class ChangeSceneButton : GUITextureButton {
  //---------------------------------------------------------------------------
  //private
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  //public
  //---------------------------------------------------------------------------
  public string sceneToChangeTo = "";
  //---------------------------------------------------------------------------

  public override void PerformButtonPressLogic() {
    Application.LoadLevel(sceneToChangeTo);
  }
}
