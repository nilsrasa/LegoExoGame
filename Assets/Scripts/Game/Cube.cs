using UnityEngine;

namespace Game
{
    /// <summary>
    /// Game cube, those you want to hit.
    /// </summary>
    public class Cube : MonoBehaviour
    {
        public static float speed = 1f;
        public readonly static int points = 50;
        public readonly static int penalty = 25;
        public static event System.Action OnCollidedHand;
        public static event System.Action OnCollided;
        public static event System.Action<Cube> OnNudgeTrigger;
        public static event System.Action<Cube> OnDisable;

        public new Rigidbody rigidbody;
        private new Renderer renderer;
        private CubeDirection _direction;
        public CubeDirection Direction { 
            get { return _direction; }
            set
            {
                _direction = value;

                //The set direction defines the rotation and color of the cube
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

        /// <summary>
        /// Call this the first time you've instantiated the Cube.
        /// </summary>
        /// <param name="parent">The transform parent to be assigned</param>
        public void Init(Transform parent)
        {
            transform.SetParent(parent);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Call this to reuse the cube with the specified postion and rotation
        /// </summary>
        /// <param name="pos">The world position of the cube</param>
        /// <param name="rot">The world rotation of the cube</param>
        public void Reuse(Vector3 pos, Quaternion rot)
        {
            gameObject.SetActive(true);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            transform.position = pos;
            transform.rotation = rot;
        }
        /// <summary>
        /// Call this to disable the cube
        /// </summary>
        public void Disable()
        {
            OnDisable?.Invoke(this);
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //If we collide with the player, we invoke the OnCollidedHand event.
            if (collision.collider.CompareTag("Player"))
                OnCollidedHand?.Invoke();
            else
                OnCollided?.Invoke();

            //Any collisions disables the cube
            Disable();
        }

        private void OnTriggerEnter(Collider other)
        {
            //If enter a nudge trigger, we invoke the OnNudgeTrigger event.
            if (other.CompareTag("Nudger"))
                OnNudgeTrigger?.Invoke(this);
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
