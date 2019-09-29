using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TrisAndVertsCalculate : MonoBehaviour {
    private float _timer;
    private Text _showText;
	// Use this for initialization
	void Start () {
        _showText = transform.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        _timer += Time.deltaTime;
        if (_timer >1)
        {
            _timer = 0;
            CalculateFream();
        }
	}

    private void CalculateFream()
    {
        long tris = 0;
        long verts = 0;
        GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < ob.Length; i++)
        {
            if (ob[i].GetComponent<MeshFilter>())
            {
                if (ob[i].GetComponent<MeshRenderer>())
                {
                    if (ob[i].GetComponent<MeshRenderer>().isVisible)
                    {
                        MeshFilter f = ob[i].GetComponent<MeshFilter>();
                        tris += f.sharedMesh.triangles.Length / 3;
                        verts += f.sharedMesh.vertexCount;
                    }
                }             
            }          
        }
        _showText.text = "三角面数:" + tris + "\n" + "顶点数" + verts + "\n";
    }
}
