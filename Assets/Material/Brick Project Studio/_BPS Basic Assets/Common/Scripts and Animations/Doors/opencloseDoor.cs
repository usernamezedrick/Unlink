using System.Collections;
using UnityEngine;

namespace SojaExiles
{
	public class opencloseDoor : MonoBehaviour
	{
		public Animator openandclose;
		public bool open;
		public Transform Player;
		public Transform Enemy;

		void Start()
		{
			open = false;
		}

		public IEnumerator ToggleDoor()
		{
			float dist = Vector3.Distance(Player.position, transform.position);
			if (dist < 15)
			{
				if (!open)
				{
					yield return StartCoroutine(opening());
				}
				else
				{
					yield return StartCoroutine(closing());
				}
			}
		}

		private void Update()
		{
			// Optional: Any update logic if needed
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("Enemy"))
			{
				Debug.Log("Enemy collided with the door.");

				if (Enemy != null)  // Ensure Enemy reference is assigned
				{
					float dist = Vector3.Distance(Enemy.position, transform.position);  // Check distance to Enemy

					if (dist < 15)
					{
						if (!open)
						{
							StartCoroutine(opening());
						}
						else
						{
							StartCoroutine(Enemyclosing());
						}
					}
				}
				else
				{
					Debug.LogWarning("Enemy transform reference is missing!");
				}
			}
			
			/*if (collision.gameObject.CompareTag("Player")) 
			{
				if (Player != null)  // Ensure Player reference is assigned
				{
					float dist = Vector3.Distance(Player.position, transform.position);

					if (dist < 15)
					{
						if (!open)
						{
							StartCoroutine(opening());
						}
						else
						{
							StartCoroutine(closing());
						}
					}
				}
			}*/
		}

		void OnMouseOver()
		{
			if (Player != null)  // Ensure Player reference is assigned
			{
				float dist = Vector3.Distance(Player.position, transform.position);

				if (dist < 15)
				{
					if (!open && Input.GetMouseButtonDown(0))
					{
						StartCoroutine(opening());
					}
					else if (open && Input.GetMouseButtonDown(0))
					{
						StartCoroutine(closing());
					}
				}
			}
			else
			{
				Debug.LogWarning("Player transform reference is missing!");
			}
		}

		IEnumerator opening()
		{
			open = true;
			Debug.Log("You are opening the door.");
			openandclose.Play("Opening");
			yield return new WaitForSeconds(0.5f);
		}

		IEnumerator closing()
		{
			Debug.Log("You are closing the door.");
			openandclose.Play("Closing");
			open = false;
			yield return new WaitForSeconds(0.5f);
		}

		IEnumerator Enemyclosing()
		{
			Debug.Log("Enemy closed the door.");
			openandclose.Play("Closing");
			open = false;
			yield return new WaitForSeconds(0.4f);
		}
	}
}
