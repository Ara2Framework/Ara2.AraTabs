// Copyright (c) 2010-2016, Rafael Leonel Pontani. All rights reserved.
// For licensing, see LICENSE.md or http://www.araframework.com.br/license
// This file is part of AraFramework project details visit http://www.arafrework.com.br
// AraFramework - Rafael Leonel Pontani, 2016-4-14
using System;
using System.Collections.Generic;
using System.Linq;
using Ara2;
using Ara2.Dev;

namespace Ara2.Components
{
    [Serializable]
    [AraDevComponent(vResizable:false,vDraggable:false)]
    public class AraTab : AraDiv, IAraDev
    {
        public object Tag = null;

        private AraTabs AraTabs
        {
            get { return (AraTabs)this.ConteinerFather; }
        }

        private string _Caption = "";
        [AraDevProperty("")]
        public string Caption
        {
            get
            {
                return _Caption;
            }
            set
            {
                _Caption = value;

                AraTabs.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SeCaptionTab('" + this.InstanceID + "','" + AraTools.StringToStringJS(_Caption) + "');");
            }
        }

        private int _Pos = -1;
        [AraDevProperty(-1)]
        public int Pos
        {
            get
            {
                return _Pos;
            }
            set
            {
                _Pos = (value > 0 ? value : 0);

                AraTabs.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SeTabPos('" + this.InstanceID + "'," + _Pos + ");");
            }
        }

        public void SetPosInternal(int vPos)
        {
            _Pos = vPos;
        }

        private bool _AllowsClose = false;
        [AraDevProperty(false)]
        public bool AllowsClose
        {
            get
            {
                return _AllowsClose;
            }
            set
            {
                _AllowsClose = value;

                AraTabs.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SeCloseTab('" + this.InstanceID + "'," + (_AllowsClose ? "true" : "false") + ");");
            }
        }

        private bool _Enable = true;
        [AraDevProperty(true)]
        public bool Enabled
        {
            get
            {
                return _Enable;
            }
            set
            {
                _Enable = value;

                AraTabs.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetEnableTab('" + this.InstanceID + "'," + (_Enable ? "true" : "false") + ");");
            }
        }


        public AraTab(AraTabs Container, bool vAllowsClose) :
            this(Container)
        {
            AllowsClose = vAllowsClose;
        }

        public AraTab(AraTabs Container,string vCaption) :
            this(Container)
        {
            Caption = vCaption;
        }

        public AraTab(AraTabs Container, string vCaption, bool vAllowsClose) :
            this(Container)
        {
            Caption = vCaption;
            AllowsClose = vAllowsClose;
        }

        public AraTab(IAraObject Container) :
            this((AraTabs)Container)
        {
        }

        public AraTab(AraTabs Container):
            base(Container)
        {
            
            //this.TypePosition = ETypePosition.Static;
            this.VisibleChange += this_VisibleChange;

            _Pos = AraTabs.Childs.Length-1;

            //if (AraTabs.TabActive == null)
            //    AraTabs.TabActive = this;

            this.Anchor.Bottom = 1;
            this.Anchor.Left = 1;
            this.Anchor.Right = 1;
            this.Anchor.Top = ((AraTabs)this.ConteinerFather).TabsHeigth;
            
        }

        private void this_VisibleChange()
        {
            AraTabs.TickScriptCall();
            Tick.GetTick().Script.Send(" vObj.SetVisibleTab('" + this.InstanceID + "'," + (this.Visible ? "true" : "false") + ");");
        }

        [AraDevProperty(false)]
        public bool TabActive
        {
            get
            {
                return ((AraTabs)this.ConteinerFather).TabActive != null && ((AraTabs)this.ConteinerFather).TabActive.InstanceID == this.InstanceID;
            }
            set
            {
                if (value)
                {
                    ((AraTabs)this.ConteinerFather).TabActive = this;
                }
            }
        }

        #region Ara2Dev
        
        #endregion
    }
}