using UnityEngine;
using TMPro;

public class Entity : MonoBehaviour
{
    public TMP_Text nametag;
    public string entityName;
    public int id;
    public string type;

    public Vector3 position;
    public float pitch;
    public float yaw;

    protected virtual void Update()
    {
        if (nametag != null)
        {
            nametag.text = entityName;
        }

        transform.position = new Vector3(-position.x, position.y, position.z);
        transform.rotation = Quaternion.identity;
        transform.Rotate(Vector3.up, -yaw + 180);
        transform.Rotate(Vector3.right, -pitch);
    }
}
