using KirosEngine3.Math;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Camera
{
    public class BaseCamera
    {
        private Vec3 _position;
        private Vec3 _lookAt;

        //camera coordinate space
        private Vec3 _forward = -Vec3.UnitZ;
        private Vec3 _up = Vec3.UnitY;
        private Vec3 _right = Vec3.UnitX;

        //rotations
        private float _pitch;
        private float _yaw = -MathHelpers.PiOver2;

        private float _fov = MathHelpers.PiOver2;
        private float _nearClip = 0.01f;
        private float _farClip = 100.0f;

        private float _width;
        private float _height;
        private float _aspectRatio;

        //camera move speed
        private float _speed;

        //mouse control variables
        private bool _firstMove = true;
        private Vec2 _lastPos;
        private float _sensitivity;

        //view matrices
        private Matrix4 _view;
        private Matrix4 _projection;
        private Matrix4 _orthographic;

        /// <summary>
        /// The position of the camera in world space
        /// </summary>
        public Vec3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                UpdateViewMatrix();
            }
        }

        /// <summary>
        /// Point to direct the camera to look at
        /// </summary>
        public Vec3 LookAt
        {
            get { return _lookAt; }
            set 
            { 
                _lookAt = value;
                LookAtSet();
            }
        }

        /// <summary>
        /// The forward vector of the view space
        /// </summary>
        public Vec3 Forward
        {
            get { return _forward; }
            set 
            { 
                _forward = value; 
                UpdateViewMatrix();
            }
        }

        /// <summary>
        /// The up vector of the view space
        /// </summary>
        public Vec3 Up
        {
            get { return _up; }
            set 
            {
                _up = value;
                UpdateViewMatrix();
            }
        }

        /// <summary>
        /// The right vector of the view space
        /// </summary>
        public Vec3 Right
        {
            get { return _right; }
            set { _right = value; }
        }

        /// <summary>
        /// The pitch of the camera in degrees
        /// </summary>
        public float Pitch
        {
            get { return MathHelpers.RadToDeg(_pitch); }
            set
            {
                //clamp between -89 and 89 deg to prevent gimble lock
                var angle = MathHelpers.Clamp(value, -89.0f, 89.0f);
                _pitch = MathHelpers.DegToRad(angle);
                UpdateVectors();
            }
        }

        /// <summary>
        /// The yaw of the camera in degrees
        /// </summary>
        public float Yaw
        {
            get { return MathHelpers.RadToDeg(_yaw); }
            set
            {
                _yaw = MathHelpers.DegToRad(value);
                UpdateVectors();
            }
        }

        /// <summary>
        /// The camera's field of view angle
        /// </summary>
        public float Fov
        {
            get { return MathHelpers.RadToDeg(_fov); }
            set
            {
                var angle = MathHelpers.Clamp(value, 1.0f, 90.0f);
                _fov = MathHelpers.DegToRad(angle);
                UpdateProjMatrix();
            }
        }

        /// <summary>
        /// The distance of the near clip plane from the camera's position
        /// </summary>
        public float NearClipDistance
        {
            get { return _nearClip; }
            set 
            {
                _nearClip = value <= 0.0f ? 0.01f : value; //minimum allowed value of 0.01
                UpdateProjMatrix();
                UpdateOrthoMatrix();
            }
        }

        /// <summary>
        /// The distance of the far clip plane from the camera's position
        /// </summary>
        public float FarClipDistance
        {
            get { return _farClip; }
            set
            {
                _farClip = value <= _nearClip ? _nearClip + 1.0f : value; //minimum allowed value of _nearClip + 1
                UpdateProjMatrix();
                UpdateOrthoMatrix();
            }
        }

        /// <summary>
        /// The width of the view, commonly the window's width
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value <= 0.0f ? 1.0f : value; //minimum allowed value of 1
                _aspectRatio = _width / _height; //update aspectRatio
                UpdateProjMatrix();
                UpdateOrthoMatrix();
            }
        }

        /// <summary>
        /// The height of the view, commonly the window's height
        /// </summary>
        public float Height
        {
            get { return _height; }
            set
            {
                _height = value <= 0.0f ? 1.0f : value; //minimum allowed value of 1
                _aspectRatio = _width / _height; //update aspectRatio
                UpdateProjMatrix();
                UpdateOrthoMatrix();
            }
        }

        /// <summary>
        /// The aspect ratio of the camera's view
        /// </summary>
        public float AspectRatio //todo: change accessibility and alter through width/height
        {
            get { return _aspectRatio; }
            set
            {
                _aspectRatio = value;
                UpdateProjMatrix();
            }
        }

        /// <summary>
        /// The camera's movement speed
        /// </summary>
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        /// <summary>
        /// The camera's turning sensitivity
        /// </summary>
        public float Sensitivity
        {
            get { return _sensitivity; }
            set { _sensitivity = value; }
        }

        /// <summary>
        /// The view matrix for the camera
        /// </summary>
        public Matrix4 View
        {
            get { return _view; }
        }

        /// <summary>
        /// The projection matrix for the camera
        /// </summary>
        public Matrix4 Projection
        {
            get { return _projection; }
        }

        /// <summary>
        /// The orthographic matrix for the camera
        /// </summary>
        public Matrix4 Orthographic
        {
            get { return _orthographic; }
        }

        public BaseCamera(Vec3 position, float width, float height)
        {
            _position = position;
            _width = width;
            _height = height;
            _aspectRatio = width / height;

            _speed = 1.5f;
            _sensitivity = 0.2f;

            UpdateViewMatrix();
            UpdateProjMatrix();
            UpdateOrthoMatrix();
        }

        public BaseCamera(Vec3 position, Vec2 windowSize) :
            this(position, windowSize.X, windowSize.Y) { }

        public BaseCamera(Vec3 position, float width, float height, float speed)
        {
            _position = position;
            _width = width;
            _height = height;
            _aspectRatio = width / height;

            _speed = speed;
            _sensitivity = 0.2f;

            UpdateViewMatrix();
            UpdateProjMatrix();
            UpdateOrthoMatrix();
        }

        public BaseCamera(Vec3 position, Vec2 windowSize, float speed) :
            this(position, windowSize.X, windowSize.Y, speed) { }

        public BaseCamera(Vec3 position, float width, float height, float speed, float sensitivity)
        {
            _position = position;
            _width = width;
            _height = height;
            _aspectRatio = width / height;

            _speed = speed;
            _sensitivity = sensitivity;

            UpdateViewMatrix();
            UpdateProjMatrix();
            UpdateOrthoMatrix();
        }

        public BaseCamera(Vec3 position, Vec2 windowSize, float speed, float sensitivity) :
            this(position, windowSize.X, windowSize.Y, speed, sensitivity) { }

        /// <summary>
        /// Update the projection matrix based on changes to camera data
        /// </summary>
        private void UpdateProjMatrix()
        {
            _projection = Matrix4.CreatePerspectiveFOV(_fov, _aspectRatio, _nearClip, _farClip);
        }

        /// <summary>
        /// Update the view matrix based on changes to camera data
        /// </summary>
        private void UpdateViewMatrix()
        {
            _view = Matrix4.LookAt(Position, Position + Forward, Up);
        }

        private void UpdateOrthoMatrix()
        {
            _orthographic = Matrix4.CreateOrthographic(_width, _height, _nearClip, _farClip);
        }

        /// <summary>
        /// Sets the view matrix so that the camera looks at the point _lookAt
        /// </summary>
        private void LookAtSet()
        {
            _view = Matrix4.LookAt(_position, _lookAt, _up);
            //todo: update camera data based on normalize(lookat - position)
        }

        private void UpdateVectors()
        {
            //update the forward vector for rotations
            _forward.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _forward.Y = MathF.Sin(_pitch);
            _forward.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _forward.Normalize();

            //update the right and up vectors
            _right = Vec3.Normalize(Vec3.Cross(_forward, Vec3.UnitY));//calculate from the global up vector, can be changed for non FPS type cameras
            _up = Vec3.Normalize(Vec3.Cross(_right, _forward));

            UpdateViewMatrix();
        }

        public void OnUpdateFrame(FrameEventArgs e, MouseState mouse)
        {
            //todo: camera update method
        }
    }
}
