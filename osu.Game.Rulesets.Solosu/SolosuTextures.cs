﻿using osu.Framework.Graphics.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace osu.Game.Rulesets.Solosu {
	public static class SolosuTextures {
		public static Texture Generate ( int width, int height, Func<int, int, Rgba32> generator ) {
			Image<Rgba32> image = new Image<Rgba32>( width, height );
			for ( int y = 0; y < height; y++ ) {
				var span = image.GetPixelRowSpan( y );
				for ( int x = 0; x < width; x++ ) {
					span[ x ] = generator( x, y );
				}
			}
			Texture texture = new Texture( width, height, true );
			texture.SetData( new TextureUpload( image ) );
			return texture;
		}
		public static Texture WidthFade ( int width, int height )
			=> Generate( width, height, ( x, y ) => new Rgba32( 255, 255, 255, (byte)( 255 - MathF.Abs( 1 - 2f * x / width ) * 255 ) ) );
	}
}