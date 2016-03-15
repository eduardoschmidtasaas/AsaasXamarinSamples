using Java.Util;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using AttributeSet = Android.Util.IAttributeSet;
using Android.Content.Res;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Util;
using Android.OS;
using System;
using XamarinChipView;

namespace ChipViewXamarin {
	public class ChipViewAdapter : Observable {
	    private Context mContext;
	    private AttributeSet mAttributeSet;
	    private int mChipSpacing;
	    private int mLineSpacing;
	    private int mChipPadding;
	    private int mChipCornerRadius;
	    private int mChipSidePadding;
	    private int mChipTextSize;
	    private int mChipRes;
	    private int mChipBackgroundColor;
	    private int mChipBackgroundColorSelected;
	    private int mChipBackgroundRes;
	    private bool mHasBackground = true;
	    private bool mToleratingDuplicate = false;
	    private LayoutInflater mInflater;
	    private List<Chip> mChipList;
		private EditText editText;

		public int GetLayoutRes(int position){
			return 0;
		}
			
		public int GetBackgroundResource(int position){
			return 0;
		}

		public int GetBackgroundColor(int position){
			return 0;
		}

		public int GetBackgroundColorSelected(int position){
			return 0;
		}

		public void OnLayout(View view, int position){
		}


		public ChipViewAdapter(Context context, AttributeSet attributeSet) {
	        mContext = context;
			mInflater = (LayoutInflater)mContext.GetSystemService (Context.LayoutInflaterService);
			mChipList = new List<Chip>();
	        SetAttributeSet(attributeSet);
	    }

	    private void Init() {
			mChipSpacing = mContext.Resources.GetDimensionPixelSize (Resource.Dimension.chip_spacing);
	        mChipSpacing = mContext.Resources.GetDimensionPixelSize (Resource.Dimension.chip_spacing);
	        mLineSpacing = mContext.Resources.GetDimensionPixelSize (Resource.Dimension.chip_line_spacing);
	        mChipPadding = mContext.Resources.GetDimensionPixelSize (Resource.Dimension.chip_padding);
	        mChipSidePadding = mContext.Resources.GetDimensionPixelSize (Resource.Dimension.chip_side_padding);
	        mChipCornerRadius = mContext.Resources.GetDimensionPixelSize (Resource.Dimension.chip_corner_radius);
//	        mChipBackgroundColor = GetColor(Resource.Color.chip_background);
//			mChipBackgroundColorSelected = GetColor(Resource.Color.chip_background_selected);

	        if (mAttributeSet != null) {
				TypedArray typedArray = mContext.Theme.ObtainStyledAttributes(mAttributeSet, Resource.Styleable.ChipView, 0, 0);

				try {
	                mChipSpacing = (int) typedArray.GetDimension(Resource.Styleable.ChipView_chip_spacing, mChipSpacing);
	                mLineSpacing = (int) typedArray.GetDimension(Resource.Styleable.ChipView_chip_line_spacing, mLineSpacing);
	                mChipPadding = (int) typedArray.GetDimension(Resource.Styleable.ChipView_chip_padding, mChipPadding);
	                mChipSidePadding = (int) typedArray.GetDimension(Resource.Styleable.ChipView_chip_side_padding, mChipSidePadding);
	                mChipCornerRadius = (int) typedArray.GetDimension(Resource.Styleable.ChipView_chip_corner_radius, mChipCornerRadius);
	                
					mChipBackgroundColor = typedArray.GetColor(Resource.Styleable.ChipView_chip_background, mChipBackgroundColor);
	                mChipBackgroundColorSelected = typedArray.GetColor(Resource.Styleable.ChipView_chip_background_selected, mChipBackgroundColorSelected);
					mChipBackgroundRes = typedArray.GetResourceId(Resource.Styleable.ChipView_chip_background_res, 0);
	            } finally {
					typedArray.Recycle ();
	            }
	        }
	    }

