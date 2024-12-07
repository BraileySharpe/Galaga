﻿using System.Threading.Tasks;
using Galaga.View.Sprites;

namespace Galaga.Model;

/// <summary>
///     The Explosion class.
/// </summary>
public class Explosion : GameObject
{
    #region Constructors

    public Explosion(double x, double y)
    {
        Sprite = new ExplosionSprite();
        X = x + 5;
        Y = y - 5;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Causes an explosion.
    /// </summary>
    public async Task Explode()
    {
        var sprite = Sprite as AnimatedSprite;
        for (var i = 0; i < 3; i++)
        {
            sprite.ToggleSprite();
            await Task.Delay(100);
        }
    }

    #endregion
}