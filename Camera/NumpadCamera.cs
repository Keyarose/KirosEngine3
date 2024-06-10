using KirosEngine3.Input;
using KirosEngine3.Math;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Camera
{
    /// <summary>
    /// A camera fixed to look at the LookAt point from wherever it is.
    /// </summary>
    public class NumpadCamera : BaseCamera
    {
        protected float _offsetFromLookAt = 5.0f;

        protected float _minDistance = 0.9f;
        protected float _maxDistance = 15.0f;

        protected float _rotationSpeed = 20.0f;//degrees

        public NumpadCamera(Vec3 position, float width, float height) : this(position, width, height, 1.5f) { }

        public NumpadCamera(Vec3 position, float width, float height, float speed) : base(position, width, height, speed)
        {
            //register keysPressed
            Keys[] keysPressed = [Keys.KeyPad1, Keys.KeyPad3, Keys.KeyPad7];
            KeyboardEventManager.SubscribeKeyboardEvents("system", keysPressed, KeyboardEventType.KeyPressed, OnKeyPress);

            Keys[] keysHeld = [Keys.KeyPadAdd, Keys.KeyPadSubtract, Keys.KeyPad2, Keys.KeyPad4, Keys.KeyPad6, Keys.KeyPad8];
            KeyboardEventManager.SubscribeKeyboardEvents("system", keysHeld, KeyboardEventType.KeyHeld, OnKeyPress);
        }

        public NumpadCamera(Vec3 position, Vec2 windowSize) : this(position, windowSize.X, windowSize.Y) { }

        public NumpadCamera(Vec3 position, Vec2 windowSize, float speed) : base(position, windowSize, speed) { }

        /// <summary>
        /// Handle keyboard events that the camera is registered to receive notifications for.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        public void OnKeyPress(object sender, KeyboardEventArgs args)
        {
            Keys key = args.Key;
            double frameTime = args.Time;

            switch (key) 
            {
                case Keys.KeyPad1:
                    //set camera to look down z axis
                    Position = new Vec3(0.0f, 0.0f, _offsetFromLookAt);
                    LookAt = Vec3.Zero;
                    break;
                case Keys.KeyPad2:
                    //move camera down
                    RotateCamera(MathHelpers.DegToRad(-_rotationSpeed * (float)frameTime), _right);//todo: account for gimble
                    break;
                case Keys.KeyPad3:
                    //set camera to look down x axis
                    Position = new Vec3(_offsetFromLookAt, 0.0f, 0.0f);
                    LookAt = Vec3.Zero;
                    break;
                case Keys.KeyPad4:
                    //move camera left
                    RotateCamera(MathHelpers.DegToRad(-_rotationSpeed * (float)frameTime), _up);
                    break;
                case Keys.KeyPad6:
                    //move camera right
                    RotateCamera(MathHelpers.DegToRad(_rotationSpeed * (float)frameTime), _up);
                    break;
                case Keys.KeyPad7:
                    //set camera to look down y axis
                    Position = new Vec3(0.0f, _offsetFromLookAt, 0.0f);
                    LookAt = Vec3.Zero;
                    break;
                case Keys.KeyPad8:
                    //move camera up
                    RotateCamera(MathHelpers.DegToRad(_rotationSpeed * (float)frameTime), _right);
                    break;
                case Keys.KeyPadAdd:
                    //move camera in
                    Vec3 nPos = _position + _forward * _speed * (float)frameTime;

                    Vec3 diff = nPos - _lookAt;
                    if (diff.Length > _minDistance)
                        Position = nPos;
                    else
                        Position = -(_forward * _minDistance) + _lookAt;
                    break;
                case Keys.KeyPadSubtract:
                    //move camera out
                    Vec3 nPosi = _position - _forward * _speed * (float)frameTime;

                    Vec3 differ = nPosi - LookAt;
                    if (differ.Length < _maxDistance)
                        Position = nPosi;
                    else
                        Position = -(_forward * _maxDistance) + _lookAt;
                    break;
            }
            Console.WriteLine(this);
        }

        /// <inheritdoc/>
        protected override void UpdateViewMatrix()
        {
            _view = Matrix4.LookAt(_position, _lookAt, _up);
        }

        /// <summary>
        /// Rotate the camera around the LookAt point by the given angle on the given axis.
        /// </summary>
        /// <param name="angle">The angle to rotate by. (radians)</param>
        /// <param name="axis">The axis to rotate around.</param>
        public void RotateCamera(float angle, Vec3 axis)
        {
            Vec3 diff = _position - _lookAt;

            Matrix4 rot;
            if (axis == Vec3.UnitX)
            {
                rot = Matrix4.CreateRotationX(angle);
            }
            else if (axis == Vec3.UnitY) 
            {
                rot = Matrix4.CreateRotationY(angle);
            }
            else if (axis == Vec3.UnitZ)
            {
                rot = Matrix4.CreateRotationZ(angle);
            }
            else
            {
                rot = Matrix4.CreateRotationOnAxis(axis, angle);
            }

            Vec4 vR = rot * new Vec4(diff);
            Position = vR.Xyz + _lookAt;
        }
    }
}
