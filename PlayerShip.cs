using Microsoft.Xna.Framework;

namespace NeonShooter;

class PlayerShip : Entity
{
    private static PlayerShip instance;
    public static PlayerShip Instance
    {
        get 
        {
            if (instance is null)
            {
                instance = new PlayerShip();
            }

            return instance;
        }
    }

    private PlayerShip()
    {
        image = Art.Player;
        Position = GameRoot.ScreenSize / 2;
        Radius = 10;
    }

    public override void Update()
    {
        const float speed = 8;
        Velocity = speed * Input.GetMovementDirection();
        Position += Velocity;
        Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

        if (Velocity.LengthSquared() > 0)
        {
            Orientation = Velocity.ToAngle();
        }
    }
}
