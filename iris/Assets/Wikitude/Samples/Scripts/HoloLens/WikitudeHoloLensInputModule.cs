using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Wikitude
{
    /*
     * WikitudeHololensInputModule is a copy of the native HololensInputModule.
     * The Wikitude HoloLens Plugin supports Unity starting with version 2017.4, where the HololensInputModule is part of the base Unity installation,
     * up to version 2018.3, where the HoloLensInputModule is part of the WindowsMixedReality package. To support both these version, as well as everything in between, 
     * without any modification, this file is duplicated here. The name and namespace of the class is changed as to not conflict with the original version.
     */
    [RequireComponent(typeof(WikitudeHoloLensInput))]
    [AddComponentMenu("Event/Wikitude HoloLens Input Module")]
    public partial class WikitudeHoloLensInputModule : StandaloneInputModule
    {
        public float normalizedNavigationToScreenOffsetScalar
        {
            get { return m_NormalizedNavigationToScreenOffsetScalar; }
            set { m_NormalizedNavigationToScreenOffsetScalar = value; }
        }

        public float timeToPressOnTap
        {
            get { return m_TimeToPressOnTap; }
            set { m_TimeToPressOnTap = value; }
        }

        protected WikitudeHoloLensInputModule()
        {
        }

        [SerializeField]
        [Tooltip("Maximum number of pixels in screen space to move a widget during a navigation gesture")]
        private float m_NormalizedNavigationToScreenOffsetScalar = 500.0f;

        [SerializeField]
        [Tooltip("Amount of time to show things that responds to clicks in their on-pressed state")]
        private float m_TimeToPressOnTap = 0.3f;

        private WikitudeHoloLensInput m_HoloLensInput;
        private bool m_HasBeenActivated = false;
        private bool m_HasGestureToProcess = false;

        ///////////////////
        // MonoBehaviour //
        ///////////////////

        protected override void Awake()
        {
            base.Awake();
            m_HoloLensInput = GetComponent<WikitudeHoloLensInput>();
            if (!m_HoloLensInput)
                m_HoloLensInput = gameObject.AddComponent<WikitudeHoloLensInput>();
            m_InputOverride = m_HoloLensInput;
        }

        ///////////////////////////
        // InputModule overrides //
        ///////////////////////////

        public override bool IsModuleSupported()
        {
            return base.IsModuleSupported() && string.Equals(UnityEngine.XR.XRSettings.loadedDeviceName, "WindowsMR");
        }

        public override bool ShouldActivateModule()
        {
            return forceModuleActive || m_HasGestureToProcess || !m_HasBeenActivated;
        }

        public override void ActivateModule()
        {
            m_HasBeenActivated = true;
            base.ActivateModule();
        }

        public override void UpdateModule()
        {
            m_HoloLensInput.UpdateInput();
            base.UpdateModule();
        }

        protected override void ProcessDrag(PointerEventData pointerEvent)
        {
            // TBaird: The Hololens has an explicit Drag gesture, and so we want to avoid starting any dragging unless that gesture is active.
            if (m_HoloLensInput.AllowDrag())
            {
                base.ProcessDrag(pointerEvent);
            }
        }

        /////////////////////////////
        // HoloLensInput callbacks //
        /////////////////////////////

        internal GameObject Internal_GetCurrentFocusedGameObject()
        {
            return GetCurrentFocusedGameObject();
        }

        internal void Internal_GestureNotifier()
        {
            m_HasGestureToProcess = true;
        }
    }
}
