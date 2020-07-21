using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private GameObject _rightLane, _leftLane, _topLane, _bottomLane;

    public void Init(float width, float depth)
    {
        _rightLane.transform.localPosition = Vector3.right * width;
        _leftLane.transform.localPosition = Vector3.left * width;
        _topLane.transform.localPosition = Vector3.up * width;
        _bottomLane.transform.localPosition = Vector3.down * width;


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
