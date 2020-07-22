using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private GameObject _rightLane, _leftLane, _topLane, _bottomLane;

    public void Init(float width, float depth)
    {
        //Placing the lanes
        BuildLane((int)depth, _rightLane, Vector3.right * (width + .5f));
        BuildLane((int)depth, _leftLane, Vector3.left * (width + .5f));
        BuildLane((int)depth, _topLane, Vector3.up * (width + .5f));
        BuildLane((int)depth, _bottomLane, Vector3.down * (width + .5f));

    }

    private void BuildLane(int n, GameObject lane, Vector3 pos)
    {
        var parent = new GameObject(lane.name).transform;
        parent.SetParent(transform);
        parent.localPosition = pos;

        for (int i = 0; i<n; i++)
        {
            var newLane = GameObject.Instantiate(lane, parent);
            newLane.transform.localPosition = Vector3.forward * (i + .5f);
        }

        lane.transform.SetParent(parent);
        lane.transform.localPosition = Vector3.forward * -.5f;
        lane.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
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
