using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using TerrainGenerator.ViewModels;
using System;
using System.Windows.Media;
using TerrainGenerator.Models;

namespace TerrainGenerator.Views
{
    /// <summary>
    /// Interaction logic for ViewportCamera3D.xaml
    /// </summary>
    /// 

    public partial class ViewportCamera3D : UserControl
    {
        private bool mouseDownRotate;
        private bool mouseDownZoom;
        private bool mouseDownMove;
        private Point centerOfViewport;
        private double yaw;
        private double pitch;
        private double zoom;
        private double camPosX;
        private double camPosZ;

        public ViewportCamera3D()
        {
            InitializeComponent();
            this.SetPerspectiveView();
            this.DataContext = Application.Current.MainWindow.DataContext; 
        }

        public void ResetCamera()
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 5);
            camera.Transform = new Transform3DGroup();
        }

        public void SetPerspectiveView()
        {
            ResetCamera();
            yaw = 0;
            pitch = -180;
            this.Rotate();
        }

        public void SetTopView()
        {
            ResetCamera();
            yaw = 0;
            pitch = -(90 * 5);
            this.Rotate();
            
        }

        public void SetSideView()
        {
            ResetCamera();
            yaw = 0;
            pitch = 0;
            this.Rotate();
        }

        private void Rotate()
        {
            double theta = yaw / 5;
            double phi = pitch / 5;

            // clamp phi (pitch) between -90 and 90 to avoid 'going upside down'
            // just remove this if you want to make loopings :)
            if (phi < -90) phi = -90;
            else if (phi > 0) phi = 0;

            // here the rotation magic happens. ask jemidiah for details, i've no clue :p
            Vector3D thetaaxis = new Vector3D(0, 1, 0);
            Vector3D phiaxis = new Vector3D(-1, 0, 0);

            Transform3DGroup transformgroup = camera.Transform as Transform3DGroup;
            transformgroup.Children.Clear();
            QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(-phiaxis, phi));
            transformgroup.Children.Add(new RotateTransform3D(r));
            r = new QuaternionRotation3D(new Quaternion(-thetaaxis, theta));
            transformgroup.Children.Add(new RotateTransform3D(r));

        }

        public void Zoom()
        {
            // for zooming we simply change the z-position of the camera
            if (camera.Position.Z > 1.5 && zoom > 0 || camera.Position.Z < 10 && zoom < 0)
            {
                camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - zoom);
            }
        }

        public void Move()
        {
            //camera.position = new point3d(camera.position.x + camposx, camera.position.y, camera.position.z);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ((Keyboard.Modifiers & ModifierKeys.Alt) > 0))
            {
                mouseDownRotate = true;
                centerOfViewport = viewport.PointToScreen(new Point(viewport.ActualWidth / 2, viewport.ActualHeight / 2));
                MouseUtilities.SetPosition(centerOfViewport);
                this.Cursor = Cursors.None;
            }
            else if (e.RightButton == MouseButtonState.Pressed && ((Keyboard.Modifiers & ModifierKeys.Alt) > 0))
            {
                mouseDownZoom = true;
                centerOfViewport = viewport.PointToScreen(new Point(viewport.ActualWidth / 2, viewport.ActualHeight / 2));
                MouseUtilities.SetPosition(centerOfViewport);
                this.Cursor = Cursors.None;
            }
            else if (e.MiddleButton == MouseButtonState.Pressed && ((Keyboard.Modifiers & ModifierKeys.Alt) > 0))
            {
                mouseDownMove = true;
                centerOfViewport = viewport.PointToScreen(new Point(viewport.ActualWidth / 2, viewport.ActualHeight / 2));
                MouseUtilities.SetPosition(centerOfViewport);
                this.Cursor = Cursors.None;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // indicate that the mouse is no longer pressed and make the cursor visible again
            mouseDownRotate = false;
            mouseDownZoom = false;
            mouseDownMove = false;
            this.Cursor = Cursors.Arrow;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownRotate)
            {
                // get mouse position relative to viewport and transform it to the center
                // literally, actualrelativepos contains the x and y amounts that the mouse is away from the center of the viewport
                Point relativePos = Mouse.GetPosition(viewport);
                Point actualRelativePos = new Point(relativePos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - relativePos.Y);

                // dx and dy are the amounts  by which the mouse moved this move event. since we keep resetting the mouse to the
                // center, this is just the new position of the mouse, relative to the center: actualrelativepos.
                double dx = actualRelativePos.X;
                double dy = actualRelativePos.Y;

                yaw += dx;
                if (pitch >= -450 && dy < 0 || pitch <= 0 && dy > 0)
                {
                    pitch += dy;
                }

                this.Rotate();

                // set mouse position back to the center of the viewport in screen coordinates
                MouseUtilities.SetPosition(centerOfViewport);
            }

            if (mouseDownZoom)
            {
                zoom = 0;
                Point relativePos = Mouse.GetPosition(viewport);
                Point actualRelativePos = new Point(relativePos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - relativePos.Y);
                double dx = actualRelativePos.X;
                double dy = actualRelativePos.Y;
                zoom += dx / 500;
                zoom += dy / 500;
                this.Zoom();
                MouseUtilities.SetPosition(centerOfViewport);
            }

            if (mouseDownMove)
            {
                camPosX = 0;
                camPosZ = 0;
                Point relativePos = Mouse.GetPosition(viewport);
                Point actualRelativePos = new Point(relativePos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - relativePos.Y);
                double dx = actualRelativePos.X;
                double dy = actualRelativePos.Y;
                if (camera.Position.X < 1 && dx < 0 || camera.Position.X > -1 && dx > 0)
                {
                    camPosX -= dx / 1000;
                }
                if (camPosZ < 1 && dy < 0 || camPosZ > -1 && dy > 0)
                {
                    camPosZ -= dy / 1000;
                }
                this.Move();
                MouseUtilities.SetPosition(centerOfViewport);
            }
        }

        private void HandlePerspectiveView(object sender, RoutedEventArgs e)
        {
            SetPerspectiveView();
        }

        private void HandleSideView(object sender, RoutedEventArgs e)
        {
            SetSideView();
        }

        private void HandleTopView(object sender, RoutedEventArgs e)
        {
            SetTopView();
        }
    }
}