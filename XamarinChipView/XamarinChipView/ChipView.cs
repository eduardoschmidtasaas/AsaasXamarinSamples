using Android.Views;
using System.Collections.Generic;
using Android.Content;
using AttributeSet = Android.Util.IAttributeSet;
using Observer = Java.Util.IObserver;
using System;
using Java.Util;
using Android.Widget;
using IEditable = Android.Text.IEditable;
using Android.Text;
using Android.Views.InputMethods;
using Android.Graphics;
using XamarinChipView;

namespace ChipViewXamarin {
	public class ChipView : ViewGroup , Observer, Android.Views.View.IOnClickListener {
		private ChipViewAdapter mAdapter;
		private OnChipClickListener mListener;

		// Data
		private List<int> mLineHeightList;

		public ChipView(Context context, AttributeSet attrs):base(context, attrs) {
			init(context, attrs);
		}

		private void init(Context context, AttributeSet attrs) {
			mLineHeightList = new List<int>();
			ChipViewAdapter adapter =  new ChipViewAdapter(context, attrs);
			SetAdapter(adapter);
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			MeasureChildren(widthMeasureSpec, heightMeasureSpec);
			mLineHeightList.Clear ();
			int width = MeasuredWidth;
			int height = PaddingTop + PaddingBottom;
			int lineHeight = 0;
			int lineWidth = PaddingLeft;
			int childCount = ChildCount;

			for (int i = 0; i < childCount; i++) {
				View childView = GetChildAt(i);
				MarginLayoutParams layoutParams = (MarginLayoutParams) childView.LayoutParameters;
				bool lastChild = (i == childCount - 1);

				if (childView.Visibility == ViewStates.Gone) {
					if (lastChild)
						mLineHeightList.Add(lineHeight);
					continue;
				}

				int childWidth = (childView.MeasuredWidth + layoutParams.LeftMargin + layoutParams.RightMargin);
				int childHeight = (childView.MeasuredHeight + layoutParams.TopMargin + layoutParams.BottomMargin);
				lineHeight = Math.Max(lineHeight, childHeight);

				if (childWidth > width)
					width = childWidth;

				if (lineWidth + childWidth + PaddingLeft > width) {
					mLineHeightList.Add(lineHeight);
					lineWidth = PaddingLeft + childWidth;
				} else
					lineWidth += childWidth;

				if (lastChild)
					mLineHeightList.Add(lineHeight);
			}

			foreach (int h in mLineHeightList)
				height += h;

			SetMeasuredDimension(width, height);
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			if (mAdapter != null) {
				int width = MeasuredWidth;
				int lineWidth = PaddingLeft;
				int childCount = ChildCount;
				int j = 0;
				int lineHeight = (mLineHeightList.Count > 0 ? mLineHeightList[j] : 0);
				int childY = PaddingTop;

				for (int i = 0; i < childCount; i++) {
					Chip chip = mAdapter.GetChipList()[i];
					View childView = GetChildAt (i);
					ViewGroup.MarginLayoutParams layoutParams = (ViewGroup.MarginLayoutParams)childView.LayoutParameters;

					if (childView.Visibility == ViewStates.Gone)
						continue;

					int childWidth = (childView.MeasuredWidth + layoutParams.LeftMargin + layoutParams.RightMargin);
					int childHeight = (childView.MeasuredHeight + layoutParams.TopMargin + layoutParams.BottomMargin);

					if (childWidth > width)
						width = childWidth;

					if (lineWidth + childWidth + PaddingRight > width) {
						childY += lineHeight;
						j++;
						lineHeight = mLineHeightList[j];
						lineWidth = PaddingLeft + childWidth;
					} else
						lineWidth += childWidth;

					int childX = lineWidth - childWidth;

					childView.Layout((childX + layoutParams.LeftMargin), (childY + layoutParams.TopMargin), (lineWidth - layoutParams.RightMargin), (childY + childHeight - layoutParams.BottomMargin));

					if (mListener != null) {
						childView.SetOnClickListener (this);
					}
				}
			}
		}

		public void OnClick(View view){
			int position = IndexOfChild (view);
			Chip chip = mAdapter.GetChipList()[position];
			mListener.OnChipClick(chip);
		}

		public void Refresh() {
			if (mAdapter != null) {
				RemoveAllViews ();

				for (int i = 0; i < Count() + 1; i++) {
					Chip chip = mAdapter.GetChipList()[i];
					View view = mAdapter.GetView(this, i);


					if (view != null) {
						if (mListener != null) {
							view.Clickable = true;
							view.Focusable = true;
						}
						if (i != (Count())) {
							view.FindViewById<EditText> (Resource.Id.editTextNewEmail).Visibility = ViewStates.Gone;
						} else if (i == Count()) {
							EditText editText = view.FindViewById<EditText> (Resource.Id.editTextNewEmail);
							editText.RequestFocus ();

							editText.AfterTextChanged += (object sender, AfterTextChangedEventArgs e) => {
								if (editText.Text.Length == 0){
									if (Count() > 0){
										Remove (GetChipAt (Count () - 1));
										GetLastChip ().SetEditText ("");
										Refresh ();
									}
								}
							};

							editText.TextChanged += (object sender, TextChangedEventArgs e) => {
								editText.Background.ClearColorFilter();
							};

							editText.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
								if (e.ActionId == ImeAction.Done){
									string email = chip.GetEditText().Remove(0, 1);
//									if (!String.IsNullOrEmpty(email) && !objVerifyFields.VerifyEmailField (email).Item1) { //Your email verification if you want.
//										Toast.MakeText (Context, "O email " + email + "  é inválido.", ToastLength.Long).Show ();
//										editText.Background.SetColorFilter(Color.Red, PorterDuff.Mode.SrcIn);
//									}else{
										if (!Exists(email)){
											GetLastChip().SetEmail(email);
											GetLastChip().SetName("NoName");
											Add(new Chip("ISNULL","ISNULL"));
										}else{
											Toast.MakeText (Context, "O email " + email + " já está adicionado.", ToastLength.Long).Show ();
											editText.Background.SetColorFilter(Color.Red, PorterDuff.Mode.SrcIn);
										}
//									}
								}
							};

						}
						AddView(view);
					}
				}

				Invalidate ();
			}
		}

