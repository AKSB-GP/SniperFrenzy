using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RifleHandling : MonoBehaviour
{
    [Header("References")]
    public BoltHandler boltHandler;
    public RifleCharacteristics rifleCharacteristics;
    public RifleAudioConfig rifleAudioConfig;
    public XRSocketInteractor magazineSocket;


    [Header("RifleStates")]
    [SerializeField]
    public int totalAmmoSize;
    public bool isRoundInChamber;
    public bool isBoltUnlocked;
    public bool isBoltOpen;

    [Header("GameObjects")]

    public GameObject projectilePrefab;
    public GameObject firingPoint;
    private MagazineBehaviour currentMagazine;

    GameObject Projectile;

    [Header("Input")]
    public InputActionReference FiringReference;
    public InputAction FiringValue;
    public InputActionProperty firingProperty;
    [Header("XR grabbable")]

    [SerializeField]
    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        totalAmmoSize = rifleCharacteristics.totalAmmoSize;

        isRoundInChamber = boltHandler.isRoundInChamber;
        isBoltUnlocked = boltHandler.isBoltUnlocked;
        isBoltOpen = boltHandler.isBoltOpen;
        //subscribe to input:
        FiringValue = FiringReference.action;

    }

    void OnEnable()
    {
        FiringValue.Enable();
    }

    void OnDisable()
    {
        FiringValue.Disable();
    }

    public void Fire()
    {
        if (boltHandler.isRoundInChamber && !boltHandler.isBoltUnlocked && !boltHandler.isBoltOpen && totalAmmoSize > 0)
        {
            Vector3 projectileorigin = firingPoint.transform.position;
            Vector3 projectiledirection = firingPoint.transform.forward;
            //projectile behaviour in seperate script
            audioSource.PlayOneShot(rifleAudioConfig.rifleFire, rifleAudioConfig.volume);

            Projectile = Instantiate(projectilePrefab, projectileorigin, Quaternion.LookRotation(projectiledirection));
            boltHandler.isRoundInChamber = isRoundInChamber = false;
            if (totalAmmoSize < 5 && currentMagazine != null)
            {
                currentMagazine.ConsumeRound();
                if (currentMagazine.currentAmmo == 0 && totalAmmoSize !=0) Destroy(currentMagazine.gameObject);
            }
            totalAmmoSize = Mathf.Clamp(totalAmmoSize - 1, 0, totalAmmoSize);

        }
    }
    public void OnMagazineRemoved(SelectExitEventArgs args)
    {
        currentMagazine = args.interactableObject.transform.GetComponent<MagazineBehaviour>();
        args.interactableObject.transform.SetParent(null, true);
        if (currentMagazine != null || args.interactorObject != null)
        {

            if (boltHandler.isRoundInChamber == true)
            {
                if (totalAmmoSize < 5) currentMagazine.ConsumeRound();
                totalAmmoSize = 1;
            }
            else
            {
                totalAmmoSize = 0;
            }
        }


    }

    public void OnMagazineInserted(SelectEnterEventArgs args)
    {

        currentMagazine = args.interactableObject.transform.GetComponent<MagazineBehaviour>();
        if (currentMagazine != null)
        {
            if (audioSource != null && rifleAudioConfig != null) audioSource.PlayOneShot(rifleAudioConfig.magazineInserted, rifleAudioConfig.volume);
            totalAmmoSize += currentMagazine.currentAmmo;
        }
    }



}
