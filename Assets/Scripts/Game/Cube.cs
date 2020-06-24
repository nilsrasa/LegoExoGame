using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Cube : MonoBehaviour
    {
        public static float speed = 1f;
        public readonly static int points = 50;
        public static event System.Action OnCollidedHand;
        public static event System.Action<CubeDirection> OnNugdeTrigger;
        public static event System.Action<Cube> OnDisable;

        public new Rigidbody rigidbody;
        private new Renderer renderer;
        private CubeDirection _direction;
        public CubeDirection Direction { 
            get { return _direction; }
            set
            {
                _direction = value;

                switch (value)
                {
                    case CubeDirection.Up:
                        transform.localRotation = Quaternion.identity;
                        renderer.material.color = Color.yellow;
                        break;
                    case CubeDirection.Down:
                        transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                        renderer.material.color = Color.blue;
                        break;
                    case CubeDirection.Left:
                        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                        renderer.material.color = Color.red;
                        break;
                    case CubeDirection.Right:
                        transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
                        renderer.material.color = Color.green;
                        break;
                }
            } 
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            renderer = GetComponent<MeshRenderer>();
        }
        void Update()
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
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
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
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
            if (collision.collider.CompareTag("Player"))
                OnCollidedHand?.Invoke();

            Disable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nugder"))
                OnNugdeTrigger?.Invoke(_direction);
        }
    }

    public enum CubeDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
