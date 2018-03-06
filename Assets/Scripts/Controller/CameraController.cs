using UnityEngine;

/// <summary>
/// Keep the Camera aimed and focused on the PlayerCharacter.</summary>
[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public Camera Camera;
    public Transform Target;

    public void Update()
    {
        if (Target == null)
        {
            return;
        }
        transform.position = Target.position;
        Camera.transform.LookAt(Target);
    }
}
