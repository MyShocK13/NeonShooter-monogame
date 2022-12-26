using Microsoft.Xna.Framework;
using System;

namespace NeonShooter;

static class EnemySpawner
{
    private static Random _rand = new Random();
    private static float _inverseSpawnChance = 60;

    public static void Update()
    {
        if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
        {
            if (_rand.Next((int)_inverseSpawnChance) == 0)
            {
                EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
            }

            if (_rand.Next((int)_inverseSpawnChance) == 0)
            {
                EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
            }
        }

        // slowly increase the spawn rate as time progresses 
        if (_inverseSpawnChance > 20)
        {
            _inverseSpawnChance -= 0.005f;
        }
    }

    private static Vector2 GetSpawnPosition()
    {
        Vector2 pos;

        do
        {
            pos = new Vector2(_rand.Next((int)GameRoot.ScreenSize.X), _rand.Next((int)GameRoot.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);
        
        return pos;
    }

    public static void Reset()
    {
        _inverseSpawnChance = 60;
    }
}
