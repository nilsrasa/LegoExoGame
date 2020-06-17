﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Cube : MonoBehaviour
    {
        public static float speed = .5f;
        public static event System.Action OnCollidedHand;
        public static event System.Action<Cube> OnDisable;

        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void Init(Transform parent)
        {
            transform.SetParent(parent);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(false);
        }

        public void Reuse(Vector3 pos, Quaternion rot)
        {
            gameObject.SetActive(true);
            transform.position = pos;
            transform.rotation = rot;
        }

        public void Disable()
        {
            OnDisable?.Invoke(this);
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //TODO: check

            Disable();
        }
    }
}