using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEntry : MonoBehaviour
{
    //controlling lifetime of a score feed entry
    public float lifetime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,lifetime);
    }


}
