using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlantAI
{
    public class LeafMotor : MonoBehaviour
    {
        public float finalScale = 20;
        private bool isMature = false;

        // Start is called before the first frame update
        void Start()
        {
            finalScale += Random.Range(-3, 1);
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Update is called once per frame
        void Update()
        {
            float factor = 0.01f;
            if (!isMature && transform.localScale.x < finalScale)
            {
                transform.localScale += new Vector3(factor, factor, factor);
            } else
            {
                isMature = true;
            }
        }
    }
}