		public bool ChipEmailIsEmpty(string email){
			if (email == " ") {
				return true;
			}
			return false;
		}

		public void Add(Chip chip) {
			mAdapter.Add (chip);
		}

		public void Remove(Chip chip) {
			mAdapter.Remove (chip);
		}

		public bool Exists(string email){
			return mAdapter.Exists (email);
		}

		public int Count() {
			return mAdapter.Count();
		}

		public List<Chip> GetChipList() {
			return mAdapter.GetChipList ();
		}

		public Chip GetChipAt(int position){
			return mAdapter.GetChipAt (position);					
		}

		public Chip GetLastChip(){
			return mAdapter.GetLastChip ();
		}

		public void SetChipList(List<Chip> chipList) {
			mAdapter.SetChipList(chipList);
		}

		public ChipViewAdapter GetAdapter() {
			return mAdapter;
		}

		public void SetAdapter(ChipViewAdapter adapter) {
			mAdapter = adapter;
			mAdapter.DeleteObservers ();
			mAdapter.AddObserver(this);
			Refresh();
		}

		public void SetChipBackgroundResource(int backgroundRes) {
			mAdapter.SetChipBackgroundResource(backgroundRes);
		}

		public int GetChipLayoutResource() {
			return mAdapter.GetChipLayoutResource();
		}

		public void SetChipLayoutResource(int chipRes) {
			mAdapter.SetChipLayoutResource(chipRes);
		}

		public void SetOnChipClickListener(OnChipClickListener listener) {
			mListener = listener;
		}

		public bool IsToleratingDuplicate() {
			return mAdapter.IsToleratingDuplicate();
		}

		public void SetToleratingDuplicate(bool toleratingDuplicate) {
			mAdapter.SetToleratingDuplicate(toleratingDuplicate);
		}

		public bool HasBackground() {
			return mAdapter.HasBackground();
		}

		public void SetHasBackground(bool hasBackground) {
			mAdapter.SetHasBackground(hasBackground);
		}

		public int GetChipSpacing() {
			return mAdapter.GetChipSpacing();
		}

		public void SetChipSpacing(int chipSpacing) {
			mAdapter.SetChipSpacing(chipSpacing);
		}

		public int GetLineSpacing() {
			return mAdapter.GetLineSpacing();
		}

		public void SetLineSpacing(int lineSpacing) {
			mAdapter.SetLineSpacing(lineSpacing);
		}

		public int GetChipPadding() {
			return mAdapter.GetChipPadding();
		}

		public void SetChipPadding(int chipPadding) {
			mAdapter.SetChipPadding(chipPadding);
		}

		public int GetChipSidePadding() {
			return mAdapter.GetChipSidePadding();
		}

		public void SetChipSidePadding(int chipSidePadding) {
			mAdapter.SetChipSidePadding(chipSidePadding);
		}

		public int GetChipCornerRadius() {
			return mAdapter.GetChipCornerRadius();
		}

		public void SetChipCornerRadius(int chipCornerRadius) {
			mAdapter.SetChipCornerRadius(chipCornerRadius);
		}

		public int GetChipBackgroundColor() {
			return mAdapter.GetChipBackgroundColor();
		}

		public void SetChipBackgroundColor(int chipBackgroundColor) {
			mAdapter.SetChipBackgroundColor(chipBackgroundColor);
		}

		public int GetChipBackgroundColorSelected() {
			return mAdapter.GetChipBackgroundColorSelected();
		}

		public void SetChipBackgroundColorSelected(int chipBackgroundColorSelected) {
			mAdapter.SetChipBackgroundColorSelected(chipBackgroundColorSelected);
		}

		public int GetChipTextSize() {
			return mAdapter.GetChipTextSize();
		}

		public void SetChipTextSize(int chipTextSize) {
			mAdapter.SetChipTextSize(chipTextSize);
		}

		public void Update(Observable observable, Java.Lang.Object data){
			Refresh ();
		}

		protected override LayoutParams GenerateDefaultLayoutParams ()
		{
			return new MarginLayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
		}

		protected override LayoutParams GenerateLayoutParams (LayoutParams p)
		{
			return new MarginLayoutParams(p);
		}

		public override LayoutParams GenerateLayoutParams (AttributeSet attrs)
		{
			return new MarginLayoutParams(Context, attrs);
		}
	}
}
