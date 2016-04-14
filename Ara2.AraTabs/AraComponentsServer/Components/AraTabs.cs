// Copyright (c) 2010-2016, Rafael Leonel Pontani. All rights reserved.
// For licensing, see LICENSE.md or http://www.araframework.com.br/license
// This file is part of AraFramework project details visit http://www.arafrework.com.br
// AraFramework - Rafael Leonel Pontani, 2016-4-14
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Ara2;
using Ara2.Dev;

namespace Ara2.Components
{
    [Serializable]
    [AraDevComponent(vCompatibleChildrenTypes: new Type[] { typeof(AraTab) }, vAddAlsoToStart: typeof(AraTab))]
    public class AraTabs : AraComponentVisualAnchorConteiner,IAraDev
    {
        public AraTabs(IAraContainerClient ConteinerFather)
            : base(AraObjectClienteServer.Create(ConteinerFather, "Div"), ConteinerFather, "AraTabs")
        {
            Click = new AraComponentEvent<EventHandler>(this, "Click");
            ClickCloseTab = new AraComponentEvent<DClickCloseTab>(this, "ClickCloseTab");
            TabActiveChange = new AraComponentEvent<Action>(this, "TabActiveChange");
            IsVisible = new AraComponentEvent<EventHandler>(this, "IsVisible");
            OnSort = new AraComponentEvent<Action>(this, "OnSort");
            ChangeTabsHeigth = new AraComponentEvent<Action>(this, "ChangeTabsHeigth");

            this.ChangeTabsHeigth += this_ChangeTabsHeigth;

            this.AddChildBefore += this_AddChildBefore;
            this.AddChildAfter += this_AddChildAfter;
            this.RemuveChildBefore += this_RemuveChildBefore;

            this.EventInternal += AraTabs_EventInternal;
            this.SetProperty += this_SetProperty;

            this._MinWidth = 40;
            this._MinHeight = 40;
            this._Width = 200;
            this._Height = 200;
        }

        private void this_AddChildBefore(IAraObject Child)
        {
            if (!(Child is AraTab || Child is AraResizable || Child is AraDraggable) )
                throw new Exception("Not allowed to add on a AraTabs an object other than AraTab.");
        }

        private void this_AddChildAfter(IAraObject Child)
        {
            if (Child is AraTab)
            {
                AraTab Tab = (AraTab)Child;

                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.AddTab('" + Tab.InstanceID + "');");
            }
        }

        private void this_RemuveChildBefore(IAraObject Child)
        {
            if (Child is AraTab)
            {
                AraTab Tab = (AraTab)Child;
                this.TickScriptCall();
                if (Tab != null)
                    Tick.GetTick().Script.Send(" vObj.delTab('" + Tab.InstanceID + "');");
            }
        }

        public override void LoadJS()
        {
            Tick vTick = Tick.GetTick();
            vTick.Session.AddJs("Ara2/Components/AraTabs/AraTabs.js");
        }

        public void AraTabs_EventInternal(String vFunction)
        {
            switch (vFunction.ToUpper())
            {
                case "CLICKCLOSETAB":
                    {
                        bool PodeFechar = true;
                        string vInstanceId = (string)Tick.GetTick().Page.Request["TabKey"];
                        AraTab Tab = (AraTab)(Tick.GetTick().Session.GetObject(vInstanceId));

                        if (ClickCloseTab.InvokeEvent != null)
                            PodeFechar = ClickCloseTab.InvokeEvent(Tab);

                        if (PodeFechar)
                            this.RemuveChild(Tab);
                    }
                    break;
                case "CLICK":
                    Click.InvokeEvent(this,new EventArgs());
                    break;
                case "TABACTIVECHANGE":
                    TabActiveChange.InvokeEvent();
                break;
                case "ISVISIBLE":
                    IsVisible.InvokeEvent(this, new EventArgs());
                    break;
                case "ONSORT":
                    OnSort.InvokeEvent();
                    break;
                case "CHANGETABSHEIGTH":
                    {
                        _TabsHeigth = Convert.ToInt32(Tick.GetTick().Page.Request["GetTabsHeigth"]);
                        ChangeTabsHeigth.InvokeEvent();
                    }
                    break;
            }
        }



        #region Eventos
        public delegate bool DClickCloseTab(AraTab Tab);
        
