using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine;
using System.Collections.Generic;
public class CustomXRGrabInteractable : XRGrabInteractable
{
    [SerializeField] private Transform primaryAttachPoint;

    private Transform primaryInitialAttachPoint;

    [SerializeField] private Transform secondaryAttachPoint;

    private Transform secondaryInitialAttachPoint;

    public GameObject primarydebugSpherePrefab;
    public GameObject secondarydebugSpherePrefab;
    private GameObject primaryDebugSphere;
    private GameObject secondaryDebugSphere;
    public bool UseDebug = false;


    protected override void Awake()
    {
        base.Awake();
        primaryInitialAttachPoint = attachTransform;
        secondaryInitialAttachPoint = secondaryAttachTransform;

        if (UseDebug)
        {
            if (primarydebugSpherePrefab != null || secondaryDebugSphere != null)
            {
                primaryDebugSphere = Instantiate(primarydebugSpherePrefab);
                primaryDebugSphere.name = "PrimaryAttachDebug";
                primaryDebugSphere.transform.localScale = Vector3.one * 0.1f;

                secondaryDebugSphere = Instantiate(secondarydebugSpherePrefab);
                secondaryDebugSphere.name = "SecondaryAttachDebug";
                secondaryDebugSphere.transform.localScale = Vector3.one * 0.1f;
            }
        }
    }
    private void Update()
    {
        if (UseDebug)
        {
            if (primaryDebugSphere != null && attachTransform != null)
            {
                primaryDebugSphere.transform.position = attachTransform.position;
            }

            if (secondaryDebugSphere != null && secondaryAttachTransform != null)
            {
                secondaryDebugSphere.transform.position = secondaryAttachTransform.position;
            }

        }
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        IXRSelectInteractor enteringinteractor = args.interactorObject;
        IXRSelectInteractor remainingInteractor = GetRemainingInteractor(interactorsSelecting, enteringinteractor);
        //upon entering is the attachtransform not at the initial position?
        if (!Mathf.Approximately(Mathf.Abs((attachTransform.transform.position - primaryInitialAttachPoint.transform.position).magnitude), 0f))
        {
            if (remainingInteractor != null)
            {
                interactionManager.SelectExit(remainingInteractor, this);
                attachTransform = primaryInitialAttachPoint;
                secondaryAttachTransform = secondaryInitialAttachPoint;

            }
        }
        base.OnSelectEntering(args);

    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {

        IXRSelectInteractor leavinginteractor = args.interactorObject;
        IXRSelectInteractor remainingInteractor = GetRemainingInteractor(interactorsSelecting, leavinginteractor);
        if (interactorsSelecting.Count > 0)
        {
            //if primary is leaving
            if (IsUsingPrimaryAttach(leavinginteractor))
            {
                if (remainingInteractor != null)
                {
                    interactionManager.SelectEnter(remainingInteractor, this);
                    //if grab is close enough to initialattach, set it as the initialattach
                    if (Mathf.Abs((remainingInteractor.transform.position - primaryInitialAttachPoint.transform.position).magnitude) < 0.25f)
                    {
                        attachTransform = primaryInitialAttachPoint;
                    }
                    else
                    {
                        attachTransform = remainingInteractor.transform;
                    }
                }

                secondaryAttachTransform = secondaryInitialAttachPoint;
            }
            //if secondary is leaving, default state
            if (IsUsingSecondaryAttach(leavinginteractor))
            {
                attachTransform = primaryInitialAttachPoint;
                secondaryAttachTransform = secondaryInitialAttachPoint;

            }
        }
        base.OnSelectExiting(args);
        //if no interactors,  reset to default 
        if (interactorsSelecting.Count == 0)
        {
            attachTransform = primaryInitialAttachPoint;
            secondaryAttachTransform = secondaryInitialAttachPoint;
        }



    }

    //helper functions to get attaches and interactors
    private bool IsUsingPrimaryAttach(IXRSelectInteractor interactor)
    {
        return GetAttachTransform(interactor) == primaryAttachPoint;
    }

    private bool IsUsingSecondaryAttach(IXRSelectInteractor interactor)
    {
        return GetAttachTransform(interactor) == secondaryAttachPoint;
    }


    private IXRSelectInteractor GetRemainingInteractor(List<IXRSelectInteractor> interactors, IXRSelectInteractor toExclude)
    {
        return interactors.Find(interactor => interactor != toExclude);
    }



}
