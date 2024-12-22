using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

namespace Terresquall
{

    [System.Serializable]
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class VirtualJoystick : MonoBehaviour
    {

        [Tooltip("The unique ID for this joystick. Needs to be unique.")]
        public int ID;
        [Tooltip("The component that the user will drag around for joystick input.")]
        public Image controlStick;

        [Header("Debug")]
        [Tooltip("Prints to the console the control stick's direction within the joystick.")]
        public bool consolePrintAxis = false;

        public enum InputMode { oldInputManager, newInputSystem };
        public static InputMode inputMode;

        // Function to get an input mode. This is determined by which input system is
        // enabled, or if both input systems are on, on what the user sets.
        public static InputMode GetInputMode()
        {
#if ENABLE_INPUT_SYSTEM
#if ENABLE_LEGACY_INPUT_MANAGER
            return InputMode.oldInputManager;
#else
            EnhancedTouchSupport.Enable();
            return InputMode.newInputSystem;
#endif
#else
            return InputMode.oldInputManager;
#endif
        }

        [Header("Settings")]
        [Tooltip("Disables the joystick if not on a mobile platform.")]
        public bool onlyOnMobile = true;
        [Tooltip("Colour of the control stick while it is being dragged.")]
        public Color dragColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        //[Tooltip("Sets the joystick back to its original position once it is let go of")] public bool snapToOrigin = false;
        [Tooltip("How responsive the control stick is to dragging.")]
        public float sensitivity = 2f;
        [Tooltip("How far you can drag the control stick away from the joystick's centre.")]
        [Range(0, 2)] public float radius = 0.7f;
        [Tooltip("How far must you drag the control stick from the joystick's centre before it registers input")]
        [Range(0, 1)] public float deadzone = 0.3f;

        [Tooltip("Joystick automatically snaps to the edge when outside the deadzone.")]
        public bool edgeSnap;
        [Tooltip("Number of directions of the joystick. " +
            "\nKeep at 0 for a free joystick. " +
            "\nWorks best with multiples of 4")]
        [Range(0, 20)] public int directions = 0;

        [Tooltip("Use this to adjust the angle that the directions are pointed towards.")]
        public float angleOffset = 0;

        [Tooltip("Snaps the joystick to wherever the finger is within a certain boundary.")]
        public bool snapsToTouch = false;
        public Rect boundaries;

        // Private variables.
        internal Vector2 desiredPosition, axis, origin, lastAxis;
        internal Color originalColor; // Stores the original color of the Joystick.
        int currentPointerId = -2;

        internal static readonly Dictionary<int, VirtualJoystick> instances = new Dictionary<int, VirtualJoystick>();

        public const string VERSION = "1.0.6";
        public const string DATE = "26 September 2024";

        Vector2Int lastScreen;
        Canvas canvas;

        // Get an existing instance of a joystick.
        public static VirtualJoystick GetInstance(int id = 0)
        {
            // Display an error if an invalid ID is used.
            if (!instances.ContainsKey(id))
            {
                // If used without any arguments, but no item has an ID of 0,
                // we get the first item in the dictionary.
                if (id == 0)
                {
                    if (instances.Count > 0)
                    {
                        id = instances.Keys.First();
                        Debug.LogWarning($"You are reading Joystick input without specifying an ID, so joystick ID {id} is being used instead.");
                    }
                    else
                    {
                        Debug.LogError("There are no Virtual Joysticks in the Scene!");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"Virtual Joystick ID '{id}' does not exist!");
                    return null;
                }
            }

            // If the code gets here, we can get and return an instance.
            return instances[id];
        }

        // Gets us the number of active joysticks on the screen.
        public static int CountActiveInstances()
        {
            int count = 0;
            foreach (KeyValuePair<int, VirtualJoystick> j in instances)
            {
                if (j.Value.isActiveAndEnabled)
                    count++;
            }
            return count;
        }

        public Vector2 GetAxisDelta() { return GetAxis() - lastAxis; }
        public static Vector2 GetAxisDelta(int id = 0)
        {
            // Show an error if no joysticks are found.
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return Vector2.zero;
            }

            return GetInstance(id).GetAxisDelta();
        }