	    public View GetView(ViewGroup parent, int position) {
	        View view = null;
	        Chip chip = GetChipAt(position);

	        if (chip != null) {
	            int chipLayoutRes = (GetLayoutRes(position) != 0 ? GetLayoutRes(position) : GetChipLayoutResource());
	            Drawable chipBackground = GenerateBackgroundSelector(position);

	            if (chipLayoutRes == 0) {
					LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
	                layoutParams.SetMargins(0, 0, mChipSpacing, mLineSpacing);
	                view = new LinearLayout(mContext);
					view.LayoutParameters = layoutParams;
					((LinearLayout)view).Orientation = Android.Widget.Orientation.Horizontal;
					((LinearLayout) view).SetGravity(GravityFlags.CenterVertical);
					((LinearLayout)view).SetGravity (GravityFlags.CenterVertical);
					view.SetPadding(mChipSidePadding, mChipPadding, mChipSidePadding, mChipPadding);

					RelativeLayout parentView = (RelativeLayout) view;

	                TextView text = new TextView(mContext);
					text.Id = Resource.Id.content;
					parentView.AddView(text);
	            } else {
	                view = mInflater.Inflate(chipLayoutRes, parent, false);
					ViewGroup.MarginLayoutParams layoutParams = (ViewGroup.MarginLayoutParams)view.LayoutParameters;
					layoutParams.SetMargins(layoutParams.LeftMargin, layoutParams.LeftMargin, (layoutParams.RightMargin > 0 ? layoutParams.RightMargin : mChipSpacing), (layoutParams.BottomMargin > 0 ? layoutParams.BottomMargin : mLineSpacing));
	            }

	            if (view != null) {
					TextView text = (TextView) view.FindViewById(Resource.Id.text1);
					View content = view.FindViewById(Resource.Id.content);
					View chip_layout = view.FindViewById (Resource.Id.chip_layout);
					editText = view.FindViewById<EditText> (Resource.Id.editTextNewEmail);

					editText.Text = chip.GetEditText ();
					editText.TextChanged += TextChangeEditEmail;

	                if (text != null) {
						text.Text = chip.GetEmail();
						text.Gravity = GravityFlags.Center;

	                    if (mChipTextSize > 0)
							text.SetTextSize (Android.Util.ComplexUnitType.Sp, mChipTextSize);
	                }
					if (text.Text == "@NULL") {
						chip_layout.Visibility = ViewStates.Gone;
					}

	                if (mHasBackground) {
						if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.JellyBean) {
	                        if (content != null)
	                            content.SetBackgroundDrawable(chipBackground);
	                        else
	                            view.SetBackgroundDrawable(chipBackground);
	                    } else {
	                        if (content != null)
	                            content.SetBackgroundDrawable(chipBackground);
	                        else
	                            view.SetBackgroundDrawable(chipBackground);
	                    }
	                }

	                OnLayout(view, position);
	            }
	        }

	        return view;
	    }

		private void TextChangeEditEmail(object sender, Android.Text.TextChangedEventArgs e){
			editText.TextChanged -= TextChangeEditEmail;
			GetLastChip().SetEditText(editText.Text);
			editText.TextChanged += TextChangeEditEmail;
		}

	    private Drawable GenerateBackgroundSelector(int position) {
	        if (GetBackgroundResource(position) != 0)
				return mContext.Resources.GetDrawable (GetBackgroundResource (position));
	        else if (mChipBackgroundRes != 0) {
				return mContext.Resources.GetDrawable (mChipBackgroundRes);
	        }

	        int backgroundColor = (GetBackgroundColor(position) != 0 ? GetBackgroundColor(position) : mChipBackgroundColor);
	        int backgroundColorSelected = (GetBackgroundColorSelected(position) != 0 ? GetBackgroundColorSelected(position) : mChipBackgroundColorSelected);

	        // Default state
	        GradientDrawable background = new GradientDrawable();
			background.SetShape (ShapeType.Rectangle);
			background.SetCornerRadius (mChipCornerRadius);
			background.SetColor(backgroundColor);

	        // Selected state
	        GradientDrawable selectedBackground = new GradientDrawable();
			selectedBackground.SetShape(ShapeType.Rectangle);
	        selectedBackground.SetCornerRadius(mChipCornerRadius);
	        selectedBackground.SetColor(backgroundColorSelected);

			IList<int> wildCardList = StateSet.WildCard;
		
			int [] wildCardArray = new int[wildCardList.Count];
			wildCardList.CopyTo(wildCardArray, 0);

	        StateListDrawable stateListDrawable = new StateListDrawable();
			stateListDrawable.AddState(new int[]{Android.Resource.Attribute.StatePressed}, selectedBackground);
			stateListDrawable.AddState(new int[]{Android.Resource.Attribute.StateFocused}, selectedBackground);
			stateListDrawable.AddState (wildCardArray, background);



	        return stateListDrawable;
	    }

