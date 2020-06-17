using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField] private Cube _cubePrefab;
        private Queue<Cube> _queue = new Queue<Cube>();


        public void Init(int n)
        {
            for (int i = 0; i < n; i++)
                _queue.Enqueue(InstantiateCube());

            Cube.OnDisable += OnCubeDisabled;
        }

        public void SpawnCube(Vector3 pos, Quaternion rot)
        {
            if (_queue.Count > 0)
                _queue.Dequeue().Reuse(pos, rot);
            else
                InstantiateCube().Reuse(pos, rot);
        }

        private Cube InstantiateCube()
        {
            var cube = Instantiate(_cubePrefab);
            cube.Init(transform);

            return cube;
        }

        private void OnCubeDisabled(Cube cube)
        {
            _queue.Enqueue(cube);
        }

    }
}
