﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quaver.Graphics.Sprites;
using Quaver.Graphics.UniversalDim;
using Quaver.Main;

namespace Quaver.Graphics.UserInterface
{
    /// <inheritdoc />
    /// <summary>
    ///     Sprite that displays numbers as textures.
    /// </summary>
    internal class NumberDisplay : Sprite
    {
        /// <summary>
        ///     The number value for this display in string format.
        ///     If the value isn't a valid number or percentage.
        ///
        ///     Sample Inputs:
        ///         - 123
        ///         - 1000000
        ///         - 40.23
        ///         - 99.12%
        /// </summary>
        private string _value;
        internal string Value
        {
            get => _value;
            set
            {
                // Here we run a check to see if the value incoming isn't the same as the
                // already set one. If is then we skip over it. If not, we set the new value,
                // and re-initialize the digits. This is so that we aren't looping more times than
                // we should per frame.
                if (_value == value) 
                    return;
                
                _value = value;
                LastValueChangeTime = GameBase.GameTime.ElapsedMilliseconds;
                
                // Only initialize if Digits has already been created.
                if (Digits != null)
                    InitializeDigits();
            } 
        }
        
        /// <summary>
        ///     The type of number display this is.
        /// </summary>
        private NumberDisplayType Type { get; }

        /// <summary>
        ///     Regular expression for all the allowed characters.
        /// </summary>
        private static Regex AllowedCharacters { get; } = new Regex(@"(\d|%|\.|:|-)+");

        /// <summary>
        ///     The digits in the number display.
        /// </summary>
        internal List<Sprite> Digits { get; }

        /// <summary>
        ///     The absolute width of the number display.
        /// </summary>
        internal float TotalWidth
        {
            get
            {
                float sum = 0;

                foreach (var d in Digits)
                {
                    // Only calc width for actually visible digits.
                    if (!d.Visible)
                        continue;

                    sum += d.SizeX;
                }

                return sum;
            }
        }

        /// <summary>
        ///     The last time the value was changed
        ///     (Used for timing animations for example).
        /// </summary>
        internal long LastValueChangeTime { get; private set; }

        /// <summary>
        ///     The size of the digits.
        /// </summary>
        private Vector2 ImageScale { get; }

        /// <summary>
        ///     Ctor -
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startingValue"></param>
        /// <param name="imageScale"></param>
        internal NumberDisplay(NumberDisplayType type, string startingValue, Vector2 imageScale)
        {
            ImageScale = imageScale;
            Value = startingValue;
            Type = type;
            
            // First validate the initial value to see if everything is correct.
            Validate();
            
            // Create and initialize the digits.
            Digits = new List<Sprite>();
            InitializeDigits();
        }

        /// <summary>
        ///     Makes the display visible.
        /// </summary>
        internal void MakeVisible()
        {
            if (Visible)
                return;

            Visible = true;
            
            // Only make the digits we're using visible.
            for (var i = 0; i < Value.Length; i++)
                Digits[i].Visible = true;
        }
        
        /// <summary>
        ///     Makes the display invisible.
        /// </summary>
        internal void MakeInvisible()
        {
            if (!Visible)
                return;
            
            Visible = false;
            Digits.ForEach(x => x.Visible = false);
        }
        
        /// <summary>
        ///     Validates the current value to see if it is a correct number.
        ///     If it isn't, it'll throw an exception.
        /// </summary>
        private void Validate()
        {
            foreach (var c in Value)
            {
                if (!AllowedCharacters.IsMatch(c.ToString()))
                    throw new ArgumentException($"{c} is not a valid value for NumberDisplay.");             
            }
        }

