using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class CheckEntropy : MonoBehaviour
{
	public List<float> prob = new List<float>()
	{
		0.9f, 0.1f
	};

    // Start is called before the first frame update
    void Start()
    {

	    //Debug.Log(). Extentions.CalculateEntropy(prob);
    }

    [Button]
    void Calc()
    {
		Debug.Log(Extentions.CalculateEntropy(prob));
	}
}
