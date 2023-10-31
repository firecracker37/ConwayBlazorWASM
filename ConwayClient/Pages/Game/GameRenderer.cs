using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using ConwayClient.Models;  // Assuming GameBoard is under this namespace

namespace ConwayClient.Pages.Game
{
    public class GameRenderer
    {
        // Properties and fields
        private Canvas2DContext _canvasContext;
        private Canvas2DContext _bufferCanvasContext;
        private Canvas2DContext _gridCanvasContext;
        private Canvas2DContext _cellCanvasContext;
        private GameBoard _gameBoard;
        private BECanvasComponent _canvasReference;
        private BECanvasComponent _bufferCanvasReference;
        private BECanvasComponent _gridCanvasReference;
        private BECanvasComponent _cellCanvasReference;


        // Constructor to initialize necessary dependencies (can be expanded as needed)
        public GameRenderer(GameBoard gameBoard,
                    Canvas2DContext canvasContext, BECanvasComponent canvasReference,
                    Canvas2DContext bufferCanvasContext, BECanvasComponent bufferCanvasReference,
                    Canvas2DContext gridCanvasContext, BECanvasComponent gridCanvasReference,
                    Canvas2DContext cellCanvasContext, BECanvasComponent cellCanvasReference)
        {
            _gameBoard = gameBoard;
            _canvasContext = canvasContext;
            _bufferCanvasContext = bufferCanvasContext;
            _gridCanvasContext = gridCanvasContext;
            _cellCanvasContext = cellCanvasContext;
            _canvasReference = canvasReference;
            _bufferCanvasReference = bufferCanvasReference;
            _gridCanvasReference = gridCanvasReference;
            _cellCanvasReference = cellCanvasReference;
        }

        // Existing DrawGridLines method
        public async Task DrawGridLines(int canvasWidth, int canvasHeight, int cellWidth, int cellHeight, string lineColor)
        {
            await _gridCanvasContext.BeginBatchAsync();

            // Set line width and stroke style
            await _gridCanvasContext.SetLineWidthAsync(1);
            await _gridCanvasContext.SetStrokeStyleAsync(lineColor);

            // Drawing horizontal lines with pixel alignment
            for (int y = 0; y <= canvasHeight; y += cellHeight)
            {
                await _gridCanvasContext.BeginPathAsync();
                await _gridCanvasContext.MoveToAsync(0, y + 0.5); // +0.5 for pixel alignment
                await _gridCanvasContext.LineToAsync(canvasWidth, y + 0.5);
                await _gridCanvasContext.StrokeAsync();
            }

            // Drawing vertical lines with pixel alignment
            for (int x = 0; x <= canvasWidth; x += cellWidth)
            {
                await _gridCanvasContext.BeginPathAsync();
                await _gridCanvasContext.MoveToAsync(x + 0.5, 0); // +0.5 for pixel alignment
                await _gridCanvasContext.LineToAsync(x + 0.5, canvasHeight);
                await _gridCanvasContext.StrokeAsync();
            }

            await _gridCanvasContext.EndBatchAsync();
        }

