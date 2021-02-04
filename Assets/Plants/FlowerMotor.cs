using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    public class FlowerMotor : MonoBehaviour
    {

        public float finalScale = 5;

        // Start is called before the first frame update
        void Start()
        {
            finalScale += Random.Range(-2f, 1f);
            transform.localScale = new Vector3(0.1f, 0.1f, 0.05f);
        }


        // Update is called once per frame
        void Update()
        {
            float factor = 0.01f;
            if (transform.localScale.x < finalScale)
            {
                transform.localScale += new Vector3(factor, factor / 2f, factor);
            }
        }
    }
}
