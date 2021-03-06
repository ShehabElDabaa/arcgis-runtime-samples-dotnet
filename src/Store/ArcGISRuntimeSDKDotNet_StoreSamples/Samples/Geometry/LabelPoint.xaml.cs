﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates use of the GeometryEngine.LabelPoint method to calculate the location of label points.
    /// </summary>
    /// <title>Label Point</title>
    /// <category>Geometry</category>
    public partial class LabelPoint : Windows.UI.Xaml.Controls.Page
    {
        private PictureMarkerSymbol _pictureMarkerSymbol;
        private GraphicsOverlay _labelOverlay;

        /// <summary>Construct Label Point sample control</summary>
        public LabelPoint()
        {
            InitializeComponent();

			_labelOverlay = MyMapView.GraphicsOverlays["labelGraphicOverlay"];

            MyMapView.ExtentChanged += MyMapView_ExtentChanged;
			SetupSymbols();
        }

        // Load the picture symbol image
        private async void SetupSymbols()
        {
            try
            {
                _pictureMarkerSymbol = LayoutRoot.Resources["PictureMarkerSymbol"] as PictureMarkerSymbol;
                await _pictureMarkerSymbol.SetSourceAsync(new Uri("ms-appx:///Assets/x-24x24.png"));
            }
            catch (Exception ex)
            {
				var _x = new MessageDialog("Error occured : " + ex.Message, "Boundary Sample").ShowAsync();
            }
        }

        // Start accepting user polygons and calculating label points
        private async void MyMapView_ExtentChanged(object sender, EventArgs e)
        {
            MyMapView.ExtentChanged -= MyMapView_ExtentChanged;
            await CalculateLabelPointsAsync();
        }

        // Continuosly accept polygons from the user and calculate label points
        private async Task CalculateLabelPointsAsync()
        {
            try
            {
                await MyMapView.LayersLoadedAsync();

                while (MyMapView.Extent != null)
                {
                    if (MyMapView.Editor.IsActive)
                        MyMapView.Editor.Cancel.Execute(null);

                    //Get the input polygon geometry from the user
                    var poly = await MyMapView.Editor.RequestShapeAsync(DrawShape.Polygon, ((SimpleRenderer)_labelOverlay.Renderer).Symbol);
                    if (poly != null)
                    {
                        //Add the polygon drawn by the user
                        _labelOverlay.Graphics.Add(new Graphic(poly));

                        //Get the label point for the input geometry
                        var labelPoint = GeometryEngine.LabelPoint(poly);
                        if (labelPoint != null)
                        {
                            _labelOverlay.Graphics.Add(new Graphic(labelPoint, _pictureMarkerSymbol));
                        }
                    }
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                var _x = new MessageDialog("Label Point Error: " + ex.Message, "Sample Error").ShowAsync();
            }
        }

        // Clear label graphics and restart calculating label points
        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _labelOverlay.Graphics.Clear();
            await CalculateLabelPointsAsync();
        }
    }
}
