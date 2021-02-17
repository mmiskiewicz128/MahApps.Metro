// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.ValueBoxes;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class ProgressDialog : BaseMetroDialog
    {
        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(ProgressDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        /// <summary>Identifies the <see cref="IsCancelable"/> dependency property.</summary>
        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register(nameof(IsCancelable), typeof(bool), typeof(ProgressDialog), new PropertyMetadata(BooleanBoxes.FalseBox, (s, e) => { ((ProgressDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden; }));

        public bool IsCancelable
        {
            get => (bool)GetValue(IsCancelableProperty);
            set => SetValue(IsCancelableProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register(nameof(NegativeButtonText), typeof(string), typeof(ProgressDialog), new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get => (string)GetValue(NegativeButtonTextProperty);
            set => SetValue(NegativeButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="ProgressBarForeground"/> dependency property.</summary>
        public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register(nameof(ProgressBarForeground), typeof(Brush), typeof(ProgressDialog), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ProgressBarForeground
        {
            get => (Brush)GetValue(ProgressBarForegroundProperty);
            set => SetValue(ProgressBarForegroundProperty, value);
        }

        public static readonly DependencyProperty ShowMessagePictureProperty = DependencyProperty.Register(nameof(ShowMessagePicture), typeof(bool), typeof(ProgressDialog), new PropertyMetadata(default(bool), (s, e) => { ((ProgressDialog)s).PART_UpdateImage.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden; }));

        public bool ShowMessagePicture
        {
            get => (bool)GetValue(ShowMessagePictureProperty);
            set => SetValue(ShowMessagePictureProperty, value);
        }

        internal ProgressDialog()
            : this(null)
        {
        }

        internal ProgressDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal ProgressDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
        }

        protected override void OnLoaded()
        {
            NegativeButtonText = DialogSettings.NegativeButtonText;
            SetResourceReference(ProgressBarForegroundProperty, DialogSettings.ColorScheme == MetroDialogColorScheme.Theme ? "MahApps.Brushes.Accent" : "MahApps.Brushes.ThemeForeground");
        }

        internal CancellationToken CancellationToken => DialogSettings.CancellationToken;

        internal double Minimum
        {
            get => PART_ProgressBar.Minimum;
            set => PART_ProgressBar.Minimum = value;
        }

        internal double Maximum
        {
            get => PART_ProgressBar.Maximum;
            set => PART_ProgressBar.Maximum = value;
        }

        internal double ProgressValue
        {
            get => PART_ProgressBar.Value;
            set
            {
                PART_ProgressBar.IsIndeterminate = false;
                PART_ProgressBar.Value = value;
                PART_ProgressBar.ApplyTemplate();
            }
        }

        internal void SetIndeterminate()
        {
            PART_ProgressBar.IsIndeterminate = true;
        }
    }
}