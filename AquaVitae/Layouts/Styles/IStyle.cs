namespace AquaVitae.Layouts.Styles;

public interface IStyle
{
    string Name { get; }
    string Id { get; }
}

public interface IStyle<TLayout> : IStyle where TLayout : ILayout;