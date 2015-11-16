using UnityEngine;
using System.Collections;

namespace MobileTouchPlugin {
	public class MobileTouchInfo {
		/// <summary>
		/// Finger ID.
		/// </summary>
		/// <value>The only index to the touch.</value>
		public int fingerId { get; set; }

		/// <summary>
		/// Current Position.
		/// </summary>
		/// <value>Position of the touched screen.</value>
		public Vector2 position { get; set; }

		/// <summary>
		/// Delta Position.
		/// </summary>
		/// <value>Position of the screen has changed from the previous frame.</value>
		public Vector2 deltaPosition { get; set; }

		/// <summary>
		/// Delta Time.
		/// </summary>
		/// <value>The total time that has elapsed since the last state change.</value>
		public float deltaTime { get; set; }

		/// <summary>
		/// Phase.
		/// </summary>
		/// <value>The phase.</value>
		public TouchPhase phase { get; set; }

		/// <summary>
		/// Contact Area.
		/// </summary>
		/// <value>Area that touched the finger and the screen.</value>
		public float contactArea { get; set; }

		/// <summary>
		/// Pressure.
		/// </summary>
		/// <value>The strength of the finger is pressing the screen.</value>
		public float force { get; set; }
	}

}