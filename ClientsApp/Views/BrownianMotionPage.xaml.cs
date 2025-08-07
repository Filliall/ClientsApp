using ClientsApp.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.ComponentModel;

namespace ClientsApp.Views;

public partial class BrownianMotionPage : ContentPage
{
    private readonly BrownianMotionViewModel _viewModel;

    public BrownianMotionPage(BrownianMotionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // Se os dados ou qualquer opção visual mudar, invalida o canvas para forçar um redesenho.
        if (e.PropertyName == nameof(BrownianMotionViewModel.LastSimulationData) ||
            e.PropertyName == nameof(BrownianMotionViewModel.SelectedColor) ||
            e.PropertyName == nameof(BrownianMotionViewModel.IsLineFilled))
        {
            canvasView.InvalidateSurface();
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKImageInfo info = e.Info;
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();

        var allData = _viewModel.LastSimulationData;
        if (allData == null || !allData.Any())
        {
            return;
        }

        // Filtra valores inválidos para evitar que o gráfico quebre.
        var validSimulations = allData
            .Where(arr => arr != null && arr.Length >= 2 && arr.All(d => !double.IsNaN(d) && !double.IsInfinity(d)))
            .ToList();

        if (!validSimulations.Any())
        {
            return;
        }


        var allValues = validSimulations.SelectMany(arr => arr);
        var trueMinVal = (float)allValues.Min();
        var trueMaxVal = (float)allValues.Max();


        var range = trueMaxVal - trueMinVal;
        if (range == 0) range = 1;
        var marginVertical = range * 0.05f;
        var minVal = trueMinVal - marginVertical;
        var maxVal = trueMaxVal + marginVertical;
        range = maxVal - minVal;

        float topMargin = 40;
        float bottomMargin = 40;
        float leftMargin = 250;
        float rightMargin = 40;
        float drawingWidth = info.Width - leftMargin - rightMargin;
        float drawingHeight = info.Height - topMargin - bottomMargin;

        // Desenha os componentes do gráfico
        DrawYAxis(canvas, trueMinVal, trueMaxVal, info, leftMargin, topMargin, drawingHeight);
        DrawSimulationPaths(canvas, validSimulations, minVal, range, info, leftMargin, topMargin, drawingWidth, drawingHeight);
        DrawXAxis(canvas, validSimulations, minVal, range, info, leftMargin, rightMargin, bottomMargin, topMargin, drawingWidth, drawingHeight);
    }

    private void DrawYAxis(SKCanvas canvas, float trueMinVal, float trueMaxVal, SKImageInfo info, float leftMargin, float topMargin, float drawingHeight)
    {
        using var axisPaint = new SKPaint
        {
            Color = SKColors.DarkGray,
            StrokeWidth = 2,
            IsAntialias = true
        };

        // Linha vertical do eixo Y
        canvas.DrawLine(leftMargin, topMargin, leftMargin, topMargin + drawingHeight, axisPaint);


        using var font = new SKFont { Size = 28f };
        using var textPaint = new SKPaint(font)
        {
            Color = SKColors.Gray,
            IsAntialias = true,
        };

        // Rótulo do valor máximo
        var maxLabel = trueMaxVal.ToString("C");
        float maxTextWidth = textPaint.MeasureText(maxLabel);
        canvas.DrawText(maxLabel, leftMargin - 15 - maxTextWidth, topMargin + (font.Size / 2), textPaint);

        // Rótulo do valor mínimo
        var minLabel = trueMinVal.ToString("C");
        float minTextWidth = textPaint.MeasureText(minLabel);
        canvas.DrawText(minLabel, leftMargin - 15 - minTextWidth, topMargin + drawingHeight, textPaint);
    }

    private void DrawSimulationPaths(SKCanvas canvas, List<double[]> validSimulations, float minVal, float range, SKImageInfo info, float leftMargin, float topMargin, float drawingWidth, float drawingHeight)
    {
        // Paleta de cores para as linhas do gráfico
        var colorPalette = new SKColor[]
        {
            SKColor.Parse("#00FFFF"), SKColor.Parse("#FF69B4"), SKColor.Parse("#00FF00"),
            SKColor.Parse("#FFFF00"), SKColor.Parse("#FFA500"), SKColor.Parse("#FF00FF"),
            SKColor.Parse("#1E90FF"), SKColor.Parse("#ADFF2F"), SKColor.Parse("#DDA0DD"),
            SKColor.Parse("#F08080")
        };

        // Desenha cada simulação
        for (int simIndex = 0; simIndex < validSimulations.Count; simIndex++)
        {
            var currentData = validSimulations[simIndex];
            var lineColor = validSimulations.Count > 1
                ? colorPalette[simIndex % colorPalette.Length]
                : SKColor.Parse(_viewModel.SelectedColor.HexValue);

            var points = currentData.Select((value, index) => new SKPoint(
                leftMargin + (index / (float)(currentData.Length - 1)) * drawingWidth,
                topMargin + drawingHeight - (((float)value - minVal) / range) * drawingHeight
            )).ToArray();

            DrawSinglePath(canvas, points, lineColor, _viewModel.IsLineFilled, topMargin, drawingHeight);
        }
    }

    private void DrawSinglePath(SKCanvas canvas, SKPoint[] points, SKColor color, bool isFilled, float topMargin, float drawingHeight)
    {
        using var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = 3.5f,
            IsAntialias = true
        };

        var linePath = new SKPath();
        linePath.MoveTo(points[0]);
        for (int i = 1; i < points.Length; i++)
        {
            linePath.LineTo(points[i]);
        }
        canvas.DrawPath(linePath, linePaint);

        if (isFilled)
        {
            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color.WithAlpha(30),
                IsAntialias = true
            };

            var fillPath = new SKPath();
            fillPath.AddPath(linePath);
            fillPath.LineTo(points.Last().X, topMargin + drawingHeight);
            fillPath.LineTo(points.First().X, topMargin + drawingHeight);
            fillPath.Close();
            canvas.DrawPath(fillPath, fillPaint);
        }
    }

    private void DrawXAxis(SKCanvas canvas, List<double[]> validSimulations, float minVal, float range, SKImageInfo info, float leftMargin, float rightMargin, float bottomMargin, float topMargin, float drawingWidth, float drawingHeight)
    {

        using var font = new SKFont { Size = 28f };
        using var textPaint = new SKPaint(font)
        {
            Color = SKColors.Gray,
            IsAntialias = true,
        };

        int days = validSimulations.First().Length - 1;
        const int monthInterval = 30;
        for (int i = 0; i <= days; i += monthInterval)
        {
            var label = i.ToString();
            float xPos = leftMargin + (i / (float)days) * drawingWidth;
            float textWidth = textPaint.MeasureText(label);
            canvas.DrawText(label, xPos - (textWidth / 2), info.Height - (bottomMargin / 4), textPaint);
        }
        if (days > 0 && days % monthInterval != 0)
        {
            var label = days.ToString();
            float xPos = leftMargin + drawingWidth;
            float textWidth = textPaint.MeasureText(label);
            canvas.DrawText(label, xPos - (textWidth / 2), info.Height - (bottomMargin / 4), textPaint);
        }
    }
}