	    private void NotifyUpdate() {
			SetChanged ();
	        NotifyObservers();
	    }

	    public void Add(Chip chip) {
	        if (!mChipList.Contains(chip) || mToleratingDuplicate) {
	            mChipList.Add(chip);
	            NotifyUpdate();
	        }
	    }
			
	    public void Remove(Chip chip) {
	        mChipList.Remove(chip);
	        NotifyUpdate();
	    }

	    public int Count() {
			return mChipList.Count - 1;
	    }

	    protected int GetColor(int colorRes) {
			return mContext.Resources.GetColor(colorRes);
	    }

		public Context GetContext() {
	        return mContext;
	    }

	    public AttributeSet GetAttributeSet() {
	        return mAttributeSet;
	    }

	    public void SetAttributeSet(AttributeSet attributeSet) {
	        mAttributeSet = attributeSet;
	        Init();
	    }

	    public List<Chip> GetChipList() {
	        return mChipList;
	    }

		public Chip GetChipAt(int position){
			return mChipList [position];
		}

		public Chip GetLastChip(){
			return mChipList [Count()];
		}

	    public void SetChipList(List<Chip> chipList) {
			chipList.Add(new Chip("@NULL", "@NULL"));
	        mChipList = chipList;
	        NotifyUpdate();
	    }

	    public bool IsToleratingDuplicate() {
	        return mToleratingDuplicate;
	    }
			
	    public void SetToleratingDuplicate(bool toleratingDuplicate) {
	        mToleratingDuplicate = toleratingDuplicate;
	    }

	    public bool HasBackground() {
	        return mHasBackground;
	    }

	    public void SetHasBackground(bool hasBackground) {
	        mHasBackground = hasBackground;
	    }

	    public int GetChipSpacing() {
	        return mChipSpacing;
	    }

	    public void SetChipSpacing(int chipSpacing) {
	        mChipSpacing = chipSpacing;
	    }

	    public int GetLineSpacing() {
	        return mLineSpacing;
	    }

	    public void SetLineSpacing(int lineSpacing) {
	        mLineSpacing = lineSpacing;
	    }

	    public int GetChipPadding() {
	        return mChipPadding;
	    }

	    public void SetChipPadding(int chipPadding) {
	        mChipPadding = chipPadding;
	    }

	    public int GetChipSidePadding() {
	        return mChipSidePadding;
	    }

	    public void SetChipSidePadding(int chipSidePadding) {
	        mChipSidePadding = chipSidePadding;
	    }

	    public int GetChipCornerRadius() {
	        return mChipCornerRadius;
	    }

	    public void SetChipCornerRadius(int chipCornerRadius) {
	        mChipCornerRadius = chipCornerRadius;
	    }

	    public int GetChipBackgroundColor() {
	        return mChipBackgroundColor;
	    }

	    public void SetChipBackgroundColor(int chipBackgroundColor) {
			mChipBackgroundColor = chipBackgroundColor;
	    }

	    public int GetChipBackgroundColorSelected() {
	        return mChipBackgroundColorSelected;
	    }

	    public void SetChipBackgroundColorSelected(int chipBackgroundColorSelected) {
	        mChipBackgroundColorSelected = chipBackgroundColorSelected;
	    }

	    public int GetChipTextSize() {
	        return mChipTextSize;
	    }

	    public void SetChipTextSize(int chipTextSize) {
	        mChipTextSize = chipTextSize;
	    }

	    public void SetChipBackgroundResource(int backgroundRes) {
	        mChipBackgroundRes = backgroundRes;
	    }

	    public int GetChipLayoutResource() {
	        return mChipRes;
	    }

	    public void SetChipLayoutResource(int chipRes) {
	        mChipRes = chipRes;
	    }
	}
}
