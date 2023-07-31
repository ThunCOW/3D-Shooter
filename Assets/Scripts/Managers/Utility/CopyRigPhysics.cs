#if UNITY_EDITOR

using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CopyRigPhysics : MonoBehaviour
{
    public bool RemoveRig;
    [Space]

    public List<Transform> Bones;

    public Transform Target;
    public List<Transform> TargetBones;

    void OnValidate()
    {
        if (RemoveRig)
        {
            RemoveRig = false;

            foreach(Transform bone in Bones)
            {
                
                Destroy(bone.GetComponent<ConfigurableJoint>());
                Destroy(bone.GetComponent<CapsuleCollider>());
                Destroy(bone.GetComponent<Rigidbody>());
            }
        }
        if (Bones.Count == 0 || Bones[0] == null)
        {
            Bones.AddRange(GetComponentsInChildren<Transform>().ToList());
        }

        if (Target != null)
        {
            if (TargetBones.Count == 0 || TargetBones[0] == null)
            {
                TargetBones.AddRange(Target.GetComponentsInChildren<Transform>().ToList());
            }
        }

        Copy();
    }

    public bool CopyPhysics;
    public void Copy()
    {
        if (CopyPhysics)
        {
            CopyPhysics = false;

            for (int i = 0; i < TargetBones.Count; i++ )
                CopyCol(TargetBones[i]);

            for (int i = 0; i < TargetBones.Count; i++)
                CopyRB(TargetBones[i]);

            for (int i = 0; i < TargetBones.Count; i++)
                CopyJoint(TargetBones[i]);
        }
    }

    // It will be copied from Base to Copy gameobject
    private void CopyCol(Transform Base)
    {
        CapsuleCollider BaseCollider = Base.GetComponent<CapsuleCollider>();
        
        if (BaseCollider != null)
        {
            foreach (Transform bone in Bones)
            {
                if (Equals(bone.gameObject.name, Base.gameObject.name) && bone.GetComponent<CapsuleCollider>() == null)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(BaseCollider);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(bone.gameObject);
                    break;
                }
            }
        }
    }

    private void CopyRB(Transform Base)
    {
        Rigidbody BaseRB = Base.GetComponent<Rigidbody>();

        if (BaseRB != null)
        {
            foreach (Transform bone in Bones)
            {
                if (Equals(bone.name, Base.name) && bone.GetComponent<Rigidbody>() == null)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(BaseRB);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(bone.gameObject);
                    break;
                }
            }
        }
    }

    private void CopyJoint(Transform Base)
    {
        ConfigurableJoint BaseCJ = Base.GetComponent<ConfigurableJoint>();

        if (BaseCJ != null)
        {
            foreach (Transform bone in Bones)
            {
                if (Equals(bone.name, Base.name) && bone.GetComponent<ConfigurableJoint>() == null)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(BaseCJ);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(bone.gameObject);
                    
                    ConfigurableJoint CopyCJ = bone.GetComponent<ConfigurableJoint>();
                    foreach (Transform connectedBone in Bones)
                    {
                        //Debug.Log(Equals(BaseCJ.connectedBody.gameObject.name, bone.gameObject.name));
                        if (Equals(BaseCJ.connectedBody.gameObject.name, connectedBone.gameObject.name))
                        {
                            //Debug.Log(BaseCJ.connectedBody.gameObject.name);
                            CopyCJ.connectedBody = connectedBone.GetComponent<Rigidbody>();
                            break;
                        }
                    }
                    break;
                }
            }

        }
    }
}

#endif