using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Background : MonoBehaviour
{
    //http://a1star.com/star-space-background-7.php
    //http://a1star.com/images/star--background-seamless-repeating9.jpg

    MeshRenderer rend;

    [SerializeField]
    float scroll_limiter = 0.05f;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3((Camera.main.aspect * Camera.main.orthographicSize) * 2, Camera.main.orthographicSize * 2, 0.0f);

        if (SceneManager.GetActiveScene().name == "_spaceship")
        {
            if (GameController.INSTANCE.Current_Game_State == GameState.IN_PLAY)
                rend.material.SetTextureOffset("_MainTex", new Vector2(ShipController.INSTANCE.Ship_Speed * Time.time * scroll_limiter, 0));
        }
        else if (SceneManager.GetActiveScene().name == "_menu")
        {
            rend.material.SetTextureOffset("_MainTex", new Vector2(10.0f * Time.time * scroll_limiter, 0));
        }
    }
}
