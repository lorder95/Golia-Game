
using UnityEngine;

using Improbable.Unity;

using Improbable.Unity.Visualizer;

using Improbable.Player;



[WorkerType(WorkerPlatform.UnityClient)]

public class PlayerInputSender : MonoBehaviour

{



	[Require] private PlayerInput.Writer PlayerInputWriter;



	void Update ()

	{
		var speedMultiplier = 3;

		var xAxis = Input.GetAxis("Horizontal")* speedMultiplier;

		var yAxis = Input.GetAxis("Vertical")* speedMultiplier;



		var update = new PlayerInput.Update();

		update.SetJoystick(new Joystick(xAxis, yAxis));

		PlayerInputWriter.Send(update);

	}

}