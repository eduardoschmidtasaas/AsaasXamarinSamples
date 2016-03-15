using System;
using Android.Widget;

namespace ChipViewXamarin
{
	public class Chip
	{
		private string mName;
		private string mEmail;
		private string mEditText = "";

		public Chip(string email,string name) {
			mName = name;
			mEmail = email;
		}

		public string GetName() {
			return mName;
		}

		public void SetName(string name) {
			mName = name;
		}

		public string GetEmail(){
			return mEmail;
		}

		public void SetEmail(string email){
			mEmail = email;
		}

		public string GetEditText(){
			return mEditText;
		}

		public void SetEditText(string editText){
			mEditText = editText;
		}
	}
}