        public async Task UpdateCellCanvas(string cellColor, int cellWidth, int cellHeight)
        {
            try
            {
                if (_gameBoard.StateUpdated)
                {

                    // Check begin batch
                    await _cellCanvasContext.BeginBatchAsync();

                    var bornCells = _gameBoard.GetBornCells();
                    var deadCells = _gameBoard.GetDeadCells();

                    // Draw all newly born cells
                    await _cellCanvasContext.SetFillStyleAsync(cellColor);
                    foreach (var cell in bornCells)
                    {
                        await _cellCanvasContext.FillRectAsync(cell.Col * cellWidth, cell.Row * cellHeight, cellWidth, cellHeight);
                    }

                    // Clear all newly dead cells
                    foreach (var cell in deadCells)
                    {
                        await _cellCanvasContext.ClearRectAsync(cell.Col * cellWidth, cell.Row * cellHeight, cellWidth, cellHeight);
                    }

                    // Check end batch
                    await _cellCanvasContext.EndBatchAsync();

                    _gameBoard.ResetStateUpdatedFlag();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during UpdateCellCanvas method: {ex.Message}");
            }
        }

        public async Task DrawToBufferCanvas(string deadCellColor, int offscreenCanvasSize, bool showGridLines)
        {
            try
            {
                // Fill buffer with dead cell color
                await _bufferCanvasContext.SetFillStyleAsync(deadCellColor);
                await _bufferCanvasContext.FillRectAsync(0, 0, (int)offscreenCanvasSize, (int)offscreenCanvasSize);

                // Overlay live cells from cellCanvasContext onto bufferCanvasContext
                await _bufferCanvasContext.DrawImageAsync(_cellCanvasReference.CanvasReference, 0, 0);

                // Overlay grid lines if enabled
                if (showGridLines)
                    await _bufferCanvasContext.DrawImageAsync(_gridCanvasReference.CanvasReference, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during DrawToBufferCanvas method: {ex.Message}");
            }
        }

        public async Task CompositeCanvases(double panX, double panY, double zoomLevel, int canvasWidth, int canvasHeight, int offscreenCanvasSize, string gridLineColor)
        {
            try
            {
                // Calculate source rectangle
                double sourceX = panX;
                double sourceY = panY;
                double sourceWidth = canvasWidth / zoomLevel;
                double sourceHeight = canvasHeight / zoomLevel;

                // Calculate destination rectangle
                double destX = 0;
                double destY = 0;
                double destWidth = canvasWidth;
                double destHeight = canvasHeight;

                await _canvasContext.ClearRectAsync(0, 0, canvasWidth, canvasHeight);
                await _canvasContext.DrawImageAsync(
                    _bufferCanvasReference.CanvasReference,
                    sourceX, sourceY, sourceWidth, sourceHeight, // source rectangle
                    destX, destY, destWidth, destHeight          // destination rectangle
                );

                if (zoomLevel > 1)
                {
                    await RenderMinimap(panX, panY, zoomLevel, canvasWidth, canvasHeight, offscreenCanvasSize, gridLineColor);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing image: {ex.Message}");
            }
        }

        public async Task RenderMinimap(double panX, double panY, double zoomLevel, int canvasWidth, int canvasHeight, double offscreenCanvasSize, string gridLineColor)
        {
            // Constants for the minimap
            double minimapSize = 100; // or another suitable size
            double minimapPadding = 10; // space from the corner
            double minimapX = canvasWidth - minimapSize - minimapPadding;
            double minimapY = canvasHeight - minimapSize - minimapPadding;

            // Draw the entire buffer canvas shrunk down to the minimap size
            await _canvasContext.DrawImageAsync(
                _bufferCanvasReference.CanvasReference,
                0, 0, offscreenCanvasSize, offscreenCanvasSize,
                minimapX, minimapY, minimapSize, minimapSize
            );

            // Calculate the viewport rectangle on the minimap
            double viewportWidth = minimapSize / zoomLevel;
            double viewportHeight = minimapSize / zoomLevel;
            double viewportX = minimapX + (panX / offscreenCanvasSize) * minimapSize;
            double viewportY = minimapY + (panY / offscreenCanvasSize) * minimapSize;

            // Draw the viewport rectangle
            await _canvasContext.SetStrokeStyleAsync("red"); // or another noticeable color
            await _canvasContext.SetLineWidthAsync(2);
            await _canvasContext.StrokeRectAsync(viewportX, viewportY, viewportWidth, viewportHeight);

            // Draw the border around the minimap
            await _canvasContext.SetStrokeStyleAsync(gridLineColor);
            await _canvasContext.StrokeRectAsync(minimapX, minimapY, minimapSize, minimapSize);
        }

        public async Task OnAliveCellColorChangedAsync(string aliveCellColor, int cellWidth, int cellHeight)
        {
            try
            {
                var aliveCells = _gameBoard.GetAliveCells();

                // Begin drawing on the canvas
                await _cellCanvasContext.BeginBatchAsync();

                // Set the new fill style
                await _cellCanvasContext.SetFillStyleAsync(aliveCellColor); // Alive cell color

                // Draw over all alive cells with the new color
                foreach (var cell in aliveCells)
                {
                    await _cellCanvasContext.FillRectAsync(cell.Col * cellWidth, cell.Row * cellHeight, cellWidth, cellHeight);
                }

                // End drawing on the canvas
                await _cellCanvasContext.EndBatchAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during OnAliveCellColorChangedAsync method: {ex.Message}");
            }
        }

        // Additional rendering-related methods can be added as needed
    }
}