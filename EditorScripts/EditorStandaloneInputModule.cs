using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class can be used in place of StandaloneInputModule, and makes select members publicly accessible.
/// </summary>
public class EditorStandaloneInputModule : StandaloneInputModule
{
	public PointerEventData GetLastPointerEventData()
	{
        #if UNITY_EDITOR

            return GetLastPointerEventData(PointerInputModule.kMouseLeftId);

        #else

            bool pressed, released;
            if (Input.touchCount > 0)
                return GetTouchPointerEventData(Input.GetTouch(0), out pressed, out released);

            return null;
        
        #endif
    }

	public new PointerEventData GetLastPointerEventData(int pointerId)
	{
		return base.GetLastPointerEventData(pointerId);
	}
}