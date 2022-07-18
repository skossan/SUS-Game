using FishNet.Object;
using UnityEngine;

public class Interactor : NetworkBehaviour
{
    [SerializeField]
    private Transform feeler;
    [SerializeField]
    private float feelerLength = 1f;

    // Ray cast hitinfo from the feeler on the last hit wall
    private IInteractable interactable;
    private Vector3 point;
    private Vector3 normal;

    private const int feelerLayerMask = 1<<3;

    public IInteractable Interactable { get => interactable; }
    public Vector3 Point { get => point; }
    public Vector3 Normal { get => normal; }

    void CheckFeeler()
    {
        // Assume nothing was found
        interactable = null;

        // Check if anything is in front of the feeler
        Ray ray = new(feeler.position, feeler.forward);
        
        Debug.DrawRay(feeler.position, feeler.forward, Color.red);
        
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, GetFeelerLength(), feelerLayerMask))
            return;

        point = hitInfo.point;
        normal = hitInfo.normal;
        interactable = hitInfo.transform.GetComponent<IInteractable>();
    }

    // Factor in the scale of the object
    private float GetFeelerLength()
    {
        return transform.localScale.x * feelerLength;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        CheckFeeler();
    }
    public void Interact()
    {
        if (interactable != null)
            interactable.Interact();
    }
    public IInteractable GetInteractable()
    {
        return interactable;
    }

    void OnDrawGizmosSelected()
    {
        // Show the feeler
        Gizmos.color = Color.red;
        Gizmos.DrawLine(feeler.position, feeler.position + GetFeelerLength() * feeler.forward);

        // Show the normal of the hit object
        if (interactable != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(point, point + GetFeelerLength() * normal);
        }
    }
}
