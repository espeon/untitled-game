using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez.Textures;
using Nez.Sprites;
using Nez.Particles;
using Nez.Tiled;

using fnaGame.Shared;


namespace fnaGame.Controllers
{
    public class PlayerController : Component, ITriggerListener, IUpdatable
    {
        public float MoveSpeed = 75f;
        public float RunSpeed = 150f;
        public float FallSpeed = 15f; // for going left and right while falling
        public float Gravity = 900f;
        public float GravityWhileDashing = 100f;
        public float JumpHeight = 8 * 10;
        public float JumpSpeed = 8;
        public float JumpHorizontalBoost = 10f;
        public float CanJump = 3;
        public float DashSpeed = 8 * 10;
        public float Stamina = 140;

        public bool CanInput = true;
        public bool CanGravity = true;
        public bool CanDash = false;
        public bool CanMove = true;

        public bool IsRunning = false;
        public bool IsDashing = false;
        public bool IsClimbing = true;
        public string LatestAnimation;

        Camera _camera;
        SpriteAnimator _animator;
        TiledMapMover _mover;
        BoxCollider _boxCollider;
        TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();
        Vector2 _velocity;

        VirtualButton _jumpInput;
        VirtualButton _runInput;
        VirtualButton _dashInput;
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;


        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture("Content/Tilesets/caveman.png");
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);

            _camera = Entity.Scene.Camera;

