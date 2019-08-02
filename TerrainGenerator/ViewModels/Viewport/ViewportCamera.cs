using System.Windows.Media.Media3D;
using System.Windows.Input;
using Topographer3D.Commands;
using Topographer3D.Utilities;
using Topographer3D.Models;

namespace Topographer3D.ViewModels
{
    public class ViewportCamera : ObservableObject
    {
        #region PROPERTIES
        public RestrictedCamera Camera { get; private set; }
        public bool IsOrthographic { get; set; }
        private bool isDynamicLighting;
        public Vector3D DirectionalLightDirection { get; private set; }

        #endregion

        #region INITIALIZATION
        public ViewportCamera()
        {
            InitCamera();
            InitCommands();
            ChangeLightingMode(0);
        }

        private void InitCamera()
        {
            IsOrthographic = true;

            Camera = new RestrictedCamera(this);
            SetOrthographicCam();
        }

        private void InitCommands()
        {
            ChangeViewCommand = new ChangeViewCommand(this);
            ChangeLightingModeCommand = new ChangeLightingModeCommand(this);
        }

        #endregion

        #region COMMAND HANDLING
        public void SetView(int view)
        {
            Camera.ActivateManualy = true;
            switch (view)
            {
                // IsometricView
                case 0:
                    if (IsOrthographic)
                    {
                        Camera.Position = new Point3D(8, 8, 8);
                        Camera.LookDirection = new Vector3D(-8, -7.7, -8);
                        Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    else
                    {
                        Camera.Position = new Point3D(2, 1, 2);
                        Camera.LookDirection = new Vector3D(-2, -0.7, -2);
                        Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    break;

                // PerspectiveView
                case 1:
                    if (IsOrthographic)
                    {
                        Camera.Position = new Point3D(9, 6, 0);
                        Camera.LookDirection = new Vector3D(-9, -5.7, 0);
                        Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    else
                    {
                        Camera.Position = new Point3D(2, 1, 0);
                        Camera.LookDirection = new Vector3D(-2, -0.7, 0);
                        Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    break;

                // TopView
                case 2:
                    if (IsOrthographic)
                    {
                        Camera.Position = new Point3D(0, 15, 0);
                        Camera.LookDirection = new Vector3D(0, -14.3, 0);
                        Camera.UpDirection = new Vector3D(1, 0, 0);
                    }
                    else
                    {
                        Camera.Position = new Point3D(0, 3, 0);
                        Camera.LookDirection = new Vector3D(0, -2.7, 0);
                        Camera.UpDirection = new Vector3D(1, 0, 0);
                    }
                    break;

                // SideView
                case 3:
                    if (IsOrthographic)
                    {
                        Camera.Position = new Point3D(15, 0.3, 0);
                        Camera.LookDirection = new Vector3D(-15, 0, 0);
                        Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    else
                    {
                        Camera.Position = new Point3D(3.0, 0.3, 0.0);
                        Camera.LookDirection = new Vector3D(-3.0, 0.0, 0.0);
                        Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    break;
            }
            Camera.ActivateManualy = false;
        }

        public void SetPerspectiveCam()
        {
            Camera.FieldOfView = 50;
            IsOrthographic = false;
            SetView(0);
        }

        public void SetOrthographicCam()
        {
            Camera.FieldOfView = 10;
            IsOrthographic = true;
            SetView(0);
        }

        public void ChangeLightingMode(int lightingMode)
        {
            switch (lightingMode)
            {
                // Dynamic Lighting
                case 0:
                    isDynamicLighting = true;
                    Camera.LookDirection = Camera.LookDirection;
                    break;

                // Static Lighting
                case 1:
                    isDynamicLighting = false;
                    break;
            }
        }

        public void UpdateLight(Vector3D LookAt)
        {
            if(isDynamicLighting)
            {
                DirectionalLightDirection = LookAt;
            }
        }

        #endregion

        #region ICOMMANDS
        public bool CanExecute { get { return true; } }
        public ICommand ChangeViewCommand { get; private set; }
        public ICommand ChangeLightingModeCommand { get; private set; }
        #endregion
    }
}
