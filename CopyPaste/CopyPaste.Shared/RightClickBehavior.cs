using System;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Controls;

namespace kahua.host.uno.ui.behaviors
{
    public class RightClickBehavior : Behavior<TextBox>
    {
        private readonly MenuFlyout _menu;
        public RightClickBehavior() => _menu = new MenuFlyout();
        protected  void OnAttach()
        {
            base.OnSetup();
            AssociatedObject.ContextFlyout = _menu;
            _menu.Opening += _menu_Opening;
        }
        protected void OnDetach()
        {
            base.OnCleanup();
            _menu.Opening -= _menu_Opening;
        }
        void L(string d)
        {
            Console.WriteLine($"######## {d}");
            System.Diagnostics.Debug.WriteLine($"######## {d}");
        }
        private async void GetClipboard(Op op)
        {
            var text = AssociatedObject.Text;

            switch (op)
            {
                case Op.paste:
                    {
                        var c = await Clipboard.GetContent().GetTextAsync();
                        L($"Pasting: {c}");
                        AssociatedObject.Text = string.IsNullOrEmpty(AssociatedObject.SelectedText) ?
                         AssociatedObject.Text + c
                            :
                            AssociatedObject.Text.Substring(0, AssociatedObject.SelectionStart) + c + AssociatedObject.Text.Substring(AssociatedObject.SelectionStart + AssociatedObject.SelectionLength);
                        L($"Paste result: {AssociatedObject.Text}");
                        return;
                    }
                case Op.cut:
                    {
                        L($"Cutting: {text}");
                        if (string.IsNullOrEmpty(AssociatedObject.SelectedText))
                        {
                            AssociatedObject.Text = String.Empty;
                        }
                        else
                        {
                            text = AssociatedObject.SelectedText;
                            AssociatedObject.Text = AssociatedObject.Text.Remove(AssociatedObject.SelectionStart, AssociatedObject.SelectionLength);
                        }
                        L($"Cutting result: {AssociatedObject.Text}");
                        break;

                    }
                case Op.copy:
                    {
                        if (!string.IsNullOrEmpty(AssociatedObject.SelectedText))
                            text = AssociatedObject.SelectedText;
                        L($"Copying: {text}");
                        break;
                    }
            }

            L($"Sending to Clipboard: {text}");
            var dp = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            dp.SetText(text);
            Clipboard.SetContent(dp);
            L($"Clipboard: {await Clipboard.GetContent().GetTextAsync()}");

        }

        enum Op { copy, cut, paste }
        private void _menu_Opening(object sender, object e)
        {
            _menu.Items.Clear();
            var copy = new MenuFlyoutItem
            {
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Text = Localization.Get("Label_Copy"),
                CommandParameter = Op.copy,
                Command = new RelayCommand<Op>(GetClipboard, _ => !string.IsNullOrEmpty(AssociatedObject.Text)),
            };
            copy.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.C, Modifiers = Windows.System.VirtualKeyModifiers.Control });
            _menu.Items.Add(copy);

            var cut = new MenuFlyoutItem
            {
                Icon = new SymbolIcon { Symbol = Symbol.Cut },
                Text = Localization.Get("Label_Cut"),
                CommandParameter = Op.cut,
                Command = new RelayCommand<Op>(GetClipboard, _ => !string.IsNullOrEmpty(AssociatedObject.Text) && AssociatedObject.IsEnabled),
            };
            cut.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.X, Modifiers = Windows.System.VirtualKeyModifiers.Control });
            _menu.Items.Add(cut);

            var paste = new MenuFlyoutItem
            {
                Icon = new SymbolIcon { Symbol = Symbol.Paste },
                Text = Localization.Get("Label_Paste"),
                CommandParameter = Op.paste,
                Command = new RelayCommand<Op>(GetClipboard, _ => Clipboard.GetContent().Contains(StandardDataFormats.Text) && AssociatedObject.IsEnabled),
            };
            paste.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.V, Modifiers = Windows.System.VirtualKeyModifiers.Control });

            _menu.Items.Add(paste);
        }
    }
}