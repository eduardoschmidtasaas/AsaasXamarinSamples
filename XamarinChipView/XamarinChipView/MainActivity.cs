using ChipViewXamarin;
using Android.OS;
using System.Collections.Generic;
using Android.Content.Res;
using Android.App;
using System;
using XamarinChipView;
using Android.Content.PM;
using Android.Views;

namespace ChipViewXamarin {

	[Activity(Label = "@string/app_name", MainLauncher = true,ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustPan)]
	public class AutoCompleteTextViewScreen : Activity, ChipViewXamarin.OnChipClickListener {

		private List<Chip> mChipList;
	
	    private ChipView mChipLayout;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.activity_main);

			mChipList = new List<Chip>();
			mChipList.Add(new Chip("mattheus@outlook.com", "Matheus"));

			// Adapter
			ChipViewAdapter adapterLayout = new ChipViewAdapter(this, null);

			mChipLayout = (ChipView) FindViewById(Resource.Id.text_chip_layout);
			mChipLayout.SetAdapter(adapterLayout);
			mChipLayout.SetChipLayoutResource(Resource.Layout.chipview);
			mChipLayout.SetChipList(mChipList);
			mChipLayout.SetOnChipClickListener (this);

		}

		public void OnChipClick(Chip chip) {
			if (chip.GetName () != "@NULL") {
				string email = chip.GetEmail ();
				string editEmail = chip.GetEditText ();
				Chip lastChip = mChipLayout.GetLastChip ();
				string lastEditEmail = lastChip.GetEditText ();
				if (chip == lastChip && lastEditEmail == "") {
					mChipLayout.Remove (chip);
					lastChip = mChipLayout.GetLastChip ();
					lastChip.SetEditText (email);
				} else if (chip == lastChip) {
					lastChip = mChipLayout.GetLastChip ();
					lastChip.SetEditText (email);
					lastChip.SetEmail (lastEditEmail);
				} else if (lastEditEmail == "") {
					mChipLayout.Remove (chip);
					lastChip.SetEditText (email);
				} else if (lastEditEmail != "") {
					var lastEditText = lastChip.GetEditText ();
					lastChip.SetEditText (email);
					chip.SetEmail (lastEditText);
				}

				mChipLayout.Refresh ();
			}
		}
	}
}