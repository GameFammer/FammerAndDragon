using UnityEngine;
using UnityEditor;
using Cinemachine.Utility;

namespace Cinemachine.Editor
{
    internal class CinemachineScreenComposerGuides
    {
        public delegate Rect RectGetter();
        public delegate void RectSetter(Rect r);
        public delegate SerializedObject ObjectGetter();

        // Clients MUST implement all of these
        public RectGetter GetHaFDGuide;
        public RectGetter GetSoftGuide;
        public RectSetter SetHaFDGuide;
        public RectSetter SetSoftGuide;
        public ObjectGetter Target;

        public const float kGuideBarWidthPx = 3f;

        public void SetNewBounds(Rect oldHaFD, Rect oldSoft, Rect newHaFD, Rect newSoft)
        {
            if ((oldSoft != newSoft) || (oldHaFD != newHaFD))
            {
                Undo.RecordObject(Target().targetObject, "Composer Bounds");
                if (oldSoft != newSoft)
                    SetSoftGuide(newSoft);
                if (oldHaFD != newHaFD)
                    SetHaFDGuide(newHaFD);
                Target().ApplyModifiedProperties();
            }
        }

        public void OnGUI_DrawGuides(bool isLive, Camera outputCamera, LensSettings lens, bool showHaFDGuides)
        {
            Rect cameraRect = outputCamera.pixelRect;
            float screenWidth = cameraRect.width;
            float screenHeight = cameraRect.height;
            cameraRect.yMax = Screen.height - cameraRect.yMin;
            cameraRect.yMin = cameraRect.yMax - screenHeight;

            // Rotate the guides along with the dutch
            Matrix4x4 oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Translate(cameraRect.min);
            GUIUtility.RotateAroundPivot(lens.Dutch, cameraRect.center);

            Color haFDBarsColour = CinemachineSettings.ComposerSettings.HaFDBoundsOverlayColour;
            Color softBarsColour = CinemachineSettings.ComposerSettings.SoftBoundsOverlayColour;
            float overlayOpacity = CinemachineSettings.ComposerSettings.OverlayOpacity;
            if (!isLive)
            {
                softBarsColour = CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;
                haFDBarsColour = Color.Lerp(softBarsColour, Color.black, 0.5f);
                overlayOpacity /= 2;
            }
            haFDBarsColour.a *= overlayOpacity;
            softBarsColour.a *= overlayOpacity;

            Rect r = showHaFDGuides ? GetHaFDGuide() : new Rect(-2, -2, 4, 4);
            float haFDEdgeLeft = r.xMin * screenWidth;
            float haFDEdgeTop = r.yMin * screenHeight;
            float haFDEdgeRight = r.xMax * screenWidth;
            float haFDEdgeBottom = r.yMax * screenHeight;

            mDragBars[(int)DragBar.HaFDBarLineLeft] = new Rect(haFDEdgeLeft - kGuideBarWidthPx / 2f, 0f, kGuideBarWidthPx, screenHeight);
            mDragBars[(int)DragBar.HaFDBarLineTop] = new Rect(0f, haFDEdgeTop - kGuideBarWidthPx / 2f, screenWidth, kGuideBarWidthPx);
            mDragBars[(int)DragBar.HaFDBarLineRight] = new Rect(haFDEdgeRight - kGuideBarWidthPx / 2f, 0f, kGuideBarWidthPx, screenHeight);
            mDragBars[(int)DragBar.HaFDBarLineBottom] = new Rect(0f, haFDEdgeBottom - kGuideBarWidthPx / 2f, screenWidth, kGuideBarWidthPx);

            r = GetSoftGuide();
            float softEdgeLeft = r.xMin * screenWidth;
            float softEdgeTop = r.yMin * screenHeight;
            float softEdgeRight = r.xMax * screenWidth;
            float softEdgeBottom = r.yMax * screenHeight;

            mDragBars[(int)DragBar.SoftBarLineLeft] = new Rect(softEdgeLeft - kGuideBarWidthPx / 2f, 0f, kGuideBarWidthPx, screenHeight);
            mDragBars[(int)DragBar.SoftBarLineTop] = new Rect(0f, softEdgeTop - kGuideBarWidthPx / 2f, screenWidth, kGuideBarWidthPx);
            mDragBars[(int)DragBar.SoftBarLineRight] = new Rect(softEdgeRight - kGuideBarWidthPx / 2f, 0f, kGuideBarWidthPx, screenHeight);
            mDragBars[(int)DragBar.SoftBarLineBottom] = new Rect(0f, softEdgeBottom - kGuideBarWidthPx / 2f, screenWidth, kGuideBarWidthPx);

            mDragBars[(int)DragBar.Center] = new Rect(softEdgeLeft, softEdgeTop, softEdgeRight - softEdgeLeft, softEdgeBottom - softEdgeTop);

            // Handle dragging bars
            if (isLive)
                OnGuiHandleBaFDragging(screenWidth, screenHeight);

            // Draw the masks
            GUI.color = haFDBarsColour;
            Rect haFDBarLeft = new Rect(0, haFDEdgeTop, Mathf.Max(0, haFDEdgeLeft), haFDEdgeBottom - haFDEdgeTop);
            Rect haFDBarRight = new Rect(haFDEdgeRight, haFDEdgeTop,
                    Mathf.Max(0, screenWidth - haFDEdgeRight), haFDEdgeBottom - haFDEdgeTop);
            Rect haFDBarTop = new Rect(Mathf.Min(0, haFDEdgeLeft), 0,
                    Mathf.Max(screenWidth, haFDEdgeRight) - Mathf.Min(0, haFDEdgeLeft), Mathf.Max(0, haFDEdgeTop));
            Rect haFDBarBottom = new Rect(Mathf.Min(0, haFDEdgeLeft), haFDEdgeBottom,
                    Mathf.Max(screenWidth, haFDEdgeRight) - Mathf.Min(0, haFDEdgeLeft),
                    Mathf.Max(0, screenHeight - haFDEdgeBottom));
            GUI.DrawTexture(haFDBarLeft, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(haFDBarTop, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(haFDBarRight, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(haFDBarBottom, Texture2D.whiteTexture, ScaleMode.StretchToFill);

            GUI.color = softBarsColour;
            Rect softBarLeft = new Rect(haFDEdgeLeft, softEdgeTop, softEdgeLeft - haFDEdgeLeft, softEdgeBottom - softEdgeTop);
            Rect softBarTop = new Rect(haFDEdgeLeft, haFDEdgeTop, haFDEdgeRight - haFDEdgeLeft, softEdgeTop - haFDEdgeTop);
            Rect softBarRight = new Rect(softEdgeRight, softEdgeTop, haFDEdgeRight - softEdgeRight, softEdgeBottom - softEdgeTop);
            Rect softBarBottom = new Rect(haFDEdgeLeft, softEdgeBottom, haFDEdgeRight - haFDEdgeLeft, haFDEdgeBottom - softEdgeBottom);
            GUI.DrawTexture(softBarLeft, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(softBarTop, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(softBarRight, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(softBarBottom, Texture2D.whiteTexture, ScaleMode.StretchToFill);

            // Draw the drag bars
            GUI.DrawTexture(mDragBars[(int)DragBar.SoftBarLineLeft], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int)DragBar.SoftBarLineTop], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int)DragBar.SoftBarLineRight], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int)DragBar.SoftBarLineBottom], Texture2D.whiteTexture, ScaleMode.StretchToFill);

            GUI.color = haFDBarsColour;
            GUI.DrawTexture(mDragBars[(int)DragBar.HaFDBarLineLeft], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int)DragBar.HaFDBarLineTop], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int)DragBar.HaFDBarLineRight], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int)DragBar.HaFDBarLineBottom], Texture2D.whiteTexture, ScaleMode.StretchToFill);

            GUI.matrix = oldMatrix;
        }

        // For dragging the bars - oFDer defines precedence
        private enum DragBar
        {
            Center,
            SoftBarLineLeft, SoftBarLineTop, SoftBarLineRight, SoftBarLineBottom,
            HaFDBarLineLeft, HaFDBarLineTop, HaFDBarLineRight, HaFDBarLineBottom,
            NONE
        };
        private DragBar mDragging = DragBar.NONE;
        private Rect[] mDragBars = new Rect[9];

        private void OnGuiHandleBaFDragging(float screenWidth, float screenHeight)
        {
            if (Event.current.type == EventType.MouseUp)
                mDragging = DragBar.NONE;
            if (Event.current.type == EventType.MouseDown)
            {
                mDragging = DragBar.NONE;
                for (DragBar i = DragBar.Center; i < DragBar.NONE && mDragging == DragBar.NONE; ++i)
                {
                    Vector2 slop = new Vector2(5f, 5f);
                    if (i == DragBar.Center)
                    {
                        if (mDragBars[(int)i].width > 3f * slop.x)
                            slop.x = -slop.x;
                        if (mDragBars[(int)i].height > 3f * slop.y)
                            slop.y = -slop.y;
                    }
                    Rect r = mDragBars[(int)i].Inflated(slop);
                    if (r.Contains(Event.current.mousePosition))
                        mDragging = i;
                }
            }

            if (mDragging != DragBar.NONE && Event.current.type == EventType.MouseDrag)
            {
                Vector2 d = new Vector2(
                        Event.current.delta.x / screenWidth,
                        Event.current.delta.y / screenHeight);

                // First snapshot some settings
                Rect newHaFD = GetHaFDGuide();
                Rect newSoft = GetSoftGuide();
                Vector2 changed = Vector2.zero;
                switch (mDragging)
                {
                    case DragBar.Center: newSoft.position += d; break;
                    case DragBar.SoftBarLineLeft: newSoft = newSoft.Inflated(new Vector2(-d.x, 0)); break;
                    case DragBar.SoftBarLineRight: newSoft = newSoft.Inflated(new Vector2(d.x, 0)); break;
                    case DragBar.SoftBarLineTop: newSoft = newSoft.Inflated(new Vector2(0, -d.y)); break;
                    case DragBar.SoftBarLineBottom: newSoft = newSoft.Inflated(new Vector2(0, d.y)); break;
                    case DragBar.HaFDBarLineLeft: newHaFD = newHaFD.Inflated(new Vector2(-d.x, 0)); break;
                    case DragBar.HaFDBarLineRight: newHaFD = newHaFD.Inflated(new Vector2(d.x, 0)); break;
                    case DragBar.HaFDBarLineBottom: newHaFD = newHaFD.Inflated(new Vector2(0, d.y)); break;
                    case DragBar.HaFDBarLineTop: newHaFD = newHaFD.Inflated(new Vector2(0, -d.y)); break;
                }

                // Apply the changes, enforcing the bounds
                SetNewBounds(GetHaFDGuide(), GetSoftGuide(), newHaFD, newSoft);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
    }
}
