using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace EyeOfSauron.ViewModel
{
    internal class ColorToolViewModel : ViewModelBase
    {
        private readonly PaletteHelper _paletteHelper = new();

        private ColorScheme _activeScheme;
        public ColorScheme ActiveScheme
        {
            get => _activeScheme;
            set
            {
                if (_activeScheme != value)
                {
                    _activeScheme = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color? _selectedColor;
        public Color? SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged();

                    // if we are triggering a change internally its a hue change and the colors will match
                    // so we don't want to trigger a custom color change.
                    var currentSchemeColor = ActiveScheme switch
                    {
                        ColorScheme.Primary => _primaryColor,
                        ColorScheme.Secondary => _secondaryColor,
                        ColorScheme.PrimaryForeground => _primaryForegroundColor,
                        ColorScheme.SecondaryForeground => _secondaryForegroundColor,
                        _ => throw new NotSupportedException($"{ActiveScheme} is not a handled ColorScheme.. Ye daft programmer!")
                    };

                    if (_selectedColor != currentSchemeColor && value is Color color)
                    {
                        ChangeCustomColor(color);
                    }
                }
            }
        }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        public ICommand ChangeCustomHueCommand { get; }

        public ICommand ChangeHueCommand { get; }
        public ICommand ChangeToPrimaryCommand { get; }
        public ICommand ChangeToSecondaryCommand { get; }
        public ICommand ChangeToPrimaryForegroundCommand { get; }
        public ICommand ChangeToSecondaryForegroundCommand { get; }

        public ICommand ToggleBaseCommand { get; }

        private void ApplyBase(bool isDark)
        {
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = isDark ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
        }
        
        public ColorToolViewModel()
        {
            ToggleBaseCommand = new CommandImplementation(o => ApplyBase((bool)o));
            ChangeHueCommand = new CommandImplementation(ChangeHue);
            ChangeCustomHueCommand = new CommandImplementation(ChangeCustomColor);
            ChangeToPrimaryCommand = new CommandImplementation(o => ChangeScheme(ColorScheme.Primary));
            ChangeToSecondaryCommand = new CommandImplementation(o => ChangeScheme(ColorScheme.Secondary));
            ChangeToPrimaryForegroundCommand = new CommandImplementation(o => ChangeScheme(ColorScheme.PrimaryForeground));
            ChangeToSecondaryForegroundCommand = new CommandImplementation(o => ChangeScheme(ColorScheme.SecondaryForeground));


            ITheme theme = _paletteHelper.GetTheme();

            _primaryColor = theme.PrimaryMid.Color;
            _secondaryColor = theme.SecondaryMid.Color;

            SelectedColor = _primaryColor;
        }

        private void ChangeCustomColor(object obj)
        {
            var color = (Color)obj;

            if (ActiveScheme == ColorScheme.Primary)
            {
                _paletteHelper.ChangePrimaryColor(color);
                _primaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                _paletteHelper.ChangeSecondaryColor(color);
                _secondaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(color);
                _primaryForegroundColor = color;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(color);
                _secondaryForegroundColor = color;
            }
        }

        private void ChangeScheme(ColorScheme scheme)
        {
            ActiveScheme = scheme;
            if (ActiveScheme == ColorScheme.Primary)
            {
                SelectedColor = _primaryColor;
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                SelectedColor = _secondaryColor;
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SelectedColor = _primaryForegroundColor;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SelectedColor = _secondaryForegroundColor;
            }
        }

        private Color? _primaryColor;

        private Color? _secondaryColor;

        private Color? _primaryForegroundColor;

        private Color? _secondaryForegroundColor;

        private void ChangeHue(object obj)
        {
            var hue = (Color)obj;

            SelectedColor = hue;
            if (ActiveScheme == ColorScheme.Primary)
            {
                _paletteHelper.ChangePrimaryColor(hue);
                _primaryColor = hue;
                _primaryForegroundColor = _paletteHelper.GetTheme().PrimaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                _paletteHelper.ChangeSecondaryColor(hue);
                _secondaryColor = hue;
                _secondaryForegroundColor = _paletteHelper.GetTheme().SecondaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(hue);
                _primaryForegroundColor = hue;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(hue);
                _secondaryForegroundColor = hue;
            }
        }

        private void SetPrimaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, color);
            theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, color);
            theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }

        private void SetSecondaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, color);
            theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, color);
            theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }
    }

    enum ColorScheme
    {
        Primary,
        Secondary,
        PrimaryForeground,
        SecondaryForeground
    }

    public static class PaletteHelperExtensions
    {
        public static void ChangePrimaryColor(this PaletteHelper paletteHelper, Color color)
        {
            ITheme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());

            paletteHelper.SetTheme(theme);
        }

        public static void ChangeSecondaryColor(this PaletteHelper paletteHelper, Color color)
        {
            ITheme theme = paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(color.Lighten());
            theme.SecondaryMid = new ColorPair(color);
            theme.SecondaryDark = new ColorPair(color.Darken());

            paletteHelper.SetTheme(theme);
        }
    }

    public static class ColorAssist
    {
        /// <summary>
        /// The relative brightness of any point in a colorspace, normalized to 0 for darkest black and 1 for lightest white
        /// For the sRGB colorspace, the relative luminance of a color is defined as L = 0.2126 * R + 0.7152 * G + 0.0722 * B where R, G and B are defined as:
        /// if RsRGB <= 0.03928 then R = RsRGB / 12.92 else R = ((RsRGB+0.055)/1.055) ^ 2.4
        /// if GsRGB <= 0.03928 then G = GsRGB / 12.92 else G = ((GsRGB+0.055)/1.055) ^ 2.4
        /// if BsRGB <= 0.03928 then B = BsRGB / 12.92 else B = ((BsRGB+0.055)/1.055) ^ 2.4
        /// and RsRGB, GsRGB, and BsRGB are defined as:
        /// RsRGB = R8bit/255
        /// GsRGB = G8bit/255
        /// BsRGB = B8bit/255
        /// Based on https://www.w3.org/TR/WCAG21/#dfn-relative-luminance
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static float RelativeLuninance(this Color color)
        {
            return
                0.2126f * Calc(color.R / 255f) +
                0.7152f * Calc(color.G / 255f) +
                0.0722f * Calc(color.B / 255f);

            static float Calc(float colorValue)
                => colorValue <= 0.03928f ? colorValue / 12.92f : (float)Math.Pow((colorValue + 0.055f) / 1.055f, 2.4);
        }

        /// <summary>
        /// The contrast ratio is calculated as (L1 + 0.05) / (L2 + 0.05), where
        /// L1 is the: relative luminance of the lighter of the colors, and
        /// L2 is the relative luminance of the darker of the colors.
        /// Based on https://www.w3.org/TR/2008/REC-WCAG20-20081211/#contrast%20ratio
        /// </summary>
        /// <param name="color"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        public static float ContrastRatio(this Color color, Color color2)
        {
            float l1 = color.RelativeLuninance();
            float l2 = color2.RelativeLuninance();
            if (l2 > l1)
            {
                float temp = l1;
                l1 = l2;
                l2 = temp;
            }
            return (l1 + 0.05f) / (l2 + 0.05f);
        }

        /// <summary>
        /// Adjust the foreground color to have an acceptable contrast ratio.
        /// </summary>
        /// <param name="foreground">The foreground color</param>
        /// <param name="background">The background color</param>
        /// <param name="targetRatio">The target contrast ratio</param>
        /// <param name="tollerance">The tollerance to the contrast ratio needs to be within</param>
        /// <returns>The updated foreground color with the target contrast ratio with the background</returns>
        public static Color EnsureContrastRatio(this Color foreground, Color background, float targetRatio, float tollerance = 0.1f)
            => EnsureContrastRatio(foreground, background, targetRatio, out _, tollerance);

        /// <summary>
        /// Adjust the foreground color to have an acceptable contrast ratio.
        /// </summary>
        /// <param name="foreground">The foreground color</param>
        /// <param name="background">The background color</param>
        /// <param name="targetRatio">The target contrast ratio</param>
        /// <param name="offset">The offset that was applied</param>
        /// <param name="tollerance">The tollerance to the contrast ratio needs to be within</param>
        /// <returns>The updated foreground color with the target contrast ratio with the background</returns>
        public static Color EnsureContrastRatio(this Color foreground, Color background, float targetRatio, out double offset, float tollerance = 0.1f)
        {
            offset = 0.0f;
            float ratio = foreground.ContrastRatio(background);
            if (ratio > targetRatio) return foreground;

            var contrastWithWhite = background.ContrastRatio(Colors.White);
            var contrastWithBlack = background.ContrastRatio(Colors.Black);

            var shouldDarken = contrastWithBlack > contrastWithWhite;

            //Lighten is negative
            Color finalColor = foreground;
            double? adjust = null;

            while ((ratio < targetRatio - tollerance || ratio > targetRatio + tollerance) &&
                   finalColor != Colors.White &&
                   finalColor != Colors.Black)
            {
                if (ratio - targetRatio < 0.0)
                {
                    //Move offset of foreground further away from background
                    if (shouldDarken)
                    {
                        if (adjust < 0)
                        {
                            adjust /= -2;
                        }
                        else if (adjust == null)
                        {
                            adjust = 1.0f;
                        }
                    }
                    else
                    {
                        if (adjust > 0)
                        {
                            adjust /= -2;
                        }
                        else if (adjust == null)
                        {
                            adjust = -1.0f;
                        }
                    }
                }
                else
                {
                    //Move offset of foreground closer to background
                    if (shouldDarken)
                    {
                        if (adjust > 0)
                        {
                            adjust /= -2;
                        }
                        else if (adjust == null)
                        {
                            adjust = -1.0f;
                        }

                    }
                    else
                    {
                        if (adjust < 0)
                        {
                            adjust /= -2;
                        }
                        else if (adjust == null)
                        {
                            adjust = 1.0f;
                        }
                    }
                }

                offset += adjust.Value;

                finalColor = foreground.ShiftLightness(offset);

                ratio = finalColor.ContrastRatio(background);
            }
            return finalColor;
        }

        public static Color ContrastingForegroundColor(this Color color)
            => color.IsLightColor() ? Colors.Black : Colors.White;

        public static bool IsLightColor(this Color color)
        {
            double rgb_srgb(double d)
            {
                d /= 255.0;
                return (d > 0.03928)
                    ? Math.Pow((d + 0.055) / 1.055, 2.4)
                    : d / 12.92;
            }
            var r = rgb_srgb(color.R);
            var g = rgb_srgb(color.G);
            var b = rgb_srgb(color.B);

            var luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;
            return luminance > 0.179;
        }

        public static bool IsDarkColor(this Color color) => !IsLightColor(color);

        public static Color ShiftLightness(this Color color, double amount = 1.0f)
        {
            var lab = color.ToLab();
            var shifted = new Lab(lab.L - LabConstants.Kn * amount, lab.A, lab.B);
            return shifted.ToColor();
        }

        public static Color ShiftLightness(this Color color, int amount = 1)
        {
            var lab = color.ToLab();
            var shifted = new Lab(lab.L - LabConstants.Kn * amount, lab.A, lab.B);
            return shifted.ToColor();
        }

        public static Color Darken(this Color color, int amount = 1) => color.ShiftLightness(amount);

        public static Color Lighten(this Color color, int amount = 1) => color.ShiftLightness(-amount);
    }

    internal static class LabConverter
    {
        public static Lab ToLab(this Color c)
        {
            var xyz = c.ToXyz();
            return xyz.ToLab();
        }

        public static Lab ToLab(this Xyz xyz)
        {
            double xyz_lab(double v)
            {
                if (v > LabConstants.e)
                    return Math.Pow(v, 1 / 3.0);
                else
                    return (v * LabConstants.k + 16) / 116;
            }

            var fx = xyz_lab(xyz.X / LabConstants.WhitePointX);
            var fy = xyz_lab(xyz.Y / LabConstants.WhitePointY);
            var fz = xyz_lab(xyz.Z / LabConstants.WhitePointZ);

            var l = 116 * fy - 16;
            var a = 500 * (fx - fy);
            var b = 200 * (fy - fz);
            return new Lab(l, a, b);
        }

        public static Color ToColor(this Lab lab)
        {
            var xyz = lab.ToXyz();

            return xyz.ToColor();
        }
    }

    internal struct Lab
    {
        public double L { get; }
        public double A { get; }
        public double B { get; }

        public Lab(double l, double a, double b)
        {
            L = l;
            A = a;
            B = b;
        }
    }

    internal class LabConstants
    {
        public const double Kn = 18;

        public const double WhitePointX = 0.95047;
        public const double WhitePointY = 1;
        public const double WhitePointZ = 1.08883;

        public static readonly double eCubedRoot = Math.Pow(e, 1.0 / 3);
        public const double k = 24389 / 27.0;
        public const double e = 216 / 24389.0;
    }

    internal static class XyzConverter
    {
        public static Color ToColor(this Xyz xyz)
        {
            double xyz_rgb(double d)
            {
                if (d > 0.0031308)
                    return 255.0 * (1.055 * Math.Pow(d, 1.0 / 2.4) - 0.055);
                else
                    return 255.0 * (12.92 * d);
            }

            byte clip(double d)
            {
                if (d < 0) return 0;
                if (d > 255) return 255;
                return (byte)Math.Round(d);
            }
            var r = xyz_rgb(3.2404542 * xyz.X - 1.5371385 * xyz.Y - 0.4985314 * xyz.Z);
            var g = xyz_rgb(-0.9692660 * xyz.X + 1.8760108 * xyz.Y + 0.0415560 * xyz.Z);
            var b = xyz_rgb(0.0556434 * xyz.X - 0.2040259 * xyz.Y + 1.0572252 * xyz.Z);

            return Color.FromRgb(clip(r), clip(g), clip(b));
        }
        public static Xyz ToXyz(this Color c)
        {
            double rgb_xyz(double v)
            {
                v /= 255;
                if (v > 0.04045)
                    return Math.Pow((v + 0.055) / 1.055, 2.4);
                else
                    return v / 12.92;
            }

            var r = rgb_xyz(c.R);
            var g = rgb_xyz(c.G);
            var b = rgb_xyz(c.B);

            var x = 0.4124564 * r + 0.3575761 * g + 0.1804375 * b;
            var y = 0.2126729 * r + 0.7151522 * g + 0.0721750 * b;
            var z = 0.0193339 * r + 0.1191920 * g + 0.9503041 * b;
            return new Xyz(x, y, z);
        }

        public static Xyz ToXyz(this Lab lab)
        {
            double lab_xyz(double d)
            {
                if (d > LabConstants.eCubedRoot)
                    return d * d * d;
                else
                    return (116 * d - 16) / LabConstants.k;
            }

            var y = (lab.L + 16.0) / 116.0;
            var x = double.IsNaN(lab.A) ? y : y + lab.A / 500.0;
            var z = double.IsNaN(lab.B) ? y : y - lab.B / 200.0;

            y = LabConstants.WhitePointY * lab_xyz(y);
            x = LabConstants.WhitePointX * lab_xyz(x);
            z = LabConstants.WhitePointZ * lab_xyz(z);

            return new Xyz(x, y, z);
        }
    }

    internal struct Xyz
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Xyz(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

}