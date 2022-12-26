using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter;

static class EntityManager
{
    private static List<Entity> _entities = new List<Entity>();
    private static List<Enemy> _enemies = new List<Enemy>();
    private static List<Bullet> _bullets = new List<Bullet>();

    static bool isUpdating;
    static List<Entity> addedEntities = new List<Entity>();

    public static int Count
    { 
        get { return _entities.Count; } 
    }

    public static void Add(Entity entity)
    {
        if (!isUpdating) 
        {
            AddEntity(entity);
        }
        else
        {
            addedEntities.Add(entity);  
        }
    }

    private static void AddEntity(Entity entity)
    {
        _entities.Add(entity);

        if (entity is Bullet)
        {
            _bullets.Add(entity as Bullet);
        }
        else if (entity is Enemy)
        {
            _enemies.Add(entity as Enemy);
        }
    }

    public static void Update()
    {
        isUpdating = true;

        HandleCollisions();

        foreach (var entity in _entities) 
        { 
            entity.Update();
        }

        isUpdating = false;

        foreach (var entity in addedEntities)
        {
            _entities.Add(entity);
        }

        addedEntities.Clear();

        // remove any expired entities. 
        _entities = _entities.Where(x => !x.IsExpired).ToList();
        _bullets = _bullets.Where(x => !x.IsExpired).ToList();
        _enemies = _enemies.Where(x => !x.IsExpired).ToList();
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in _entities)
        {
            entity.Draw(spriteBatch);
        }
    }

    private static bool IsColliding(Entity a, Entity b)
    {
        float radius = a.Radius + b.Radius;
        return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
    }

    private static void HandleCollisions()
    {
        // handle collisions between enemies 
        for (int i = 0; i < _enemies.Count; i++)
            for (int j = i + 1; j < _enemies.Count; j++)
            {
                if (IsColliding(_enemies[i], _enemies[j]))
                {
                    _enemies[i].HandleCollision(_enemies[j]);
                    _enemies[j].HandleCollision(_enemies[i]);
                }
            }

        // handle collisions between bullets and enemies 
        for (int i = 0; i < _enemies.Count; i++)
            for (int j = 0; j < _bullets.Count; j++)
            {
                if (IsColliding(_enemies[i], _bullets[j]))
                {
                    _enemies[i].WasShot();
                    _bullets[j].IsExpired = true;
                }
            }

        // handle collisions between the player and enemies 
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].IsActive && IsColliding(PlayerShip.Instance, _enemies[i]))
            {
                PlayerShip.Instance.Kill();
                _enemies.ForEach(x => x.WasShot());
                EnemySpawner.Reset();
                break;
            }
        }
    }
}
