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
        // ship logic goes here 
    }
}
