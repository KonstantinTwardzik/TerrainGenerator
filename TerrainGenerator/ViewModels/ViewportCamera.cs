using HelixToolkit.Wpf.SharpDX;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Topographer3D.Commands;
using System.Windows.Controls;
using Topographer3D.Models;

namespace Topographer3D.ViewModels
{

    class ViewportCamera : ObservableObject
    {
        public RestrictedCamera Camera { get; private set; }
        public bool IsOrthographic { get; set; }

        public ViewportCamera()
        {
            InitCamera();
            InitCommands();
        }

        private void InitCamera()
        {
            IsOrthographic = false;
            Camera = new RestrictedCamera();
        }

        private void InitCommands()
        {
            TopViewCommand = new TopViewCommand(this);
            SideViewCommand = new SideViewCommand(this);
            PerspectiveViewCommand = new PerspectiveViewCommand(this);

        }

        public void SetPerspectiveView()
        {
            Camera.ActivateManual = true;
            if (IsOrthographic)
            {
                Camera.Position = new Point3D(10, 10, 0);
                Camera.LookDirection = new Vector3D(-10, -9.7, 0);
                Camera.UpDirection = new Vector3D(0, 1, 0);
            }
            else
            {
                Camera.Position = new Point3D(2, 2, 0);
                Camera.LookDirection = new Vector3D(-2, -1.7, 0);
                Camera.UpDirection = new Vector3D(0, 1, 0);
            }
            Camera.ActivateManual = false;
        }

        public void SetTopView()
        {
            Camera.ActivateManual = true;
            if (IsOrthographic)
            {
                Camera.Position = new Point3D(0, 20, 0);
                Camera.LookDirection = new Vector3D(0, -19.3, 0);
                Camera.UpDirection = new Vector3D(1, 0, 0);
            }
            else
            {
                Camera.Position = new Point3D(0, 4, 0);
                Camera.LookDirection = new Vector3D(0, -3.7, 0);
                Camera.UpDirection = new Vector3D(1, 0, 0);
            }
            Camera.ActivateManual = false;
        }

        public void SetSideView()
        {
            Camera.ActivateManual = true;
            if (IsOrthographic)
            {
                Camera.Position = new Point3D(20, 0.3, 0);
                Camera.LookDirection = new Vector3D(-20, 0, 0);
                Camera.UpDirection = new Vector3D(0, 1, 0);
            }
            else
            {
                Camera.Position = new Point3D(4.0, 0.3, 0.0);
                Camera.LookDirection = new Vector3D(-4.0, 0.0, 0.0);
                Camera.UpDirection = new Vector3D(0, 1, 0);

            }
            Camera.ActivateManual = false;
        }

        public void SetPerspectiveCam()
        {
            Camera.FieldOfView = 60;
            IsOrthographic = false;
            SetPerspectiveView();
        }

        public void SetOrthographicCam()
        {
            Camera.FieldOfView = 10;
            IsOrthographic = true;
            SetPerspectiveView();
        }

        #region ICommands 
        public bool CanExecute { get { return true; } }
        public ICommand TopViewCommand { get; private set; }
        public ICommand SideViewCommand { get; private set; }
        public ICommand PerspectiveViewCommand { get; private set; }
        #endregion
    }

    public class RestrictedCamera : HelixToolkit.Wpf.SharpDX.PerspectiveCamera
    {
        private Vector3D oldLookDir;
        public bool IsLocked { get; set; }
        public bool ActivateManual { get; set; }

        public RestrictedCamera()
        {
            Position = new Point3D(3, 3, 3);
            LookDirection = new Vector3D(-3, -2.7, -3);
            UpDirection = new Vector3D(0, 1, 0);
            FarPlaneDistance = 1000;
            ActivateManual = false;
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
