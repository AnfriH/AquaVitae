using System.Collections.Immutable;
using AquaVitae.Layouts;
using AquaVitae.Layouts.Grids;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorGridRenderer
{
    public static readonly PrintPoint PageBuffer = PrintPoint.FromPoints(1);
    
    private readonly CompiledGridLayout _gridLayout;
    private readonly Graphics _graphics;
    private readonly LayoutConstraints _constraints;
    private readonly VectorElementRenderer _elementRenderer;
    private readonly PageOverflowBehaviour _overflowBehaviour;
    private readonly ChildrenMetadata[] _childrenMetadata;

    public VectorGridRenderer(
        CompiledGridLayout gridLayout, 
        Graphics graphics, 
        LayoutConstraints constraints,
        VectorElementRenderer elementRenderer,
        PageOverflowBehaviour overflowBehaviour = PageOverflowBehaviour.Truncate
    )
    {
        _gridLayout = gridLayout;
        _graphics = graphics;
        _constraints = constraints;
        _elementRenderer = elementRenderer;
        _overflowBehaviour = overflowBehaviour;

        var childrenCount = _gridLayout.ChildIndices.Count;
        _childrenMetadata = new ChildrenMetadata[childrenCount];

        for (var i = 0; i < childrenCount; i++)
        {
            _childrenMetadata[i] = new ChildrenMetadata(_gridLayout.ChildIndices[i], _gridLayout.ParentCounts[i]);
        }
    }

    public PrintPoint RenderGrid()
    {
        var frontier = new Queue<(int ElementIndex, PrintPoint YPosition)>();
        frontier.EnsureCapacity(_gridLayout.RootIndices.Length);
        foreach (var element in _gridLayout.RootIndices) 
        {
            frontier.Enqueue((element, 0));
        }

        var maxYPosition = PrintPoint.Zero;

        while (frontier.Count > 0)
        {
            var (index, yPosition) = frontier.Dequeue();
            
            var element = _gridLayout.Elements[index];

            // If the element fails to render, we skip this element
            if (!TryRenderElement(element, yPosition, out var elementYPosition)) continue;
            
            maxYPosition = maxYPosition.Max(elementYPosition);
            
            var childrenIndex = element.ChildrenIndex;
            if (childrenIndex == -1) continue;
            
            ref var metadata = ref _childrenMetadata[childrenIndex];
            if (!metadata.AddYPosition(elementYPosition)) continue;
            
            // Once all the parents have been visited, we queue the children
            foreach (var childIndex in metadata.ChildIndices)
            {
                frontier.Enqueue((childIndex, metadata.YPosition));
            }
        }

        return maxYPosition;
    }

    private struct ChildrenMetadata(ImmutableArray<int> childIndices, int remainingParents)
    {
        private int _remainingParents = remainingParents;
        public ImmutableArray<int> ChildIndices => childIndices;
        public PrintPoint YPosition { get; private set; }

        // Returns true if all parents have been added
        public bool AddYPosition(PrintPoint yPosition)
        {
            YPosition = YPosition.Max(yPosition);
            _remainingParents--;

            return _remainingParents == 0;
        }
    }
    
    /// <summary>
    /// Tries to render the given grid element if there is room to do so.
    /// Returns true if the element is rendered, and false if not.
    /// </summary>
    /// <param name="element">The grid element to be rendered</param>
    /// <param name="yPosition">The Y position to render the element at</param>
    /// <param name="elementYPosition">The Y position at the base of the rendered element</param>
    /// <returns>Whether the element rendered successfully</returns>
    private bool TryRenderElement(CompiledGridElement element, PrintPoint yPosition, out PrintPoint elementYPosition)
    {
        if (_overflowBehaviour == PageOverflowBehaviour.Truncate && yPosition >= _constraints.Height)
        {
            elementYPosition = yPosition;
            return false;
        }
        
        var remainingHeight = _constraints.Height - yPosition.Points % _constraints.Height.Points;
        
        var xPosition = element.XFraction * _constraints.Width;
        var width = element.WidthFraction * _constraints.Width;
        
        var elementGraphics = new Graphics();
        
        var renderedHeight = _elementRenderer.RenderElement(
            element.Element,
            elementGraphics,
            new LayoutConstraints(0, 0, width, element.MaximumHeight)
        );
        
        // Crop the element to the available space
        var height = renderedHeight.Max(element.MinimumHeight);
        if (_overflowBehaviour == PageOverflowBehaviour.Truncate)
        {
            height = height.Min(remainingHeight);
        }
        
        elementGraphics.Crop(new Rectangle(0, 0, width.Points, height.Points));

        if (_overflowBehaviour == PageOverflowBehaviour.PageFit)
        {
            // If the element is too tall to fit on the current page and isn't taller than a whole page,
            // shift it down to the next page
            if (height > remainingHeight && height < _constraints.Height)
            {
                yPosition += remainingHeight + PageBuffer;
            }
        }
        
        // Draw the element to the backing canvas
        _graphics.DrawGraphics(
            _constraints.X.Points + xPosition.Points, 
            _constraints.Y.Points + yPosition.Points,
            elementGraphics
        );
        
        // Set the new y-position for the following element
        elementYPosition = yPosition + height;
        return true;
    }
}