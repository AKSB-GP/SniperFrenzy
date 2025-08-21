using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MagazineBehaviour : MonoBehaviour
{
    public int capacity = 3; // rounds in mag
    public int currentAmmo;  // rounds remaining
    public MeshFilter projectileMesh;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {

        grabInteractable = GetComponent<XRGrabInteractable>();
        currentAmmo = capacity;
        grabInteractable.selectExited.AddListener(OnReleased);

    }
    public void ConsumeRound()
    {
        currentAmmo = Mathf.Max(currentAmmo - 1, 0);

    }
    public void OnReleased(SelectExitEventArgs args)
    {
        if (currentAmmo == 0)
        {
            projectileMesh.mesh = null;
        }
    }
   

    void OnDestroy()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
