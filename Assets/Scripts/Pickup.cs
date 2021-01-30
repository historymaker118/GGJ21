using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pickup : MonoBehaviour
{
    public MovementPart part;

    public GameObject legView;
    public GameObject jumpView;
    public GameObject armview;

    public bool isDirty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnValidate()
    {
        isDirty = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDirty)
        {
            legView.SetActive(false);
            jumpView.SetActive(false);
            armview.SetActive(false);

            switch (part)
            {
                case MovementPart.Legs:
                    legView.SetActive(true);
                    break;
                case MovementPart.Arms:
                    armview.SetActive(true);
                    break;
                case MovementPart.Jump:
                    jumpView.SetActive(true);
                    break;
                case MovementPart.NONE:
                    break;
                default:
                    break;
            }

            isDirty = false;
        }
    }

    public void SetPart(MovementPart part)
    {
        this.part = part;
        isDirty = true;
    }
}
