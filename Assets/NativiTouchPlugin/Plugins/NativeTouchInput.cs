using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NativeTouchInputPlugin {
	public class NativeTouchInput : MonoBehaviour { //メッセージを受け取るために
		private const string CALLBACK_METHOD_NAME = "onMessageCallback";
		private const float a = -565.58f;
		private const float b = 914.63f;
		private const float c = -295.71f;
		private const float d = 51.868f;

		/* For Reference */
		public static TouchInfo[] touches;

		/* For Calculate */
		private List<TouchInfo> beforeTouches;
		private	List<TouchInfo> currentTouches;
		private int beforeTime;
		private int currentTime;
		private float deltaTime;

		/* For Platfform Check */
		private bool other = false;
		private bool android = false;

		/// <summary>
		/// Initialization Method
		/// </summary>
		public void Initialization() {
			touches = new TouchInfo[0];
			beforeTime = 0;
			currentTime = 0;
			deltaTime = 0.0f;
			beforeTouches = new List<TouchInfo> ();
			currentTouches = new List<TouchInfo> ();

			/* Checking Duplicated */
			bool OverlapFlag = true;
			foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) {
				if (obj.name == gameObject.name) {
					if (OverlapFlag) { 
						OverlapFlag = false; 
					} else { 
						Debug.LogError (gameObject.name + " is duplicated. Please don't overlap."); 
						return; 
					}
				}
			}
			/* Initialization Plugin */
			if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
				initAndroidPlugin ();
				#endif
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IOS
				initIOSPlugin();
				#endif
			} else {
				initOtherPlugin ();
			}
		}


		/// <summary>
		/// Message Receive Method
		/// </summary>
		/// <param name="message">Message.</param>
		public void onMessageCallback(String message) {
			List<TouchInfo> targetTouches = new List<TouchInfo> ();
			
			/* 【 message structure 】		 	 	*
		 	 * 	  'type','count','timestamp', ...  	*/

			/* Parse Touch Parameter */
			string[] values = message.Split (',');
			currentTime = int.Parse (values [2]); 
			deltaTime = (float)(currentTime - beforeTime) / 1000.0f;
			int point = 3;
			for (int n = 0, size = int.Parse (values [1]); n < size;n++) {
				TouchInfo touchInfo = new TouchInfo ();
				touchInfo.fingerId = int.Parse(values [point++]);
				touchInfo.position = new Vector2 (float.Parse(values [point]), float.Parse(values [point + 1]));
				point += 2;
				touchInfo.contactArea = convertConstantArea (float.Parse(values [point++]));
				touchInfo.force = float.Parse(values [point++]);
				targetTouches.Add (touchInfo);
			}
			/* Switch Action by Message Type */
			switch (values [0]) {
			case "began":
				registerTouches (targetTouches);
				break;
			case "ended":
				unregisterTouches (targetTouches, TouchPhase.Ended);
				break;
			case "moved":
				updateTouches (targetTouches);
				break;
			case "cancelled":
				unregisterTouches (targetTouches, TouchPhase.Canceled);
				break;
			}
		}

		/// <summary>
		/// Convert ContactArea to iOS
		/// </summary>
		/// <returns>The constant area.</returns>
		/// <param name="convertArea">Convert area.</param>
		private float convertConstantArea(float contactArea) {
			if (android) {
				Debug.Log (contactArea);
				float result = (contactArea * contactArea * contactArea * a) 
				               + (contactArea * contactArea * b) + (contactArea * c) + d;
				return result;
			}
			return contactArea;
		}

		/// <summary>
		/// Registers the touches.
		/// </summary>
		private void registerTouches(List<TouchInfo> targetTouches) {
			for (int n = 0; n < targetTouches.Count; n++) {
				targetTouches [n].deltaPosition = Vector2.zero;
				targetTouches [n].deltaTime = 0.0f;
				targetTouches [n].phase = TouchPhase.Began;
			}
			currentTouches.AddRange (targetTouches);
		}

		/// <summary>
		/// Unregisters the touches.
		/// </summary>
		private void unregisterTouches(List<TouchInfo> targetTouches, TouchPhase phase) {
			foreach (var target in targetTouches) {
				for (int n = 0; n < currentTouches.Count; n++) {
					if (target.fingerId == currentTouches [n].fingerId) {
						if (currentTouches [n].phase == TouchPhase.Began) {
							currentTouches.RemoveAt (n);
						} else {
							currentTouches [n].phase = phase;
							currentTouches [n].deltaPosition = currentTouches [n].position - target.position; 
							currentTouches [n].position = target.position;
							currentTouches [n].contactArea = target.contactArea;
							currentTouches [n].force = target.force;
							currentTouches [n].deltaTime = deltaTime;
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Updates the touches.
		/// </summary>
		private void updateTouches(List<TouchInfo> targetTouches) {
			foreach (var target in targetTouches) {
				for (int n = 0; n < currentTouches.Count; n++) {
					if (target.fingerId == currentTouches [n].fingerId && currentTouches [n].phase != TouchPhase.Began) {
						for (int m = 0; m < beforeTouches.Count; m++) {
							if (target.fingerId == beforeTouches [n].fingerId) {
								currentTouches [n].position = target.position;
								currentTouches [n].contactArea = target.contactArea;
								currentTouches [n].force = target.force;
								currentTouches [n].deltaTime = deltaTime;
							}
						}
					}
				}
			}
		}

			
		/// ***************
		/// Platform Action
		/// ***************
		/* Target iOS */
		#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern void initPlugin(String objectName, String callbackMethod);
		private void initIOSPlugin() {
			initPlugin (gameObject.name, CALLBACK_METHOD_NAME);
		}
		#endif

		/* Target Android */
		#if UNITY_ANDROID
		private string JAVA_CLASS_NAME = "info.touchplugin.lib.TouchPlugin";
		private void initAndroidPlugin() {
			android = true;
			using (AndroidJavaClass plugin = new AndroidJavaClass(JAVA_CLASS_NAME)) {
				plugin.CallStatic("initPlugin", gameObject.name, CALLBACK_METHOD_NAME);
			}
		}
		#endif

		/* Target Other */
		private void initOtherPlugin() {
			other = true;
		}


		/// ***************
		/// Standard Method
		/// ***************
		void Start() { Initialization (); }

		void Update() {
			if (other) {
				OtherUpdate ();
			} else {
				if (currentTouches.Count > 0) {
					beforeTouches = new List<TouchInfo> (currentTouches);
					touches = currentTouches.ToArray ();
					beforeTime = currentTime;
					for (int n = 0; n < currentTouches.Count; n++) {
						if (currentTouches [n].phase == TouchPhase.Began) {
							currentTouches [n].phase = TouchPhase.Moved;
						} else if (currentTouches [n].phase == TouchPhase.Ended
						          || currentTouches [n].phase == TouchPhase.Canceled) {
							currentTouches.Remove (currentTouches [n]);
						} else if (currentTouches [n].phase == TouchPhase.Moved) {
							// TouchPhase.Stationary への変化処理を書く予定
					
						} else if (currentTouches [n].phase == TouchPhase.Stationary) {
					
							// TouchPhase.Moved への変化処理を書く予定
						}
					}
				} else {
					beforeTime = 0;
					currentTime = 0;
					deltaTime = 0.0f;
					beforeTouches.Clear ();
					currentTouches.Clear ();
				}
			}
		}
	
		/// <summary>
		/// Update by Other Platform.
		/// </summary>
		private void OtherUpdate() {
			if (IsTouchPlatform ()) {
				currentTouches.Clear ();
				foreach (var touch in Input.touches) {
					TouchInfo touchInfo = new TouchInfo ();
					touchInfo.fingerId = touch.fingerId;
					touchInfo.position = touch.position;
					touchInfo.phase = touch.phase;
					touchInfo.deltaPosition = touch.deltaPosition;
					touchInfo.deltaTime = touch.deltaTime;
					touchInfo.contactArea = 0.0f;
					touchInfo.force = 0.0f;
					currentTouches.Add (touchInfo);
				}
			} else {
				if (Input.GetMouseButton (0)) {
					TouchInfo touchInfo = new TouchInfo ();
					touchInfo.fingerId = 0;
					touchInfo.position = Input.mousePosition;
					touchInfo.phase = currentTouches.Count == 0 ? TouchPhase.Began : TouchPhase.Moved;
					touchInfo.deltaTime = Time.deltaTime;
					touchInfo.deltaPosition = currentTouches.Count == 0 ? Vector2.zero : touchInfo.position - currentTouches [0].position;
					touchInfo.contactArea = 0.0f;
					touchInfo.force = 0.0f;
					currentTouches.Clear ();
					currentTouches.Add (touchInfo);
				} else if(Input.GetMouseButtonUp(0)) {
					for (int n = 0; n < currentTouches.Count; n++) {
						currentTouches [n].deltaPosition = (Vector2)Input.mousePosition - currentTouches [n].position;
						currentTouches [n].position = Input.mousePosition;
						currentTouches [n].phase = TouchPhase.Ended;
						currentTouches [n].deltaTime = Time.deltaTime;
					}
				}
			}
			touches = currentTouches.ToArray ();
		}

		/// <summary>
		/// Whether touchable platform
		/// </summary>
		/// <returns><c>true</c> if this instance is touchable platform; otherwise, <c>false</c>.</returns>
		private bool IsTouchPlatform () {
			if (Application.platform == RuntimePlatform.Android || 
				Application.platform == RuntimePlatform.IPhonePlayer) {
				return true;
			}
			return false;
		}
	
		/*
		void OnGUI() {
			int x = 0;
			int y = 0;

			for (int i = 0; i < touches.Length; i++) {
				TouchInfo info = touches [i];
				GUI.Label (new Rect (x, y, 300, 20), "FingerID = " + info.fingerId.ToString ());
				y += 20;
				GUI.Label (new Rect (x, y, 300, 20), "ScreenPosition = " + info.position.ToString ());
				y += 20;
				GUI.Label (new Rect (x, y, 300, 20), "DeltaPosition = " + info.deltaPosition.ToString ());
				y += 20;
				GUI.Label (new Rect (x, y, 300, 20), "DeltaTime = " + info.deltaTime.ToString ());
				y += 20;
				GUI.Label (new Rect (x, y, 300, 20), "ContactArea = " + info.contactArea.ToString ());
				y += 20;
				GUI.Label (new Rect (x, y, 300, 20), "Pressure = " + info.force.ToString ());
				y += 40;
			}
		}
		*/
	}
}