        /// <summary>
        ///   Goes through each character in the value and either initializes the sprite
        ///   or updates the texture of it.
        /// </summary>
        private void InitializeDigits()
        {
            // Go through each character and either initialize/update the sprite with the correct
            // texture.
            for (var i = 0; i < Value.Length; i++)
            {
                // If the digit doesn't already exist, we need to create it.
                if (i >= Digits.Count)
                {             
                    Digits.Add(new Sprite
                    {
                        Parent = this,
                        Image = CharacterToTexture(Value[i]),
                    });
                    
                    // Set size
                    Digits[i].Size = new UDim2D(Image.Width / ImageScale.X, Image.Height / ImageScale.Y);
                                        
                    // Set position
                    // If it's the first image, set the x pos to 0.
                    if (i == 0)
                        Digits[i].PosX = 0;
                    // Otherwise, make it next to the previous one.
                    else
                        Digits[i].PosX = Digits[i - 1].PosX + Digits[i - 1].SizeX;
                    
                }
                // If the digit already exists, then we need to just update the texture of it.
                else
                {
                    Digits[i].Image = CharacterToTexture(Value[i]);
                }

                // Reset the sprite to be visible.
                Digits[i].Visible = true;
            }
            
            // Now check if the length of the digits matches the one of the value,
            // if it doesn't then we need to handle some of the extra lost digits.
            if (Value.Length == Digits.Count) 
                return;
            
            // Make the extra ones invisible.
            for (var i = Value.Length; i < Digits.Count; i++)
                Digits[i].Visible = false;
                
        }

        /// <summary>
        ///     Converts a single character into a texture.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private Texture2D CharacterToTexture(char c)
        {
            switch (c)
            {
                // 0
                case '0' when Type == NumberDisplayType.Score:
                case '0' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[0];
                case '0' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[0];
                case '0' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[0];
                // 1
                case '1' when Type == NumberDisplayType.Score:
                case '1' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[1];
                case '1' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[1];
                case '1' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[1];
                // 2
                case '2' when Type == NumberDisplayType.Score:
                case '2' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[2];
                case '2' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[2];
                case '2' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[2];
                // 3
                case '3' when Type == NumberDisplayType.Score:
                case '3' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[3];
                case '3' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[3];
                case '3' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[3];
                // 4
                case '4' when Type == NumberDisplayType.Score:
                case '4' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[4];
                case '4' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[4];
                case '4' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[4];
                // 5
                case '5' when Type == NumberDisplayType.Score:
                case '5' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[5];
                case '5' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[5];
                case '5' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[5];
                // 6
                case '6' when Type == NumberDisplayType.Score:
                case '6' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[6];
                case '6' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[6];
                case '6' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[6];
                // 7
                case '7' when Type == NumberDisplayType.Score:
                case '7' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[7];
                case '7' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[7];
                case '7' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[7];
                // 8
                case '8' when Type == NumberDisplayType.Score:
                case '8' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[8];
                case '8' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[8];
                case '8' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[8];
                // 9
                case '9' when Type == NumberDisplayType.Score:
                case '9' when Type == NumberDisplayType.Accuracy:
                    return GameBase.LoadedSkin.ScoreDisplayNumbers[9];
                case '9' when Type == NumberDisplayType.Combo:
                    return GameBase.LoadedSkin.ComboDisplayNumbers[9];
                case '9' when Type == NumberDisplayType.SongTime:
                    return GameBase.LoadedSkin.SongTimeDisplayNumbers[9];
                case '.':
                    return GameBase.LoadedSkin.ScoreDisplayDecimal;
                case '%':
                    return GameBase.LoadedSkin.ScoreDisplayPercent;
                case '-':
                    return GameBase.LoadedSkin.SongTimeDisplayMinus;
                case ':':
                    return GameBase.LoadedSkin.SongTimeDisplayColon;
                default:
                    throw new ArgumentException($"Invalid character {c} specified.");
            }
        }
    }

    /// <summary>
    ///     Enum that dictates which type of display it is.
    ///     Some textures are used versus others, so specifying the type here
    ///     allows us to use the correct one.
    /// </summary>
    internal enum NumberDisplayType
    {
        Score,
        Accuracy,
        Combo,
        SongTime
    }
}