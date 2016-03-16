using ChipViewXamarin;
using Android.OS;
using System.Collections.Generic;
using Android.Content.Res;
using Android.App;
using System;
using XamarinChipView;
using Android.Content.PM;
using Android.Views;
using Android.Widget;

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
			if (chip.GetName () != "ISNULL") {
				string email = chip.GetEmail ();
				string editEmail = chip.GetEditText ();
				Chip lastChip = mChipLayout.GetLastChip ();
				string lastEditEmail = lastChip.GetEditText ();
				if (mChipLayout.ChipEmailIsEmpty(lastEditEmail)) {
					mChipLayout.Remove (chip);
					lastChip.SetEditText (email);
				} else {
					lastEditEmail = lastEditEmail.Remove (0, 1);
//					if (objVerifyFields.VerifyEmailField (lastEditEmail).Item1) { //Your email verification if you want.
						lastChip.SetEmail (lastEditEmail);
						lastChip.SetName ("NoName");

						Chip newChip = new Chip ("ISNULL", "ISNULL");
						newChip.SetEditText (chip.GetEmail ());
						mChipLayout.Add (newChip);
						mChipLayout.Remove (chip);
//					}else{
//						Toast.MakeText (this, "O email " + lastEditEmail + "  é inválido.", ToastLength.Long).Show ();							
//					}
				}
				mChipLayout.Refresh ();
			}
		}
	}
}