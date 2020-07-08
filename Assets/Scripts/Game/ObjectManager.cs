using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Used to manage a cube object pool for better performance
    /// </summary>
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField] private Cube _cubePrefab;
        private Queue<Cube> _queue = new Queue<Cube>();

        /// <summary>
        /// Initializes the objectpool with n objects
        /// </summary>
        /// <param name="n">The initial size of the objectpool</param>
        public void Init(int n)
        {
            for (int i = 0; i < n; i++)
                _queue.Enqueue(InstantiateCube());

            Cube.OnDisable += OnCubeDisabled;
        }
        /// <summary>
        /// Places a cube at the given position with the given rotation
        /// </summary>
        /// <param name="pos">The world position of the cube</param>
        /// <param name="rot">The world rotation of the cube</param>
        /// <returns>The placed cube</returns>
        public Cube SpawnCube(Vector3 pos, Quaternion rot)
        {
            var cube = (_queue.Count > 0) ? _queue.Dequeue() : InstantiateCube();

            cube.Reuse(pos, rot);

            return cube;
        }
        /// <summary>
        /// Instantiantes a cube object
        /// </summary>
        /// <returns>The instantiated cube</returns>
        private Cube InstantiateCube()
        {
            var cube = Instantiate(_cubePrefab);
            cube.Init(transform);

            return cube;
        }
        /// <summary>
        /// When a cube is disabled it is returned to the pool queue
        /// </summary>
        /// <param name="cube">The disabled cube</param>
        private void OnCubeDisabled(Cube cube)
        {
            _queue.Enqueue(cube);
        }

    }
}
