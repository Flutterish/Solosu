using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace osu.Game.Rulesets.Solosu {
	public static class SolosuTextures {
		public static Texture Generate ( IRenderer renderer, int width, int height, Func<int, int, Rgba32> generator ) {
			Image<Rgba32> image = new Image<Rgba32>( width, height );
			image.ProcessPixelRows( rows => {
				for ( int y = 0; y < height; y++ ) {
					var span = rows.GetRowSpan( y );
					for ( int x = 0; x < width; x++ ) {
						span[ x ] = generator( x, y );
					}
				}		
			} );
			
			Texture texture = renderer.CreateTexture( width, height, true );
			texture.SetData( new TextureUpload( image ) );
			return texture;
		}
		public static Texture GeneratePercentile ( IRenderer renderer, int width, int height, Func<double, double, Rgba32> generator )
			=> Generate( renderer, width, height, ( x, y ) => generator( (double)x / width, (double)y / height ) );
		public static Texture GenerateMirroredPercentile ( IRenderer renderer, int width, int height, Func<double, double, Rgba32> generator )
			=> Generate( renderer, width, height, ( x, y ) => generator( 1 - Math.Abs( 1 - x * 2d / width ), 1 - Math.Abs( 1 - y * 2d / height ) ) );
		public static Texture WidthFade ( IRenderer renderer, int width, int height )
			=> Generate( renderer, width, height, ( x, y ) => new Rgba32( 255, 255, 255, (byte)( 255 - MathF.Abs( 1 - 2f * x / width ) * 255 ) ) );

		public static Texture FadeLeft ( IRenderer renderer, int width, int height )
			=> Generate( renderer, width, height, ( x, y ) => new Rgba32( 255, 255, 255, (byte)( 255 * ( width - x ) / width ) ) );

		static Texture cachedFadeLeft;
		public static Texture CachedFadeLeft ( IRenderer renderer ) => cachedFadeLeft ??= FadeLeft( renderer, 100, 100 );
	}
}
