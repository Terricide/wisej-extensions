﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Wisej.Base;
using Wisej.Core;
using Wisej.Core.Design;

namespace Wisej.Web.Ext.CountUp
{
	/// <summary>
	/// CountUp.js is a dependency-free, lightweight JavaScript widget that can be used to 
	/// create animations that display numerical data in a more interesting way.
	/// https://github.com/inorganik/countUp.js.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(CountUp))]
	[DefaultProperty(nameof(Value))]
	[DefaultEvent(nameof(CountTerminated))]
	[DefaultBindingProperty(nameof(Value))]
	[Description("CountUp.js is a JavaScript widget that can create animations to display numerical data.")]
	public class CountUp : Control, IWisejControl
	{
		#region Events

		/// <summary>
		/// Fired when the value changes.
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Fired when the widget has finished counting.
		/// </summary>
		public event EventHandler CountTerminated
		{
			add { AddHandler(nameof(CountTerminated), value); }
			remove { RemoveHandler(nameof(CountTerminated), value); }
		}

		/// <summary>
		/// Fires the <see cref="E:Wisej.Web.Ext.CountUp.ValueChanged" /> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnValueChanged(EventArgs e)
		{
			if (this.ValueChanged != null)
				ValueChanged(this, e);
		}

		/// <summary>
		/// Fires the <see cref="E:Wisej.Web.Ext.CountUp.CountTerminated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnCountTerminated(EventArgs e)
		{
			((EventHandler)base.Events[nameof(CountTerminated)])?.Invoke(this, e);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns or sets a numeric value to count to.
		/// </summary>
		[Bindable(true)]
		[DefaultValue(0)]
		[DesignerActionList]
		[SRCategory("CatBehavior")]
		[SRDescription(" Returns or sets a numeric value to count to.")]
		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (this._value != value)
				{
					this._value = value;

					Update();
					OnValueChanged(EventArgs.Empty);
				}
			}
		}
		private float _value;

		/// <summary>
		/// Returns or sets the duration of the animation in milliseconds.
		/// </summary>
		[DefaultValue(2500)]
		[DesignerActionList]
		[SRCategory("CatBehavior")]
		[SRDescription("Returns or sets the duration of the animation in milliseconds.")]
		public int Duration
		{
			get { return this._duration; }
			set
			{
				if (this._duration != value)
				{
					if (value < 100)
						throw new ArgumentOutOfRangeException("value");

					this._duration = value;
					Update();
				}
			}
		}
		private int _duration = 2500;

		/// <summary>
		/// Returns or sets whether the animation will use easing.
		/// </summary>
		[DefaultValue(true)]
		[SRCategory("CatBehavior")]
		[SRDescription("Returns or sets whether the animation will use easing.")]
		public bool UseEasing
		{
			get { return this._useEasing; }
			set
			{
				if (this._useEasing != value)
				{
					this._useEasing = value;
					Update();
				}
			}
		}
		private bool _useEasing = true;

		/// <summary>
		/// Returns or sets whether the numeric value will be formatted using the
		/// grouping separator.
		/// </summary>
		[DefaultValue(true)]
		[SRCategory("CatBehavior")]
		[SRDescription("Returns or sets whether the numeric value will be formatted using the grouping separator.")]
		public bool UseGrouping
		{
			get { return this._useGrouping; }
			set
			{
				if (this._useGrouping != value)
				{
					this._useGrouping = value;
					Update();
				}
			}
		}
		private bool _useGrouping = true;

		/// <summary>
		/// Array of custom numerals for the digits from 0 to 9.
		/// </summary>
		[SRCategory("CatAppearance")]
		[Description("Array of custom numerals for the digits from 0 to 9.")]
		public string[] Numerals
		{
			get { return this._numerals; }
			set
			{
				if (value == null)
					value = new string[0];

				if (this._numerals != value)
				{
					if (value.Length > 0 && value.Length != 10)
						throw new ArgumentException("Must specify 0 or 10 numerals.", "value");

					this._numerals = value;
					Update();
				}
			}
		}
		private string[] _numerals = new string[0];

		private bool ShouldSerializeNumerals()
		{
			return this._numerals.Length > 0;
		}

		private void ResetNumerals()
		{
			this.Numerals = null;
		}

		#endregion

		#region Wisej Implementation

		/// <summary>
		/// Returns the theme appearance key for this control.
		/// </summary>
		string IWisejControl.AppearanceKey
		{
			get { return this.AppearanceKey ?? "countup"; }
		}

		/// <summary>
		/// Processes events from the client.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnWebEvent(WisejEventArgs e)
		{
			switch (e.Type)
			{
				case "countTerminated":
					OnCountTerminated(EventArgs.Empty);
					break;

				default:
					base.OnWebEvent(e);
					break;
			}
		}

		/// <summary>
		/// Renders the client component.
		/// </summary>
		/// <param name="config">Dynamic configuration object.</param>
		protected override void OnWebRender(dynamic config)
		{
			base.OnWebRender((object)config);

			config.className = "wisej.web.ext.CountUp";
			config.value = this.Value;
			config.duration = this.Duration;
			config.numerals = this.Numerals;
			config.useEasing = this.UseEasing;
			config.useGrouping = this.UseGrouping;
            switch(this.TextAlign)
            {
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.TopLeft:
                    config.align = "left";
                    break;
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                case ContentAlignment.TopRight:
                    config.align = "right";
                    break;
                default:
                    config.align = "center";
                    break;
            }

			var numberFormat = Application.CurrentCulture.NumberFormat;
			config.separator = numberFormat.NumberGroupSeparator;
			config.@decimal = numberFormat.NumberDecimalSeparator;

			config.wiredEvents.Add("countTerminated");
		}

        //
        // Summary:
        //     Returns or sets the alignment of text in the label.
        //
        // Returns:
        //     One of the System.Drawing.ContentAlignment values. The default is System.Drawing.ContentAlignment.TopLeft.
        [DefaultValue(ContentAlignment.TopLeft)]
        [Localizable(true)]
        [ResponsiveProperty]
        [SRCategory("CatAppearance")]
        [SRDescription("LabelTextAlignDescr")]
        public virtual ContentAlignment TextAlign
        {
            get
            {
                return mTextAlign;
            }
            set
            {
                if (this.mTextAlign != value)
                {
                    this.mTextAlign = value;
                    Update();
                }
            }
        }

        private ContentAlignment mTextAlign = ContentAlignment.TopLeft;


        #endregion

    }
}
