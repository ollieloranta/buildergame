using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    public Dictionary<string, string> getInformation() {
        return getObjectContents();
    }

    public virtual Dictionary<string, string> getObjectContents() {
        var genericContents = new Dictionary<string, string>();
        genericContents["Name"] = "WorldObject";
        return genericContents;
    }
}