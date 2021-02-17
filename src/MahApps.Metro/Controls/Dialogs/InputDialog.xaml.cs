// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public partial class InputDialog : BaseMetroDialog
    {
        private CancellationTokenRegistration cancellationTokenRegistration;

        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(InputDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register(nameof(Input), typeof(string), typeof(InputDialog), new PropertyMetadata(default(string)));

        public string Input
        {
            get => (string)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        /// <summary>Identifies the <see cref="AffirmativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register(nameof(AffirmativeButtonText), typeof(string), typeof(InputDialog), new PropertyMetadata("OK"));

        public string AffirmativeButtonText
        {
            get => (string)GetValue(AffirmativeButtonTextProperty);
            set => SetValue(AffirmativeButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register(nameof(NegativeButtonText), typeof(string), typeof(InputDialog), new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get => (string)GetValue(NegativeButtonTextProperty);
            set => SetValue(NegativeButtonTextProperty, value);
        }

        internal InputDialog()
            : this(null)
        {
        }

        internal InputDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal InputDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
        }

        internal Task<string> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    Focus();
                    PART_TextBox.Focus();
                }));

            var tcs = new TaskCompletionSource<string>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = () =>
                {
                    PART_TextBox.KeyDown -= affirmativeKeyHandler;

                    KeyDown -= escapeKeyHandler;

                    PART_NegativeButton.Click -= negativeHandler;
                    PART_AffirmativeButton.Click -= affirmativeHandler;

                    PART_NegativeButton.KeyDown -= negativeKeyHandler;
                    PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;

                    cancellationTokenRegistration.Dispose();
                };

            cancellationTokenRegistration = DialogSettings
                                                     .CancellationToken
                                                     .Register(() =>
                                                         {
                                                             this.BeginInvoke(() =>
                                                                 {
                                                                     cleanUpHandlers();
                                                                     tcs.TrySetResult(null);
                                                                 });
                                                         });

            escapeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(null);
                    }
                };

            negativeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(null);
                    }
                };

            affirmativeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(Input);
                    }
                };

            negativeHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);

                    e.Handled = true;
                };

            affirmativeHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(Input);

                    e.Handled = true;
                };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_TextBox.KeyDown += affirmativeKeyHandler;

            KeyDown += escapeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            AffirmativeButtonText = DialogSettings.AffirmativeButtonText;
            NegativeButtonText = DialogSettings.NegativeButtonText;

            switch (DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    PART_NegativeButton.SetResourceReference(StyleProperty, "MahApps.Styles.Button.Dialogs.AccentHighlight");
                    PART_TextBox.SetResourceReference(ForegroundProperty, "MahApps.Brushes.ThemeForeground");
                    PART_TextBox.SetResourceReference(ControlsHelper.FocusBorderBrushProperty, "MahApps.Brushes.TextBox.Border.Focus");
                    break;
            }
        }
    }
}