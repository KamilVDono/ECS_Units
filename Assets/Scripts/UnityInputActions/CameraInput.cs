// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/UnityInputActions/CameraInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UnityInput
{
    public class @CameraInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @CameraInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""CameraInput"",
    ""maps"": [
        {
            ""name"": ""Camera"",
            ""id"": ""d4b44bd9-bf8d-4ced-a84c-13240028aaa9"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3d3f85d3-08b6-47ce-81fe-69fdc28d0698"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""9554c7dc-b598-49cb-a825-8044ea7be2fb"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WSAD"",
                    ""id"": ""6b5a7ca2-c080-4e75-a898-b92f713c448d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6f020f36-a4e1-4199-9b9f-52c5890951b5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7f2b59fb-39de-4382-b4db-bc8dae06fb14"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5d98add3-d2ff-427e-b00b-56e27399cb6d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""03e50a4f-a10d-4f4c-b1d1-4262ac75212d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Mouse"",
                    ""id"": ""1c16f6ae-2077-4b67-9650-25a967b53a63"",
                    ""path"": ""ConditionalComposite2D"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""X"",
                    ""id"": ""6bfbad95-834b-409c-b074-1ebbaac46b91"",
                    ""path"": ""<Pointer>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Y"",
                    ""id"": ""587214bd-492c-43b2-b0df-8e7e638e3d66"",
                    ""path"": ""<Pointer>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""ConditionalButton"",
                    ""id"": ""a31ed37f-78e1-4e18-9926-11137da83ddd"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""914e219f-25f8-4f67-8387-0474fe504baf"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Camera
            m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
            m_Camera_Move = m_Camera.FindAction("Move", throwIfNotFound: true);
            m_Camera_Zoom = m_Camera.FindAction("Zoom", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Camera
        private readonly InputActionMap m_Camera;
        private ICameraActions m_CameraActionsCallbackInterface;
        private readonly InputAction m_Camera_Move;
        private readonly InputAction m_Camera_Zoom;
        public struct CameraActions
        {
            private @CameraInput m_Wrapper;
            public CameraActions(@CameraInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Camera_Move;
            public InputAction @Zoom => m_Wrapper.m_Camera_Zoom;
            public InputActionMap Get() { return m_Wrapper.m_Camera; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
            public void SetCallbacks(ICameraActions instance)
            {
                if (m_Wrapper.m_CameraActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnMove;
                    @Zoom.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                    @Zoom.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                    @Zoom.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                }
                m_Wrapper.m_CameraActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Zoom.started += instance.OnZoom;
                    @Zoom.performed += instance.OnZoom;
                    @Zoom.canceled += instance.OnZoom;
                }
            }
        }
        public CameraActions @Camera => new CameraActions(this);
        public interface ICameraActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnZoom(InputAction.CallbackContext context);
        }
    }
}
