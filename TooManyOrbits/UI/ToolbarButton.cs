using System;
using KSP.UI.Screens;
using UnityEngine;
using ToolbarControl_NS;

namespace TooManyOrbits.UI
{
    internal class ToolbarButton : IDisposable
    {
        
        internal  static ToolbarControl toolbarControl;
        
        private GameObject gameObject;
        internal static ResourceProvider rp;

        public ToolbarButton(GameObject go, ResourceProvider resources)
        {
            gameObject = go;
            rp = resources;
            toolbarControl = null;
        }

        public void Dispose()
        {
            Hide();
        }

        public void Show()
        {
            if (toolbarControl == null)
            {
                BuildButton();

            }
        }

        public void Hide()
        {
            if (toolbarControl != null)
            {
                DestroyButton();
                toolbarControl = null;
            }
        }

        internal const string MODID = "toomanyorbits_NS";
        internal const string MODNAME = "Too Many Orbits";

        private void BuildButton()
        {

            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(OnEnable2, OnDisabl2e,
                ApplicationLauncher.AppScenes.ALWAYS,
                MODID,
                "geocacheButton",
                //rp.BuildPath("ToolbarIcon-Green-38", false),
                rp.BuildPath("ToolbarIcon-38", false),
                //rp.BuildPath("ToolbarIcon-Green-24", false),
                rp.BuildPath("ToolbarIcon-24", false)

            );
        }
        void OnEnable2()
        {
            TooManyOrbitsModule.Instance.m_window.Show();
        }
        void OnDisabl2e()
        {
            TooManyOrbitsModule.Instance.m_window.Hide();
        }
        private void DestroyButton()
        {
            toolbarControl﻿.OnDestroy();
            UnityEngine.Object.Destroy(toolbarControl);

        }
    }
}
