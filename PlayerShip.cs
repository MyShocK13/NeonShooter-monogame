using Microsoft.Xna.Framework;
using System;

namespace NeonShooter;

class PlayerShip : Entity
{
    private static PlayerShip _instance;
    public static PlayerShip Instance
    {
        get 
        {
            if (_instance is null)
            {
                _instance = new PlayerShip();
            }

            return _instance;
        }
    }

    private const float Speed = 8;
    private const int CooldownFrames = 6;
    private int _cooldownRemaining = 0;
    private static Random _rand = new Random();

    private PlayerShip()
    {
        image = Art.Player;
        Position = GameRoot.ScreenSize / 2;
        Radius = 10;
    }

    public override void Update()
    {
        
        Velocity = Speed * Input.GetMovementDirection();
        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

        if (Velocity.LengthSquared() > 0)
        {
            Orientation = Velocity.ToAngle();
        }

        var aim = Input.GetAimDirection();
        if (aim.LengthSquared() > 0 && _cooldownRemaining <= 0)
        {
            _cooldownRemaining = CooldownFrames;
            float aimAngle = aim.ToAngle();
            Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

            float randomSpread = _rand.NextFloat(-0.04f, 0.04f) + _rand.NextFloat(-0.04f, 0.04f);
            Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

            Vector2 offset = Vector2.Transform(new Vector2(25, -8), aimQuat);
            EntityManager.Add(new Bullet(Position + offset, vel));

            offset = Vector2.Transform(new Vector2(25, 8), aimQuat);
            EntityManager.Add(new Bullet(Position + offset, vel));
        }

        if (_cooldownRemaining > 0)
        {
            _cooldownRemaining--;
        }
    }
}
