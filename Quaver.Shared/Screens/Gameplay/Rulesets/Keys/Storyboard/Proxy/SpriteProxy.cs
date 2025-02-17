using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Wobble.Graphics;
using Wobble.Graphics.Sprites;

namespace Quaver.Shared.Screens.Gameplay.Rulesets.Keys.Storyboard.Proxy;

public class SpriteProxy : DrawableProxy
{
    private readonly Sprite _drawable;

    [MoonSharpHidden]
    public SpriteProxy(Sprite drawable) : base(drawable)
    {
        _drawable = drawable;
    }

    public Sprite Rotate(float radian)
    {
        Rotation += radian;
        return _drawable;
    }
    
    public float Rotation
    {
        get => _drawable.Rotation;
        set => _drawable.Rotation = value;
    }

    public float Alpha
    {
        get => _drawable.Alpha;
        set => _drawable.Alpha = value;
    }

    public Color Tint
    {
        get => _drawable.Tint;
        set => _drawable.Tint = value;
    }
}