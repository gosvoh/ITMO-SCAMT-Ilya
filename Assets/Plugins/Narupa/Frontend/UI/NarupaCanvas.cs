// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Frontend.Controllers;
using Narupa.Frontend.Input;
using Narupa.Frontend.XR;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR;

namespace Narupa.Frontend.UI
{
    /// <summary>
    /// Component required to register a Unity canvas such that the given controller
    /// can interact with it.
    /// </summary>
    /// <remarks>
    /// All canvases that would like to be interacted with by physical controllers
    /// should have a script that derives from <see cref="NarupaCanvas" />. This should
    /// provide the controller and the action which is counted as a 'click'. The
    /// <see cref="RegisterCanvas" /> method can be overriden to provide a custom
    /// <see cref="IPosedObject" /> and <see cref="IButton" /> to provide the cursor
    /// location and click button.
    /// </remarks>
    [RequireComponent(typeof(Canvas))]
    public class NarupaCanvas : MonoBehaviour
    {
#pragma warning disable 0649
        /// <summary>
        /// The controller that can interact with this canvas.
        /// </summary>
        /// <remarks>
        /// Currently, only one controller can interact with a given canvas.
        /// </remarks>
        [SerializeField]
        private VrController controller;

        /// <summary>
        /// The SteamVR action that triggers a virtual mouse click for the UI.
        /// </summary>
        [SerializeField]
        private SteamVR_Action_Boolean inputAction;

        /// <summary>
        /// The input source to use for <see cref="inputAction" />.
        /// </summary>
        [SerializeField]
        private SteamVR_Input_Sources inputSource;
#pragma warning restore 0649

        private Canvas canvas;

        private void Awake()
        {
            Assert.IsNotNull(controller, $"{nameof(NarupaCanvas)} must have a pointer to the {nameof(VrController)} that will control it.");
            canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            RegisterCanvas();
        }

        /// <summary>
        /// Register the canvas with the cursor input system.
        /// </summary>
        protected virtual void RegisterCanvas()
        {
            WorldSpaceCursorInput.SetCanvasAndCursor(canvas,
                                                     controller.HeadPose,
                                                     inputAction.WrapAsButton(inputSource));
        }

        private void OnDisable()
        {
            WorldSpaceCursorInput.ReleaseCanvas(canvas);
        }
    }
}