        [AraDevEvent]
        public AraComponentEvent<EventHandler> Click;
        [AraDevEvent]
        public AraComponentEvent<DClickCloseTab> ClickCloseTab;
        [AraDevEvent]
        public AraComponentEvent<Action> TabActiveChange;
        [AraDevEvent]
        public AraComponentEvent<EventHandler> IsVisible;
        [AraDevEvent]
        public AraComponentEvent<Action>  OnSort;
        [AraDevEvent]
        public AraComponentEvent<Action> ChangeTabsHeigth;
        #endregion


        private void this_SetProperty(String vProperty, dynamic vValue)
        {
            switch (vProperty)
            {
                case "TabActiveId":
                {
                    if (vValue != null)
                        _TabActive.Object = (AraTab)(Tick.GetTick().Session.GetObject(vValue));
                    else
                        _TabActive.Object = null;
                }
                break;
                case "GetPosTabs()":
                {
                    Tick Tick =Tick.GetTick();
                    foreach (dynamic vValue2 in Json.DynamicJson.Parse(vValue))
                    {
                        object vTmpObj = Tick.Session.GetObject(vValue2.key);
                        if (vTmpObj!=null)
                            ((AraTab)vTmpObj).SetPosInternal( Convert.ToInt32(vValue2.pos));
                    }
                }
                break;
                case "GetTabsHeigth()":
                {
                    _TabsHeigth = Convert.ToInt32(vValue);
                }
                break;
            }
        }

        private int _TabsHeigth=41;
        [AraDevProperty(41)]
        public int TabsHeigth
        {
            get
            {
                return _TabsHeigth;
            }
        }

        private void this_ChangeTabsHeigth()
        {
            foreach (AraTab TmpTab in this.Childs.Select(a => (AraTab)a))
            {
                TmpTab.Anchor.Top = TabsHeigth;
            }
        }
        
        Ara2.Components.AraObjectInstance<AraTab> _TabActive = new AraObjectInstance<AraTab>();


        //private string _TabActiveNamePendente = null;
        /// <summary>
        /// Aba Ativa no momento
        /// Obs: Este metodo não suporta varias alterações simultaneas! (Problemas no Jquery)
        /// </summary>
        public AraTab TabActive
        {
            get
            {
                return _TabActive.Object;
            }
            set
            {
                _TabActive.Object = value;

                this.TickScriptCall();
                Tick.GetTick().Script.Send("vObj.SetTabActiveId('" + value.InstanceID + "');\n");
            }
        }

        [AraDevProperty(null)]
        public string TabActiveByName
        {
            get
            {
                //if (TabActive == null)
                //    return null;

                //return TabActive.Name;
                return null;
            }
            set
            {
                //if (value == null)
                //    TabActive = null;
                //else
                //{
                //    if (this.Childs.Where(a => ((IAraDev)a).Name == value).Count() > 0)
                //        TabActive = (AraTab)this.Childs.Where(a => ((AraTab)a).Name == value).First();
                //    else
                //        _TabActiveNamePendente = value;
                //}
            }
        }

                                


        private bool _Sortable = false;
        [AraDevProperty(false)]
        public bool Sortable
        {
            get
            {
                return _Sortable;
            }
            set
            {
                _Sortable = value;

                this.TickScriptCall();
                Tick.GetTick().Script.Send(" vObj.SetSortable(" + (_Sortable ? "true" : "false") + ");\n");
            }
        }


        #region Ara2Dev

        private string _Name = "";
        [AraDevProperty("")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private AraEvent<DStartEditPropertys> _StartEditPropertys = null;
        public AraEvent<DStartEditPropertys> StartEditPropertys
        {
            get
            {
                if (_StartEditPropertys == null)
                {
                    _StartEditPropertys = new AraEvent<DStartEditPropertys>();
                    this.Click += this_ClickEdit;
                }

                return _StartEditPropertys;
            }
            set
            {
                _StartEditPropertys = value;
            }
        }
        private void this_ClickEdit(object sender, EventArgs e)
        {
            if (_StartEditPropertys.InvokeEvent != null)
                _StartEditPropertys.InvokeEvent(this);
        }

        private AraEvent<DStartEditPropertys> _ChangeProperty = new AraEvent<DStartEditPropertys>();
        public AraEvent<DStartEditPropertys> ChangeProperty
        {
            get
            {
                return _ChangeProperty;
            }
            set
            {
                _ChangeProperty = value;
            }
        }
        #endregion
    }
}
