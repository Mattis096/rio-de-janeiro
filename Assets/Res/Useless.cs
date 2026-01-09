using Unity.VisualScripting;
using UnityEngine;

public class Useless : MonoBehaviour
{
    public float speed;
    
    public void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
}
