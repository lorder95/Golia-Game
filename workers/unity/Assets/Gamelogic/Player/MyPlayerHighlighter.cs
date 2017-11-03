
using Improbable.Player;

using Improbable.Unity.Visualizer;

using UnityEngine;



public class MyPlayerHighlighter : MonoBehaviour

{



	[Require] private PlayerInput.Writer PlayerInputWriter;



	[SerializeField] private GameObject playerBody;



	private void OnEnable()

	{

		playerBody.GetComponent<Renderer>().material.color = Color.blue;

	}

}