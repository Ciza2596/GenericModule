using System;
using System.Collections.Generic;
using UnityEngine;

namespace CizaInputModule
{
	public abstract class Icon : IIcon
	{
		private const TextureFormat FORMAT = TextureFormat.RGBA32;

		private const int WIDTH = 64;
		private const int HEIGHT = 64;

		[field: NonSerialized]
		private static readonly Dictionary<int, Texture2D> Cache = new Dictionary<int, Texture2D>();

		// MEMBERS: -------------------------------------------------------------------------------

		[NonSerialized]
		private readonly Color m_Tint;

		[NonSerialized]
		private readonly IIcon m_Overlay;

		protected virtual ColorTheme.Type OverlayColor => ColorTheme.Type.Blue;

		protected virtual byte[] Bytes => null;

		public override int GetHashCode()
		{
			int hashKey = GetType().GetHashCode() ^ m_Tint.GetHashCode();
			if (m_Overlay != null)
				hashKey ^= m_Overlay.GetHashCode() ^ OverlayColor.GetHashCode();

			return hashKey;
		}

		public Texture2D Texture
		{
			get
			{
				var hashKey = GetHashCode();

				if (Cache.TryGetValue(hashKey, out var texture) && texture != null)
					return texture;


				var overlayTexture = m_Overlay?.Texture;
				var overlayColor = ColorTheme.Get(OverlayColor);

				texture = new Texture2D(WIDTH, HEIGHT, FORMAT, false);
				texture.LoadRawTextureData(Bytes);

				for (var i = 0; i < WIDTH; ++i)
					for (var j = 0; j < HEIGHT; ++j)
					{
						var pixel = texture.GetPixel(i, j);
						var color = new Color(pixel.r * m_Tint.r, pixel.g * m_Tint.g, pixel.b * m_Tint.b, pixel.a);

						if (overlayTexture != null)
						{
							var watermark = overlayTexture.GetPixel(i, j);
							color = Color.Lerp(color, overlayColor, 1f - watermark.r);
							color.a *= 1f - watermark.g;
						}

						texture.SetPixel(i, j, color);
					}

				texture.Apply();
				Cache[hashKey] = texture;

				return texture;
			}
		}

		protected Icon(Color color, IIcon overlay)
		{
			m_Tint = color;
			m_Overlay = overlay;
		}
	}
}