using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BoltHandler : MonoBehaviour
{

    public Animator animator;
    public Collider interactionZone;
    [SerializeField]
    public Transform boltTransformStartpoint;
    [SerializeField]
    public Transform boltTransformMiddlepoint;
    [SerializeField]
    public RifleHandling rifleHandling;

    public Transform boltTransformEndpoint;
    public float grabvalue;

    public XRBaseInteractor interactor;


    public InputActionReference BoltGrabReference;
    public InputAction BoltGrabValue;
    public InputActionProperty BoltGrabProperty;
    public Vector3 pos;
    public float normalizedProjection;
    public float inputdistance;
    public float inputdistancehorizontal;
    public float length;
    public Vector3 inputvector;
    public Vector3 inputvectorhorizontal;

    public float locationvalue;
    public float rotationvalue;
    public bool isBoltUnlocked;
    public bool isBoltOpen;
    public bool isRoundInChamber;


    void Start()
    {

        //get input:
        BoltGrabValue = BoltGrabReference.action;

        //bolt states:
        isBoltOpen = false;
        isBoltUnlocked = false;
        isRoundInChamber = true;
        //create normalized vector:
        //for rotating bolt
        inputdistance = (boltTransformMiddlepoint.transform.position - boltTransformStartpoint.transform.position).magnitude;
        inputvector = (boltTransformMiddlepoint.transform.position - boltTransformStartpoint.transform.position).normalized;
        length = inputvector.magnitude;

        // for moving 
        inputvectorhorizontal = (boltTransformEndpoint.transform.position - boltTransformMiddlepoint.transform.position).normalized;
        inputdistancehorizontal = inputvectorhorizontal.magnitude;

    }

    void OnEnable()
    {
        BoltGrabValue.Enable();
    }

    void OnDisable()
    {
        BoltGrabValue.Disable();
    }



    private float RotateBolt(Collider other)
    {
        Vector3 controllerPosition = other.transform.position;
        Vector3 startToController = controllerPosition - boltTransformStartpoint.transform.position;
        float projection = Vector3.Dot(startToController, inputvector);
        normalizedProjection = Mathf.Clamp01(projection / inputdistance);


        return normalizedProjection;
    }
    private float MoveBolt(Collider other)
    {
        Vector3 controllerPosition = other.transform.position;
        Vector3 middleToController = controllerPosition - boltTransformMiddlepoint.transform.position;

        Vector3 forwardDirection = boltTransformEndpoint.transform.position - boltTransformMiddlepoint.transform.position;
        forwardDirection.Normalize();

        float projectionh = Vector3.Dot(middleToController, forwardDirection);
        float normalizedProjectionh = Mathf.Clamp01(projectionh / inputdistance);

        return normalizedProjectionh;
    }

    void OnTriggerStay(Collider other)
    {


        grabvalue = BoltGrabValue.ReadValue<float>();
        if ( rotationvalue <= 0.7 && isRoundInChamber ==true)
        {
            rotationvalue = 0f;
            locationvalue = 0f;

        }

        if (grabvalue == 1 && other.CompareTag("Player") == true)
        {

            rotationvalue = RotateBolt(other);
            locationvalue = MoveBolt(other);



            animator.SetFloat("BoltRotation", rotationvalue);
            if (rotationvalue >= 0.8f)
            {
                isBoltUnlocked = true;

            }
            else
            {
                isBoltOpen = false;

                isBoltUnlocked = false;

            }

            if (isBoltUnlocked)
            {
                //round in chamber is true
                animator.SetFloat("BoltPosition", locationvalue);
                if (locationvalue >= 0.8f)
                {
                    isBoltOpen = true;
                    if (rifleHandling.totalAmmoSize > 0)
                        isRoundInChamber = true;
                }

            }
        }


    }


}
