﻿using System;
using Microsoft.Xna.Framework;
using Wobble.Graphics;
using Wobble.Graphics.Animations;
using Wobble.Graphics.Sprites;
using Wobble.Window;

namespace Quaver.Graphics.Transitions
{
    public static class Transitioner
    {
        /// <summary>
        ///     The blackness sprite that acts as the screen's transitioner
        /// </summary>
        public static Sprite Blackness { get; set; }


        /// <summary>
        ///     Initializes the screen transitioner.
        /// </summary>
        public static void Initialize()
        {
            Blackness = new Sprite()
            {
                Size = new ScalableVector2(WindowManager.Width, WindowManager.Height),
                Tint = Color.Black,
                Alpha = 0
            };

            WindowManager.ResolutionChanged += OnResolutionChanged;
        }

        public static void Update(GameTime gameTime) => Blackness.Update(gameTime);
        public static void Draw(GameTime gameTime) => Blackness.Draw(gameTime);

        /// <summary>
        ///     Disposes of the the transitioner.
        /// </summary>
        public static void Dispose()
        {
            Blackness.Destroy();
            Blackness = null;

            WindowManager.ResolutionChanged -= OnResolutionChanged;
        }

        /// <summary>
        ///     Called when the resolution of the game changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnResolutionChanged(object sender, EventArgs e) =>
            Blackness.Size = new ScalableVector2(WindowManager.Width, WindowManager.Height);

        /// <summary>
        ///     Fades the transitioner out
        /// </summary>
        public static void FadeIn()
        {
            Blackness.ClearAnimations();
            Blackness.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.Linear, Blackness.Alpha, 1, 200));
        }

        /// <summary>
        ///     Unfades the transitioner
        /// </summary>
        public static void FadeOut()
        {
            Blackness.ClearAnimations();
            Blackness.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.Linear, Blackness.Alpha, 0, 200));
        }
    }
}