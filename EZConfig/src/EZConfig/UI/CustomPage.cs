﻿using UnityEngine;
using UnityEngine.UI;

namespace EZConfig.UI;

#nullable disable
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class CustomPage : MenuWindow
{
    private Canvas Canvas { get; set; }
    private CanvasScaler Scaler { get; set; }

    private Image Background { get; set; }

    public override bool openOnStart => false;

    public override bool selectOnOpen => true;

    public override bool closeOnPause => true;

    public override bool closeOnUICancel => true;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3 | AdditionalCanvasShaderChannels.Tangent;

        Canvas.sortingOrder = 1;

        Scaler = GetComponent<CanvasScaler>();
        Scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        Scaler.referenceResolution = new Vector2(1920, 1080);
        Scaler.matchWidthOrHeight = 1;
    }

    public static readonly Color DEFAULT_BACKGROUND_COLOR = new(0, 0, 0, 0.9569f);
    public CustomPage CreateBackground(Color? backgroundColor = null)
    {
        if (Background == null)
        {
            var newBackground = new GameObject("Background", typeof(CanvasRenderer), typeof(Image));
            newBackground.transform.SetParent(transform, false);

            Background = newBackground.GetComponent<Image>();

            var backgroundTransform = Background.rectTransform;
            backgroundTransform.anchorMin = Vector2.zero;
            backgroundTransform.anchorMax = Vector2.one;
            backgroundTransform.offsetMin = backgroundTransform.offsetMax = Vector2.zero;

            if (!backgroundColor.HasValue)
                backgroundColor = DEFAULT_BACKGROUND_COLOR;

            Background.color = backgroundColor.Value;
        }

        return this;
    }
}