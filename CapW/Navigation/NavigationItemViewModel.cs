namespace CapW.Navigation;

public class NavigationItemViewModel
{
    public NavigationItemViewModel(string name, Type pageType, string? glyph = default, string? glyphFont = default, IReadOnlyList<NavigationItemViewModel>? children = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(pageType);

        Name = name;
        PageType = pageType;
        Glyph = glyph;
        GlyphFont = glyphFont;
        Children = children;
    }

    public required Type PageType { get; init; }
    public required string Name { get; init; }
    public string? Glyph { get; init; }
    public string? GlyphFont { get; init; }
    public IReadOnlyList<NavigationItemViewModel>? Children { get; init; }
}

[method: SetsRequiredMembers]
public class NavigationItemViewModel<TPage>(string name, string? glyph = default, string? glyphFont = default, IReadOnlyList<NavigationItemViewModel>? children = default)
    : NavigationItemViewModel(name, typeof(TPage), glyph, glyphFont, children)
{
}