        public Vector2 GetAxis() { return axis; }

        public float GetAxis(string axe)
        {
            switch (axe.ToLower())
            {
                case "horizontal":
                case "h":
                case "x":
                    return axis.x;
                case "vertical":
                case "v":
                case "y":
                    return axis.y;
            }
            return 0;
        }

        public static float GetAxis(string axe, int id = 0)
        {
            // Show an error if no joysticks are found.
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return 0;
            }

            return GetInstance(id).GetAxis(axe);
        }

        public static Vector2 GetAxis(int id = 0)
        {
            // Show an error if no joysticks are found.
            if (instances.Count <= 0)
            {
                return Vector2.zero;
            }

            return GetInstance(id).axis;
        }

        public Vector2 GetAxisRaw()
        {
            return new Vector2(
                Mathf.Abs(axis.x) < deadzone || Mathf.Approximately(axis.x, 0) ? 0 : Mathf.Sign(axis.x),
                Mathf.Abs(axis.y) < deadzone || Mathf.Approximately(axis.y, 0) ? 0 : Mathf.Sign(axis.y)
            );
        }

        public float GetAxisRaw(string axe)
        {
            float f = GetAxis(axe);
            if (Mathf.Abs(f) < deadzone || Mathf.Approximately(f, 0))
                return 0;
            return Mathf.Sign(f);
        }

        public static float GetAxisRaw(string axe, int id = 0)
        {
            // Show an error if no joysticks are found.
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No active instance of Virtual Joystick found on the Scene.");
                return 0;
            }

            return GetInstance(id).GetAxisRaw(axe);
        }

        public static Vector2 GetAxisRaw(int id = 0)
        {
            // Show an error if no joysticks are found.
            if (instances.Count <= 0)
            {
                Debug.LogWarning("No instances of joysticks found on the Scene.");
                return Vector2.zero;
            }

            return GetInstance(id).GetAxisRaw();
        }

        // Get the radius of this joystick.
        public float GetRadius()
        {
            RectTransform t = transform as RectTransform;
            if (t)
                return radius * t.rect.width * 0.5f;
            return radius;
        }

        // What happens when we press down on the element.
        public void OnPointerDown(PointerEventData data)
        {
            currentPointerId = data.pointerId;
            SetPosition(data.position);
            controlStick.color = dragColor;
        }

        // What happens when we stop pressing down on the element.
        public void OnPointerUp(PointerEventData data)
        {
            desiredPosition = transform.position;
            controlStick.color = originalColor;
            currentPointerId = -2;

            //Snaps the joystick back to its original position
            /*if (snapToOrigin && (Vector2)transform.position != origin) {
                transform.position = origin;
                SetPosition(origin);
            }*/
        }

        protected void SetPosition(Vector2 position)
        {

            // Gets the difference in position between where we want to be,
            // and the center of the joystick.
            Vector2 diff = position - (Vector2)transform.position;

            // Other variables needed for various functionalities.
            float radius = GetRadius();
            bool snapToEdge = edgeSnap && (diff / radius).magnitude > deadzone;

            // If no directions to snap to, joystick moves freely.
            if (directions <= 0)
            {
                // If edge snap is on, it will always snap to the edge when outside of the deadzone.
                if (snapToEdge)
                {
                    desiredPosition = (Vector2)transform.position + diff.normalized * radius;
                }
                else
                {
                    // Clamp the desired position within the radius.
                    desiredPosition = (Vector2)transform.position + Vector2.ClampMagnitude(diff, radius);
                }
            }
            else
            {
                // Calculate nearest snap directional vectors
                Vector2 snapDirection = SnapDirection(diff.normalized, directions, ((360f / directions) + angleOffset) * Mathf.Deg2Rad);

                // Do we snap to the edge outside of the deadzone?
                if (snapToEdge)
                {
                    // Snap to the edge if we are beyond the deadzone.
                    desiredPosition = (Vector2)transform.position + snapDirection * radius;
                }
                else
                {
                    desiredPosition = (Vector2)transform.position + Vector2.ClampMagnitude(snapDirection * diff.magnitude, radius);
                }
            }
        }

        // Calculates nearest directional snap vector to the actual directional vector of the joystick
        private Vector2 SnapDirection(Vector2 vector, int directions, float symmetryAngle)
        {
            //Gets the line of symmetry between 2 snap directions
            Vector2 symmetryLine = new Vector2(Mathf.Cos(symmetryAngle), Mathf.Sin(symmetryAngle));

            //Gets the angle between the joystick dir and the nearest snap dir
            float angle = Vector2.SignedAngle(symmetryLine, vector);

            // Divides the angle by the step size between directions, which is 180f / directions.
            // The result is that the angle is now expressed as a multiple of the step size between directions.
            angle /= 180f / directions;

            // Angle is then rounded to the nearest whole number so that it corresponds to one of the possible directions.
            angle = (angle >= 0f) ? Mathf.Floor(angle) : Mathf.Ceil(angle);

            // Checks if angle is odd
            if ((int)Mathf.Abs(angle) % 2 == 1)
            {
                // Adds or subtracts 1 to ensure that angle is always even.
                angle += (angle >= 0f) ? 1 : -1;
            }

            // Scale angle back to original scale as we divided it too make a multiple before.
            angle *= 180f / directions;
            angle *= Mathf.Deg2Rad;

            // Gets directional vector nearest to the joystick dir with a magnitude of 1.
            // Then multiplies it by the magnitude of the joytick vector.
            Vector2 result = new Vector2(Mathf.Cos(angle + symmetryAngle), Mathf.Sin(angle + symmetryAngle));
            result *= vector.magnitude;
            return result;
        }

        // Loops through children to find an appropriate component to put in.
        void Reset()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                // Once we find an appropriate Image component, abort.
                Image img = transform.GetChild(i).GetComponent<Image>();
                if (img)
                {
                    controlStick = img;
                    break;
                }
            }
        }

        // Function for us to modify the bounds value in future.
        public Rect GetBounds()
        {
            if (!snapsToTouch) return new Rect(0, 0, 0, 0);
            return new Rect(boundaries.x, boundaries.y, Screen.width * boundaries.width, Screen.height * boundaries.height);
        }

        void OnEnable()
        {
            // If we are not on mobile, and this is mobile only, disable.
            if (!Application.isMobilePlatform && onlyOnMobile)
            {
                gameObject.SetActive(false);
                Debug.Log($"Your Virtual Joystick \"{name}\" is disabled because Only On Mobile is checked, and you are not on a mobile platform or mobile emualation.", gameObject);
                return;
            }

            // Gets the Canvas that this joystick is on.
            canvas = GetComponentInParent<Canvas>();
            if (!canvas)
            {
                Debug.LogError(
                    $"Your Virtual Joystick \"{name})\" is not attached to a Canvas, so it won't work. It has been disabled.",
                    gameObject
                );
                enabled = false;
            }

            origin = desiredPosition = transform.position;
            StartCoroutine(Activate());
            originalColor = controlStick.color;

            // Record the screen's attributes so we can detect changes to screen size,
            // such a phone changing orientations.
            lastScreen = new Vector2Int(Screen.width, Screen.height);

            // Add this instance to the List.
            if (!instances.ContainsKey(ID))
                instances.Add(ID, this);
            else
                Debug.LogWarning("You have multiple Virtual Joysticks with the same ID on the Scene! You may not be able to retrieve input from some of them.", this);
        }

        // Added in Version 1.0.2.
        // Resets the position of the joystick again 1 frame after the game starts.
        // This is because the Canvas gets rescaled after the game starts, and this affects
        // how the position is calculated.        
        IEnumerator Activate()
        {
            yield return new WaitForEndOfFrame();
            origin = desiredPosition = transform.position;
        }

        void OnDisable()
        {
            if (instances.ContainsKey(ID))
                instances.Remove(ID);
            else
                Debug.LogWarning("Unable to remove disabled joystick from the global Virtual Joystick list. You may have changed the ID of your joystick on runtime.", this);
        }

        void Update()
        {
            PositionUpdate();
            CheckForDrag();

            // Record the last axis value before we update.
            // For calculating GetAxisDelta().
            lastAxis = axis;

            // Update the position of the joystick.
            controlStick.transform.position = Vector2.MoveTowards(controlStick.transform.position, desiredPosition, sensitivity);

            // If the joystick is moved less than the dead zone amount, it won't register.
            axis = (controlStick.transform.position - transform.position) / GetRadius();
            if (axis.magnitude < deadzone)
                axis = Vector2.zero;

            // If a joystick is toggled and we are debugging, output to console.
            if (axis.sqrMagnitude > 0)
            {
                string output = string.Format("Virtual Joystick ({0}): {1}", name, axis);
                if (consolePrintAxis)
                    Debug.Log(output);
            }
        }

        // Takes the mouse's or finger's position and registers OnPointerDown()
        // if the position hits any part of our Joystick.
        void CheckForInteraction(Vector2 position, int pointerId = -1)
        {
            // Create PointerEventData
            PointerEventData data = new PointerEventData(null);
            data.position = position;
            data.pointerId = pointerId;

            // Perform raycast using GraphicRaycaster attached to the Canvas
            List<RaycastResult> results = new List<RaycastResult>();
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            raycaster.Raycast(data, results);

            // Go through the results, and compare it to 
            foreach (RaycastResult result in results)
            {
                // Check if the hit GameObject is the control stick or one of its children
                if (IsGameObjectOrChild(result.gameObject, gameObject))
                {
                    // Start dragging the joystick
                    OnPointerDown(data);
                    break;
                }
            }
        }

        void CheckForDrag()
        { // Used if using the old Input Manager
            // If the screen has changed, reset the joystick.
            if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
            {
                lastScreen = new Vector2Int(Screen.width, Screen.height);
                OnEnable();
            }

            // If the currentPointerId > -2, we are being dragged.
            if (currentPointerId > -2)
            {
                // If this is more than -1, the Joystick is manipulated by touch.
                if (currentPointerId > -1)
                {

                    // We need to loop through all touches to find the one we want.
                    for (int i = 0; i < GetTouchCount(); i++)
                    {
                        Touch t = GetTouch(i);
                        if (t.fingerId == currentPointerId)
                        {
                            SetPosition(t.position);
                            break;
                        }
                    }

                }
                else
                {
                    // Otherwise, we are being manipulated by the mouse position.
                    SetPosition(GetMousePosition());
                }
            }
        }

        void PositionUpdate()
        { //Used if using the old Input Manager
            // Handle the joystick interaction on Touch.
            int touchCount = GetTouchCount();
            if (touchCount > 0)
            {
                // Also detect touch events too.
                for (int i = 0; i < touchCount; i++)
                {
                    Touch t = GetTouch(i);
                    switch (t.phase)
                    {
                        case Touch.Phase.Began:

                            CheckForInteraction(t.position, t.fingerId);

                            // If currentPointerId < -1, it means this is the first frame we were
                            // clicked on. Check if we need to Uproot().
                            if (currentPointerId < -1)
                            {
                                if (GetBounds().Contains(t.position))
                                {
                                    Uproot(t.position, t.fingerId);
                                    return;
                                }
                            }
                            break;
                        case Touch.Phase.Ended:
                        case Touch.Phase.Canceled:
                            if (currentPointerId == t.fingerId)
                                OnPointerUp(new PointerEventData(null));
                            break;
                    }
                }

            }
            else if (GetMouseButtonDown(0))
            {
                // Checks if our Joystick is being clicked on.
                Vector2 mousePos = GetMousePosition();
                CheckForInteraction(mousePos, -1);

                // If currentPointerId < -1, it means this is the first frame we were
                // clicked on. Check if we need to Uproot().
                if (currentPointerId < -1)
                {
                    if (GetBounds().Contains(mousePos))
                    {
                        Uproot(mousePos);
                    }
                }
            }

            // Trigger OnPointerUp() when we release the button.
            if (GetMouseButtonUp(0) && currentPointerId == -1)
            {
                OnPointerUp(new PointerEventData(null));
            }
        }

        // Roots the joystick to a new position.
        public void Uproot(Vector2 newPos, int newPointerId = -1)
        {
            // Don't move the joystick if we are not tapping too far from it.
            if (Vector2.Distance(transform.position, newPos) < radius)
                return;

            // Otherwise move the virtual joystick to where we clicked.
            transform.position = newPos;
            desiredPosition = transform.position;

            // Artificially trigger the drag event.
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = newPos;
            data.pointerId = newPointerId;
            OnPointerDown(data);
        }

        // Utility method to check if <hitObject> is <target> or its children.
        // Used by CheckForInteraction().
        bool IsGameObjectOrChild(GameObject hitObject, GameObject target)
        {
            if (hitObject == target) return true;

            foreach (Transform child in target.transform)
                if (IsGameObjectOrChild(hitObject, child.gameObject)) return true;

            return false;
        }

        // Get the mouse position. The function automatically adapts
        // depending on the input system used.
        static Vector2 GetMousePosition()
        {
            switch (GetInputMode())
            {
                case InputMode.newInputSystem:
                    return Mouse.current.position.ReadValue();
            }

            // Default to the old input system.
            return Input.mousePosition;
        }

        static bool GetMouseButton(int buttonId)
        {
            switch (GetInputMode())
            {
                case InputMode.newInputSystem:
                    switch (buttonId)
                    {
                        case 0:
                            return Mouse.current.leftButton.isPressed;
                        case 1:
                            return Mouse.current.rightButton.isPressed;
                        case 2:
                            return Mouse.current.middleButton.isPressed;
                    }
                    return false;
            }

            // Default to the old input system.
            return Input.GetMouseButton(buttonId);
        }

        static bool GetMouseButtonDown(int buttonId)
        {
            switch (GetInputMode())
            {
                case InputMode.newInputSystem:
                    switch (buttonId)
                    {
                        case 0:
                            return Mouse.current.leftButton.wasPressedThisFrame;
                        case 1:
                            return Mouse.current.rightButton.wasPressedThisFrame;
                        case 2:
                            return Mouse.current.middleButton.wasPressedThisFrame;
                    }
                    return false;
            }

            // Default to the old input system.
            return Input.GetMouseButtonDown(buttonId);
        }

        static bool GetMouseButtonUp(int buttonId)
        {
            switch (GetInputMode())
            {
                case InputMode.newInputSystem:
                    switch (buttonId)
                    {
                        case 0:
                            return Mouse.current.leftButton.wasReleasedThisFrame;
                        case 1:
                            return Mouse.current.rightButton.wasReleasedThisFrame;
                        case 2:
                            return Mouse.current.middleButton.wasReleasedThisFrame;
                    }
                    return false;
            }

            // Default to the old input system.
            return Input.GetMouseButtonUp(buttonId);
        }

        // Nested touch class to manage both kinds of touch.
        public class Touch
        {
            public Vector2 position;
            public int fingerId = -1;
            public enum Phase { None, Began, Moved, Stationary, Ended, Canceled }
            public Phase phase = Phase.None;
        }

        static Touch GetTouch(int touchId)
        {
            switch (GetInputMode())
            {
                case InputMode.newInputSystem:
                    UnityEngine.InputSystem.EnhancedTouch.Touch nt = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[touchId];
                    return new Touch
                    {
                        position = nt.screenPosition,
                        fingerId = nt.finger.index,
                        phase = (Touch.Phase)Enum.Parse(typeof(Touch.Phase), nt.phase.ToString())
                    };
            }

            // Default to the old input system.
            UnityEngine.Touch t = Input.GetTouch(touchId);
            return new Touch
            {
                position = t.position,
                fingerId = t.fingerId,
                phase = (Touch.Phase)Enum.Parse(typeof(Touch.Phase), t.phase.ToString())
            };
        }

        static int GetTouchCount()
        {
            switch (GetInputMode())
            {
                case InputMode.newInputSystem:
                    return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
            }

            // Default to the old input system.
            return Input.touchCount;
        }
    }
}