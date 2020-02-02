﻿using UnityEngine;

public class FireBulletPresenter : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.75f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
