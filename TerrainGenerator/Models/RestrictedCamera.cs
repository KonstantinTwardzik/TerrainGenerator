using System.Windows.Media.Media3D;
using Topographer3D.ViewModels;
namespace Topographer3D.Models
{
    public class RestrictedCamera : HelixToolkit.Wpf.SharpDX.PerspectiveCamera
    {
        #region ATTRIBUTES & PROPERTIES
        private ViewportCamera vpc;
        private Vector3D oldLookDir;
        public bool IsLocked { get; set; }
        public bool ActivateManualy { get; set; }

        #endregion

        #region LOGIC
        public RestrictedCamera(ViewportCamera vpc)
        {
            ActivateManualy = false;
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
                if (ActivateManualy || value.Y >= 0 && (sameSign(base.Position.X, value.X) || sameSign(base.Position.Z, value.Z)))
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
        
        #endregion
    }
}