using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public int musicToPlay;
    private bool musicStarted;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void LateUpdate()
    {
        if (!GameSessionManager.Instance.cutSceneMusicActive)
        {
            if (!musicStarted)
            {
                musicStarted = true;
                AudioManager.instance.PlayBGM(musicToPlay);
            }
        }

    }
}