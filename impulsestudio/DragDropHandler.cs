using CefSharp;
using CefSharp.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Rocket.features
{
    public class DragDropHandler : IDragHandler
    {
        public Region draggableRegion = new Region();
        public event Action<Region> RegionsChanged;

        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            return false;
        }

        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions)
        {
            if (browser.IsPopup == false)
            {
                draggableRegion = null;
                if (regions.Count > 0)
                {
                    foreach (var region in regions)
                    {
                        var rect = new Rectangle(region.X, region.Y, region.Width, region.Height);

                        if (draggableRegion == null)
                        {
                            draggableRegion = new Region(rect);
                        }
                        else
                        {
                            if (region.Draggable)
                            {
                                draggableRegion.Union(rect);
                            }
                            else
                            {
                                //In the scenario where we have an outer region, that is draggable and it has
                                // an inner region that's not, we must exclude the non draggable.
                                // Not all scenarios are covered in this example.
                                draggableRegion.Exclude(rect);
                            }
                        }
                    }
                }

                RegionsChanged?.Invoke(draggableRegion);
            }
        }

        public void Dispose()
        {
            RegionsChanged = null;
        }

        /*public bool OnDragEnter(IWebBrowser chromiumWebBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            throw new NotImplementedException();
        }*/

        public void OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
        {
            throw new NotImplementedException();
        }
    }
}