            _boxCollider = Entity.GetComponent<BoxCollider>();
            _mover = Entity.GetComponent<TiledMapMover>();
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));

            // extract the animations from the atlas. they are setup in rows with 8 columns
            _animator.AddAnimation("Walk", new[]
            {
                sprites[0],
                sprites[1],
                sprites[2],
                sprites[3],
                sprites[4],
                sprites[5]
            });

            _animator.AddAnimation("Run", new[]
            {
                sprites[8 + 0],
                sprites[8 + 1],
                sprites[8 + 2],
                sprites[8 + 3],
                sprites[8 + 4],
                sprites[8 + 5],
                sprites[8 + 6]
            });

            _animator.AddAnimation("Idle", new[]
            {
                sprites[16]
            });

            _animator.AddAnimation("Attack", new[]
            {
                sprites[24 + 0],
                sprites[24 + 1],
                sprites[24 + 2],
                sprites[24 + 3]
            });

            _animator.AddAnimation("Death", new[]
            {
                sprites[40 + 0],
                sprites[40 + 1],
                sprites[40 + 2],
                sprites[40 + 3]
            });

            _animator.AddAnimation("Falling", new[]
            {
                sprites[48]
            });

            _animator.AddAnimation("Hurt", new[]
            {
                sprites[64],
                sprites[64 + 1]
            });

            _animator.AddAnimation("Jumping", new[]
            {
                sprites[72 + 0],
                sprites[72 + 1],
                sprites[72 + 2],
                sprites[72 + 3]
            });

            SetupInput();
        }

        public override void OnRemovedFromEntity()
        {
            // deregister virtual input
            _jumpInput.Deregister();
            _xAxisInput.Deregister();
            _yAxisInput.Deregister();
            _dashInput.Deregister();
            _runInput.Deregister();
        }

        void SetupInput()
        {
            // setup input for jumping. we will allow z on the keyboard or a on the gamepad
            _jumpInput = new VirtualButton();
            _jumpInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Z));
            _jumpInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));

            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left,
                Keys.Right));

            // vertical input of ^, used for dashing
            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadUpDown());
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickY());
            _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up,
                Keys.Down));

            // running input, left shift on keyboard and b on gamepad
            _runInput = new VirtualButton();
            _runInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.LeftShift));
            _runInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.B));

            //dash input, X on keyboard and X on gamepad
            _dashInput = new VirtualButton();
            _dashInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.X));
            _dashInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.X));
        }

        void IUpdatable.Update()
        {
            // handle movement and animations
            var moveDir = new Vector2(_xAxisInput.Value, 0);
            string animation = null;

            if (IsRunning == true && _collisionState.Below && !_runInput.IsDown)
                IsRunning = false;

            if (CanMove == false)
                return;

            if (CanInput == false)
            {
                if (!_collisionState.Below && _velocity.Y > 0)
                    animation = "Falling";

                // apply gravity
                if (CanGravity)
                    _velocity.Y += Gravity * Time.DeltaTime;

                // move
                _mover.Move(_velocity * Time.DeltaTime, _boxCollider, _collisionState);


                if (animation != null && !_animator.IsAnimationActive(animation))
                    _animator.Play(animation);

                return;
            }

            // moving right
            if (moveDir.X < 0 && Math.Abs(_velocity.X) < RunSpeed)
            {
                if (_collisionState.Below)
                {
                    // running
                    if (_runInput.IsDown)
                    {
                        animation = "Run";
                        _velocity.X = -RunSpeed;
                        IsRunning = true;
                    }
                    // walking
                    else
                    {
                        animation = "Walk";
                        _velocity.X = (MoveSpeed / moveDir.X);
                        Debug.Log(moveDir.X);
                    }
                }
                //moving when jumping
                else
                {
                    if (_velocity.X <= MoveSpeed)
                    {
                        if (IsRunning && (_velocity.X <= RunSpeed))
                        {
                            _velocity.X = -RunSpeed;
                        }
                        else
                            _velocity.X = (MoveSpeed / moveDir.X);
                    }
                    else
                    {
                        _velocity.X = _velocity.X + -FallSpeed;
                    }
                }
                _animator.FlipX = true;
            }
            // moving left
            else if (moveDir.X > 0 && Math.Abs(_velocity.X) < RunSpeed)
            {
                if (_collisionState.Below)
                {
                    if (_runInput.IsDown)
                    {
                        animation = "Run";
                        _velocity.X = RunSpeed;
                        IsRunning = true;
                    }
                    else
                    {
                        animation = "Walk";
                        _velocity.X = (MoveSpeed / moveDir.X);
                    }
                }
                else
                {
                    if (_velocity.X >= MoveSpeed)
                    {
                        if (IsRunning && (_velocity.X <= RunSpeed))
                        {
                            _velocity.X = RunSpeed;
                        }
                        else
                            _velocity.X = (MoveSpeed / moveDir.X);
                    }
                    else
                    {
                        _velocity.X = _velocity.X + FallSpeed;
                    }
                }
                _animator.FlipX = false;
            }
            //slowing down 
            else
            {
                if (_velocity.X >= 15)
                {
                    _velocity.X = _velocity.X - 15;
                    Debug.Log(_velocity.X);
                }
                else if (_velocity.X <= -15)
                {
                    _velocity.X = _velocity.X + 15;
                }
                else
                    _velocity.X = 0;

                if(Math.Abs(_velocity.X) >= 15 && Math.Abs(_velocity.X) <= MoveSpeed){
                    animation = "Hurt";
                }

                if (_collisionState.Below && animation != "Hurt" && Math.Abs(_velocity.X) <= MoveSpeed)
                    animation = "Idle";
            }

            if(_collisionState.Right || _collisionState.Left){
                Debug.Log(Stamina);
                Stamina -= 1;
                if(Stamina < 0){
                    Stamina = 140;
                    _velocity.X = -1200;
                }
            }

            // jump when on ground
            if ((_collisionState.Below || CanJump != 0) && _jumpInput.IsPressed)
            {
                animation = "Jumping";
                _velocity.Y = -Mathf.Sqrt(2f * JumpHeight * Gravity);
                Debug.Log(_velocity.Y);
                CanJump = 0;
            }

            //dash
            if (_dashInput.IsPressed && CanDash)
            {
                CanDash = false;

                Debug.Log(Math.Abs(_xAxisInput.Value) * DashSpeed);
                Debug.Log(Math.Abs(_yAxisInput.Value) * DashSpeed);

                float VeloX;
                float VeloY;

                // not moving horizontally
                if(_yAxisInput.Value == 0)
                {
                    Debug.Log("hello");
                    VeloX = Mathf.Sqrt(2f * DashSpeed * Gravity);
                    VeloY = 0;//-Mathf.Sqrt(1.5f * DashSpeed * GravityWhileDashing);
                    Core.StartCoroutine(DisableGravity(.50f)); //disable gravity for a bit
                    Core.StartCoroutine(DisableInputs(.50f)); //dash should last ~.75sec
                }
                else
                {
                VeloX = Mathf.Sqrt(2f * (Math.Abs(_xAxisInput.Value) * DashSpeed) * Gravity);
                VeloY = -Mathf.Sqrt(2f * (Math.Abs(_yAxisInput.Value) * DashSpeed) * Gravity);
                Core.StartCoroutine(DisableGravity(.25f)); //disable gravity for a bit
                }
                if (_xAxisInput.Value < 0 || _animator.FlipX == true)
                    VeloX = -VeloX;
                if (_yAxisInput.Value > 0)
                    VeloY = -VeloY;
                _velocity.X = VeloX;
                _velocity.Y = VeloY;

                if(_yAxisInput.Value < -.02f)
                    animation = "Death";

                //shake camera cause we can :sunglasses:
                Entity.GetComponent<CameraShake>().Shake(15, .75f, new Vector2(-VeloX, -VeloY));
                //emit particles (i dont know how to do shaders)
                Core.StartCoroutine(ParticleSnowDash(new Vector2().AngleBetween(new Vector2(1, 0), new Vector2(-VeloX, -VeloY)), 0.3f));
                Core.StartCoroutine(DisableMovement(.15f));
            }

            if (!_collisionState.Below && _velocity.Y > 0)
                animation = "Falling";

            // apply gravity
            if (CanGravity)
                _velocity.Y += Gravity * Time.DeltaTime;

            // move
            _mover.Move(_velocity * Time.DeltaTime, _boxCollider, _collisionState);

            //reset dashes and stamina and jump grace thingy
            if (_collisionState.Below)
            {
                _velocity.Y = 0;
                CanJump = 5;
                CanDash = true;
                Stamina = 140;
            }

            // snow particles when land
            if (_collisionState.BecameGroundedThisFrame && !_collisionState.IsGroundedOnOneWayPlatform)
            {
                Particles.LoadParticleSystem(1, Entity, 90, 100);
            }

            if (LatestAnimation != animation)
            {
                Debug.Log(animation);
            }

            if (animation != null && !_animator.IsAnimationActive(animation))
                _animator.Play(animation);

            // jump grace countdown (-1 per frame)
            if (CanJump > 0 && !_collisionState.Below)
            {
                CanJump--;
                Debug.Log($"CanJump {CanJump}");
            }

            LatestAnimation = animation;
        }

        private IEnumerator DisableMovement(float seconds)
        {
            CanMove = false;
            yield return Coroutine.WaitForSeconds(seconds);
            CanMove = true;
        }

        private IEnumerator DisableGravity(float seconds)
        {
            CanGravity = false;
            yield return Coroutine.WaitForSeconds(seconds);
            CanGravity = true;
        }

        private IEnumerator DisableInputs(float seconds)
        {
            yield return Coroutine.WaitForSeconds(.05f);
            CanGravity = false;
            yield return Coroutine.WaitForSeconds(seconds - .05f);
            CanGravity = true;
        }

        private IEnumerator ParticleSnowDash(float angle, float secs)
        {
            yield return Coroutine.WaitForSeconds(.25f);
            Particles.LoadParticleSystem(1, Entity, -angle, 75);
            _camera.SetZoom(0.01f);
            yield return Coroutine.WaitForSeconds(secs);
            Debug.Log("coroutine finished?");
            _camera.SetZoom(0f);
        }

        #region ITriggerListener implementation

        void ITriggerListener.OnTriggerEnter(Collider other, Collider self)
        {
            Debug.Log("triggerEnter: {0}", other.Entity.Name);
        }

        void ITriggerListener.OnTriggerExit(Collider other, Collider self)
        {
            Debug.Log("triggerExit: {0}", other.Entity.Name);
        }

        #endregion
    }
}