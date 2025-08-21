using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private XRSocketInteractor socketInteractor;

    [SerializeField]
    private GameObject prefabToSpawn;
    private void OnEnable()
    {

        if (socketInteractor != null)
        {
            socketInteractor.selectExited.AddListener(OnSocketSelectExited);
        }
    }

    private void OnDisable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectExited.RemoveListener(OnSocketSelectExited);
        }
    }

    private void Start()
    {
        EnsureSocketFilled();
    }

    private void EnsureSocketFilled()
    {
        if (socketInteractor == null || prefabToSpawn == null)
        {
            Debug.LogWarning("ItemSpawner: Assign both socketInteractor and prefabToSpawn in the Inspector.");
            return;
        }

        if (socketInteractor.hasSelection)
            return;

        SpawnIntoSocket();
    }

    private void OnSocketSelectExited(SelectExitEventArgs args)
    {
        // When the player removes the object from the socket, respawn another one
        // Avoid starting coroutines while disabled/inactive (e.g., during scene unload)
        if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
            return;
        // Do it next frame to avoid selection conflicts, just check that the gameobject is active
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(RespawnNextFrame());
        }
    }

    private System.Collections.IEnumerator RespawnNextFrame()
    {
        yield return null;
        if (this == null || !gameObject.activeInHierarchy)
        {
            yield break;
        }
        SpawnIntoSocket();
    }

    private void SpawnIntoSocket()
    {
        if (socketInteractor == null || prefabToSpawn == null)
            return;
        // Instantiate at the socket's attachTransform (or socket transform if none)
        Transform attach = socketInteractor.attachTransform != null ? socketInteractor.attachTransform : socketInteractor.transform;
        GameObject instance = Instantiate(prefabToSpawn, attach.position, attach.rotation);

        // Ensure instance starts unselected and then force the socket to select it
        var interactionManager = socketInteractor.interactionManager;
        var grabInteractable = instance.GetComponent<IXRSelectInteractable>();
        if (interactionManager != null && grabInteractable != null)
        {
            interactionManager.SelectEnter(socketInteractor, grabInteractable);
        }


    }


}
