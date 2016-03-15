using System;
using Android.Text.Method;
using Android.Runtime;
using Android.Text;
using Java.Lang;

namespace XamarinChipView
{
	public class DeleteListener : QwertyKeyListener
	{
		public EventHandler eventHandler;

		public DeleteListener (TextKeyListener.Capitalize cap, bool autotext):base(cap, autotext){
		}

		public override InputTypes InputType { get{ return InputTypes.TextVariationWebPassword; } }

		public override bool OnKeyDown (Android.Views.View view, IEditable content, Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
		{
			if (e.KeyCode == Android.Views.Keycode.Del) {
				if (eventHandler != null) {
					eventHandler.Invoke (this, EventArgs.Empty);
				}
			}
			return base.OnKeyDown (view, content, keyCode, e);
		}
	}
}

