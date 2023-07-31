using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    public bool RagdollActive;

    private Rigidbody rb;
    private Animator enemyAnimator;

    [Header("________Ragdoll Var________")]
    public GameObject RagdollRig;
    [Space]

    public bool GetRagdollVars;
    public List<Collider> RagdollColliderList;
    public List<Rigidbody> RagdollRigidBodyList;
    [Space]
    public string BoneRootName, BoneHeadName;
    public Rigidbody RagdollRootRb;
    public Rigidbody RagdollHeadRb;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (GetRagdollVars)
        {
            GetRagdollVars = false;
            
            RagdollRig = GetComponentInChildren<CopyRigPhysics>().gameObject;

            RagdollRigidBodyList = transform.GetChild(0).GetComponentsInChildren<Rigidbody>().ToList();
            RagdollColliderList = transform.GetChild(0).GetComponentsInChildren<Collider>().ToList();

            foreach (Rigidbody rb in RagdollRigidBodyList)
            {
                if (BoneRootName == rb.name)
                    RagdollRootRb = rb;
                else if (BoneHeadName == rb.name)
                    RagdollHeadRb = rb;
            }
        }
#endif
    }

    void Awake()
    {
        enemyAnimator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void Activate(bool active)
    {
        if (RagdollActive)
        {
            foreach (var col in RagdollColliderList)
                col.enabled = active;
            foreach (var rb in RagdollRigidBodyList)
            {
                rb.detectCollisions = active;
                rb.isKinematic = !active;
            }

            enemyAnimator.enabled = !active;
            if (rb != null)
            {
                rb.detectCollisions = !active;
                rb.isKinematic = active;
            }
        }
    }

    public void RagdollDeath(Vector3 forceDir)
    {
        if (RagdollActive)
        {
            RagdollRig.SetActive(true);
            Activate(true);

            RagdollHeadRb.detectCollisions = true;
            RagdollHeadRb.isKinematic = false;
            RagdollHeadRb.AddForce(forceDir * 50, ForceMode.Impulse);
            //RagdollRootRb.AddTorque(Vector3.left * 100000);
        }
    }

    private void OnEnable()
    {
        Activate(false);
        RagdollRig.SetActive(false);

        if (!RagdollActive)
        {
            foreach (Rigidbody rig in RagdollRigidBodyList)
            {
                //Destroy(rig.GetComponent<ConfigurableJoint>());
                //Destroy(rig.GetComponent<CapsuleCollider>());
                //Destroy(rig);
            }
        }
    }
}
