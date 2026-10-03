namespace AquaVitae.Layouts.Abstractions;

public closed class ElementLayoutBase : LayoutBase
{
    /// <summary>
    /// Visits all paragraphs in the layout and passes them to the callback.
    /// This is primarily used for rendering docx files, where "decorations"
    /// are ignored and the raw paragraphs are inserted into the document.
    /// </summary>
    public abstract void CollectParagraphs(Action<ParagraphLayoutBase> callback);
}