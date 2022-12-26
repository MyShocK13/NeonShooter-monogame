using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace NeonShooter;

static class Sound
{
    public static Song Music { get; private set; }

    private static readonly Random _rand = new Random();
    
    private static SoundEffect[] _explosions;
    public static SoundEffect Explosion { get { return _explosions[_rand.Next(_explosions.Length)]; } }

    private static SoundEffect[] _shots;
    public static SoundEffect Shot { get { return _shots[_rand.Next(_shots.Length)]; } }

    private static SoundEffect[] _spawns;
    public static SoundEffect Spawn { get { return _spawns[_rand.Next(_spawns.Length)]; } }

    public static void Load(ContentManager content)
    {
        Music = content.Load<Song>("Sound/Music");

        // These linq expressions are just a fancy way loading all sounds of each category into an array. 
        _explosions = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Sound/explosion-0" + x)).ToArray();
        _shots = Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Sound/shoot-0" + x)).ToArray();
        _spawns = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Sound/spawn-0" + x)).ToArray();
    }
}