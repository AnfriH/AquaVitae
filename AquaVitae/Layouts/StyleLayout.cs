using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class StyleLayout
{
    private readonly Dictionary<string, IStyle> _runStyles = [];

    public ICollection<IStyle> Styles => _runStyles.Values;
    
    private void AddStyle(IStyle style)
    {
        if (!_runStyles.TryAdd(style.Id, style))
        {
            throw new ArgumentException($"A style with the Id '{style.Id}' already exists");
        }
    }

    public StyleId<TStyle> RegisterStyle<TStyle>(TStyle style) where TStyle: IStyle
    {
        AddStyle(style);
        return new StyleId<TStyle>(style.Id);
    }
    
    private IStyle GetStyle(string? styleId)
    {
        if (styleId == null) throw new NullReferenceException("StyleId was null");
        
        return !_runStyles.TryGetValue(styleId, out var style) 
            ? throw new KeyNotFoundException($"Style with Id '{styleId}' does not exist") 
            : style;
    }

    public TStyle GetStyle<TStyle>(StyleId<TStyle> id) where TStyle : IStyle
    {
        var style = GetStyle(id.Value);
        
        if (style is not TStyle typedStyle)
        {
            throw new InvalidCastException(
                $"Style with Id '{id.Value}' is of type '{style.GetType()}', not '{typeof(TStyle)}'"
            );
        }
        
        return typedStyle;
    }

    private bool TryGetStyle(string? styleId, out IStyle? style)
    {
        if (styleId != null) return _runStyles.TryGetValue(styleId, out style);
        style = null;
        return false;
    }

    public bool TryGetStyle<TStyle>(StyleId<TStyle> id, out TStyle? style) where TStyle : IStyle
    {
        if (!TryGetStyle(id.Value, out var untypedStyle) || untypedStyle is not TStyle typedStyle)
        {
            style = default;
            return false;
        }

        style = typedStyle;
        return true;
    }
}