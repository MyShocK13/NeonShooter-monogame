using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeonShooter;

class Enemy : Entity
{
    public static Random Rand = new Random();
    public bool IsActive { get { return _timeUntilStart <= 0; } }

    private int _timeUntilStart = 60;
    private List<IEnumerator<int>> _behaviours = new List<IEnumerator<int>>();

    public Enemy(Texture2D image, Vector2 position)
    {
        this.image = image;
        Position = position;
        Radius = image.Width / 2f;
        color = Color.Transparent;
    }

    public override void Update()
    {
        if (_timeUntilStart <= 0)
        {
            ApplyBehaviours();
        }
        else
        {
            _timeUntilStart--;
            color = Color.White * (1 - _timeUntilStart / 60f);
        }

        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

        Velocity *= 0.8f;
    }

    public void WasShot()
    {
        IsExpired = true;
    }

    public static Enemy CreateSeeker(Vector2 position)
    {
        var enemy = new Enemy(Art.Seeker, position);
        enemy.AddBehaviour(enemy.FollowPlayer());

        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var enemy = new Enemy(Art.Wanderer, position);
        enemy.AddBehaviour(enemy.MoveRandomly());

        return enemy;
    }

    private void AddBehaviour(IEnumerable<int> behaviour)
    {
        _behaviours.Add(behaviour.GetEnumerator());
    }

    private void ApplyBehaviours()
    {
        for (int i = 0; i < _behaviours.Count; i++)
        {
            if (!_behaviours[i].MoveNext())
            {
                _behaviours.RemoveAt(i--);
            }
        }
    }

    #region Behaviours
    IEnumerable<int> FollowPlayer(float acceleration = 1f)
    {
        while (true)
        {
            Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
            
            if (Velocity != Vector2.Zero)
            {
                Orientation = Velocity.ToAngle();
            }

            yield return 0;
        }
    }

    IEnumerable<int> MoveInASquare()
    {
        const int framesPerSide = 30;
        while (true)
        {
            // move right for 30 frames 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = Vector2.UnitX;
                yield return 0;
            }
            // move down 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = Vector2.UnitY;
                yield return 0;
            }
            // move left 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = -Vector2.UnitX;
                yield return 0;
            }
            // move up 
            for (int i = 0; i < framesPerSide; i++)
            {
                Velocity = -Vector2.UnitY;
                yield return 0;
            }
        }
    }

    IEnumerable<int> MoveRandomly()
    {
        float direction = Rand.NextFloat(0, MathHelper.TwoPi);
        while (true)
        {
            direction += Rand.NextFloat(-0.1f, 0.1f);
            direction = MathHelper.WrapAngle(direction);

            for (int i = 0; i < 6; i++)
            {
                Velocity += MathUtil.FromPolar(direction, 0.4f);
                Orientation -= 0.05f;

                var bounds = GameRoot.Viewport.Bounds;
                bounds.Inflate(-image.Width, -image.Height);

                // if the enemy is outside the bounds, make it move away from the edge 
                if (!bounds.Contains(Position.ToPoint()))
                    direction = (GameRoot.ScreenSize / 2 - Position).ToAngle() + Rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                
                yield return 0;
            }
        }
    }
    #endregion
}
