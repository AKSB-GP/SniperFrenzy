using UnityEngine;

public class UIBoltPrompt : MonoBehaviour
{
    [Header("References")]
    public BoltHandler boltHandler;
    public RifleHandling rifleHandling;

    [Header("UI meshes")]
    public GameObject boltClosedMesh;
    public GameObject boltOpenMesh;
    public GameObject boltUnlockedMesh;

    [Header("UI Material")]
    public Material promptMaterial;

    private void Start()
    {
        // Apply material to all prompt meshes
        ApplyTransparentMaterial(boltClosedMesh);
        ApplyTransparentMaterial(boltOpenMesh);
        ApplyTransparentMaterial(boltUnlockedMesh);

        // Hide all at start
        HideAllMeshes();
    }

    private void Update()
    {
        if (boltHandler == null) return;

        HideAllMeshes();

        if (rifleHandling.totalAmmoSize > 0)
        {
            //closed and round not loaded
            if (!boltHandler.isRoundInChamber && !boltHandler.isBoltUnlocked && !boltHandler.isBoltOpen)
            {

                if (boltHandler.rotationvalue < 0.8f)
                {
                    ShowMesh(boltUnlockedMesh);
                }

            }
            //has loaded round?
            if (boltHandler.rotationvalue > 0.8f)
            {
                if (!boltHandler.isRoundInChamber && boltHandler.isBoltUnlocked) ShowMesh(boltOpenMesh);
                else if (boltHandler.isRoundInChamber && boltHandler.isBoltOpen) ShowMesh(boltClosedMesh);
            }

        }
    }

    //apply material to the ui meshes
    //can be done in inspector but this is more convienent
    private void ApplyTransparentMaterial(GameObject obj)
    {
        if (obj != null && promptMaterial != null)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.material = promptMaterial;
            }
        }
    }

    private void HideAllMeshes()
    {
        if (boltClosedMesh != null) boltClosedMesh.SetActive(false);
        if (boltOpenMesh != null) boltOpenMesh.SetActive(false);
        if (boltUnlockedMesh != null) boltUnlockedMesh.SetActive(false);
    }

    private void ShowMesh(GameObject meshObj)
    {
        if (meshObj != null) meshObj.SetActive(true);
    }
}
