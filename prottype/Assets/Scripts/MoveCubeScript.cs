using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WE
{

	public class MoveCubeScript : MonoBehaviour
	{
		private float time = 0.0f;
		public float reverseTime = 3.0f;
		public Vector3 moveSpeed = new Vector3(0, 0, 0);

		private void Start()
		{
			
		}

		private void Update()
		{
			Rigidbody rb = GetComponent<Rigidbody>();
			rb.velocity = new Vector3(moveSpeed.x, moveSpeed.y, moveSpeed.z);
			time += Time.deltaTime;

			//ˆÚ“®•ûŒü‚ð‹t“]
			if (time >= reverseTime)
			{
				time = 0;
				moveSpeed *= -1;
			}
		}
	}

}
