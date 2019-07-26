using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using System.Windows.Input;
using Topographer3D.Commands;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels
{

    public class ViewportCamera : ObservableObject
    {
        public RestrictedCamera Camera { get; private set; }
        public bool IsOrthographic { get; set; }
        private bool isDynamicLighting;
        public Vector3D DirectionalLightDirection { get; private set; }


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

        public void SetView(int view)
        {
            Camera.ActivateManual = true;
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
            Camera.ActivateManual = false;
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

        #region ICommands 
        public bool CanExecute { get { return true; } }
        public ICommand ChangeViewCommand { get; private set; }
        public ICommand ChangeLightingModeCommand { get; private set; }
        #endregion
    }

    public class RestrictedCamera : HelixToolkit.Wpf.SharpDX.PerspectiveCamera
    {
        private Vector3D oldLookDir;
        public bool IsLocked { get; set; }
        public bool ActivateManual { get; set; }

        private ViewportCamera vpc;

        public RestrictedCamera(ViewportCamera vpc)
        {
            ActivateManual = false;
            this.vpc = vpc;
        }


        public override Point3D Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                if (ActivateManual || value.Y >= 0 && (sameSign(base.Position.X, value.X) || sameSign(base.Position.Z, value.Z)))
                {
                    base.Position = value;
                    this.IsLocked = false;

                }
                else
                {
                    this.IsLocked = true;
                    base.LookDirection = this.oldLookDir;
                }
            }
        }

        public override Vector3D LookDirection
        {
            get
            {
                return base.LookDirection;
            }

            set
            {
                this.oldLookDir = base.LookDirection;
                base.LookDirection = value;
                vpc.UpdateLight(value);
            }
        }

        public override Vector3D UpDirection
        {
            get
            {
                return base.UpDirection;
            }

            set
            {
                if (!this.IsLocked)
                {
                    base.UpDirection = value;
                }
            }
        }

        bool sameSign(double num1, double num2)
        {
            return num1 >= 0 && num2 >= 0 || num1 < 0 && num2 < 0;
        }

    }
}
