using UnityEditor;
using UnityEngine;

public class PlayerBodyParts : MonoBehaviour
{
    public BodyParts bodyPart;
    public GameObject player;

}
public enum BodyParts
{
    Head,
    Body,
    Arm,
    Leg
}
