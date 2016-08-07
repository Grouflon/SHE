using UnityEngine;
using System.Collections;

public class SoundKiller : MonoBehaviour {

    private AudioSource source = null;

    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }
	// Update is called once per frame
	void Update () 
    {
        if (!source.isPlaying)
            Destroy(this.gameObject);
	}
}
